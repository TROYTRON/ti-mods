"""Game-independent handbook checks; Python 3.11+ standard library only.

Checks strict JSON, local Markdown/HTML link targets and Markdown anchors,
maintained mod metadata, C# reference packaging, and prohibited new binaries.
External URLs are researched separately, not requested by CI.
"""
import argparse
import html
import json
from pathlib import Path
import re
import subprocess
import sys
from urllib.parse import unquote, urlsplit
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[1]


def without_fences(text):
    return re.sub(r"(?ms)^\s*(`{3,}|~{3,})[^\n]*\n.*?^\s*\1\s*$", "", text)


def anchors(text):
    found, counts = set(), {}
    for heading in re.findall(r"(?m)^ {0,3}#{1,6}\s+(.+?)\s*#*\s*$", without_fences(text)):
        heading = re.sub(r"<[^>]+>", "", html.unescape(heading)).lower()
        slug = re.sub(r"[^\w\- ]", "", heading).replace(" ", "-")
        count = counts.get(slug, 0)
        counts[slug] = count + 1
        found.add(f"{slug}-{count}" if count else slug)
    found.update(re.findall(r'<(?:a|h[1-6])\b[^>]*(?:id|name)=["\']([^"\']+)', text))
    return found


def local_links(text):
    text = without_fences(text)
    # Ordinary inline links/images, reference definitions, and HTML href/src.
    for match in re.finditer(r"\[[^\]\n]*\]\((<[^>\n]+>|[^\n)]*)\)", text):
        target = match.group(1).strip()
        if target.startswith("<"):
            yield target[1:-1]
        else:
            yield re.split(r'\s+["\']', target, maxsplit=1)[0]
    for match in re.finditer(r"(?m)^\s*\[[^\]]+\]:\s*(<[^>]+>|\S+)", text):
        yield match.group(1).strip("<>")
    yield from re.findall(r'(?:href|src)=["\']([^"\']+)', text)


def link_errors(root, path, text):
    errors = []
    for target in local_links(text):
        parts = urlsplit(target)
        if parts.scheme or parts.netloc:
            continue
        decoded = unquote(parts.path)
        dest = (root / decoded.lstrip("/") if decoded.startswith("/")
                else path.parent / decoded) if decoded else path
        dest = dest.resolve()
        if not dest.is_relative_to(root.resolve()):
            errors.append(f"{path.relative_to(root)}: link leaves repository: {target}")
        elif not dest.exists():
            errors.append(f"{path.relative_to(root)}: missing link target: {target}")
        elif parts.fragment and dest.suffix.lower() == ".md":
            fragment = unquote(parts.fragment)
            if fragment not in anchors(dest.read_text(encoding="utf-8-sig")):
                errors.append(f"{path.relative_to(root)}: missing anchor: {target}")
    return errors


def strict_json(text):
    def pairs(items):
        result = {}
        for key, value in items:
            if key in result:
                raise ValueError(f"duplicate key {key!r}")
            result[key] = value
        return result

    def constant(value):
        raise ValueError(f"non-JSON numeric constant {value}")

    return json.loads(text, object_pairs_hook=pairs, parse_constant=constant)


def metadata_errors(path, value):
    errors = []
    if not isinstance(value, dict):
        return [f"{path}: metadata must be an object"]
    fields = ("Id", "DisplayName", "Version", "AssemblyName", "EntryMethod") if path.name == "ModFile.json" else ("Title", "Author", "Description")
    for field in fields:
        if not isinstance(value.get(field), str) or not value[field].strip():
            errors.append(f"{path}: missing/non-string {field}")
    if "LoadOrder" in value and type(value["LoadOrder"]) is not int:
        errors.append(f"{path}: LoadOrder must be an integer")
    for field in ("TemplatesToConcatArrays", "TemplatesToReplaceArrays", "TemplatesToReplace"):
        if field in value and (not isinstance(value[field], list) or
                               any(not isinstance(v, str) or not v.endswith(".json") for v in value[field])):
            errors.append(f"{path}: {field} must list JSON filenames")
    return errors


def files(root):
    try:
        result = subprocess.check_output(["git", "ls-files", "-z", "--cached", "--others", "--exclude-standard"], cwd=root)
        return sorted({root / p for p in result.decode("utf-8").split("\0") if p and (root / p).is_file()})
    except (OSError, subprocess.CalledProcessError):
        return sorted(p for p in root.rglob("*") if p.is_file() and not any(
            part in {".git", ".local", "bin", "obj", "__pycache__"} for part in p.relative_to(root).parts))


def validate(root):
    errors, counts = [], {"markdown": 0, "json": 0, "json_snippets": 0, "projects": 0}
    for path in files(root):
        rel = path.relative_to(root)
        if path.suffix.lower() == ".md":
            counts["markdown"] += 1
            text = path.read_text(encoding="utf-8-sig")
            errors.extend(link_errors(root, path, text))
            for index, snippet in enumerate(re.findall(r"(?ms)^```json[ \t]*\n(.*?)^```[ \t]*$", text), 1):
                counts["json_snippets"] += 1
                try:
                    strict_json(snippet)
                except ValueError as exc:
                    errors.append(f"{rel}: invalid JSON fence {index}: {exc}")
        if path.suffix.lower() == ".json":
            counts["json"] += 1
            try:
                value = strict_json(path.read_text(encoding="utf-8-sig"))
                if path.name in {"ModInfo.json", "ModFile.json"}:
                    errors.extend(metadata_errors(rel, value))
                if path.name.startswith("TI") and path.name.endswith("Template.json"):
                    if not isinstance(value, list) or any(not isinstance(v, dict) or not isinstance(v.get("dataName"), str) for v in value):
                        errors.append(f"{rel}: template must be an array of named records")
            except (ValueError, UnicodeError) as exc:
                errors.append(f"{rel}: invalid JSON: {exc}")
        if rel.parts[0] == "examples" and path.suffix in {".csproj", ".props", ".targets"}:
            counts["projects"] += path.suffix == ".csproj"
            try:
                xml = ET.parse(path)
                for reference in xml.findall(".//Reference"):
                    if "Remove" in reference.attrib:
                        continue
                    private = reference.find("Private")
                    if private is None or (private.text or "").lower() != "false":
                        errors.append(f"{rel}: local assembly reference must have Private=false")
            except ET.ParseError as exc:
                errors.append(f"{rel}: invalid MSBuild XML: {exc}")
        if path.suffix.lower() in {".dll", ".exe", ".pdb", ".save", ".sav"}:
            errors.append(f"{rel}: compiled binary/save must not be published")
        if path.suffix.lower() == ".zip":
            try:
                with zipfile.ZipFile(path) as archive:
                    for entry in archive.namelist():
                        name = Path(entry.replace('\\', '/')).name.lower()
                        if name.endswith('.dll') and (name.startswith(('assembly-csharp', 'unityengine', 'unitymodmanager')) or name == '0harmony.dll'):
                            errors.append(f"{rel}: archive contains game/loader dependency {entry}")
            except zipfile.BadZipFile as exc:
                errors.append(f"{rel}: unreadable ZIP: {exc}")
    return errors, counts


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=ROOT)
    args = parser.parse_args()
    errors, counts = validate(args.root.resolve())
    for error in errors:
        print(error)
    print(f"Checked {counts['markdown']} Markdown files, {counts['json']} JSON files, {counts['json_snippets']} JSON snippets, {counts['projects']} C# projects; {len(errors)} errors.")
    return bool(errors)


if __name__ == "__main__":
    sys.exit(main())

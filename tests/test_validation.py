"""Regression cases for checks that protect publishable handbook examples."""
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from scripts.validate import link_errors, metadata_errors, strict_json, validate


class ValidationTests(unittest.TestCase):
    def test_json_rejects_duplicate_keys_trailing_comma_and_nan(self):
        for text in ('{"x": 1, "x": 2}', '[1,]', '{"x": NaN}'):
            with self.subTest(text=text), self.assertRaises(ValueError):
                strict_json(text)

    def test_encoded_paths_root_links_and_anchors(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source = root / "index.md"
            source.write_text("# Index\n", encoding="utf-8")
            (root / "A guide.md").write_text("# Heading\n# Heading\n", encoding="utf-8")
            self.assertEqual([], link_errors(root, source, '[a](A%20guide.md#heading-1) [b](/A%20guide.md)'))
            self.assertEqual(1, len(link_errors(root, source, '[x](missing.md)')))
            self.assertEqual(1, len(link_errors(root, source, '[x](A%20guide.md#absent)')))
            self.assertEqual([], link_errors(root, source, '```md\n[x](missing.md)\n```'))

    def test_metadata_rejects_boolean_order_and_malformed_array_controls(self):
        value = {"Title": "Test", "Author": "Author", "Description": "Example", "LoadOrder": True,
                 "TemplatesToReplace": "TIGlobalConfig.json"}
        self.assertEqual(2, len(metadata_errors(Path("ModInfo.json"), value)))

    def test_reference_removal_is_not_a_copied_dependency(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            project = root / "examples" / "Check.csproj"
            project.parent.mkdir()
            project.write_text('<Project><ItemGroup><Reference Remove="UnityEngine" />'
                               '<Reference Include="Game"><Private>false</Private></Reference>'
                               '</ItemGroup></Project>', encoding="utf-8")
            with patch("scripts.validate.files", return_value=[project]):
                self.assertEqual([], validate(root)[0])
            project.write_text('<Project><ItemGroup><Reference Include="Game" />'
                               '</ItemGroup></Project>', encoding="utf-8")
            with patch("scripts.validate.files", return_value=[project]):
                self.assertEqual(1, len(validate(root)[0]))


if __name__ == "__main__":
    unittest.main()

# Map authoring: regions, outlines, and geometry

For Windows stable **1.0.53a** with **Dark Skies**. The parser constraints below are checked against the game code; a complete export, geometry-generation and reimport workflow remains unverified. SVG input alone does not create a playable map.

The community [Creating Regions and Nations guide](https://steamcommunity.com/sharedfiles/filedetails/?id=2965074107) provides additional background; compare its examples with your game version.

## Establish which layer you are changing

| Layer | What it controls | Required consistency |
| --- | --- | --- |
| Region/nation templates | Identity, ownership, claims, population/economy, scenario setup | References resolve to intended template identities |
| Outline collection | Polygon/curve data, region and nation labels | Outline names correspond to template map references |
| Generated geometry | Surfaces, borders, mesh/collider-related assets | Shapes and selection agree on the globe |
| Rendered labels and markers | Positions and presentation | Locations align with the new geography |

An existing region reassigned to another nation can be primarily a template/scenario change. Splitting its shape adds asset work: new outline data and corresponding generated geometry must exist as well as the new region template. Changing a `mapRegionName` reference does not generate a missing region surface.

## Inspect a small existing region

Start with a copy of one installed region template and the corresponding asset references. Record its `dataName`, `mapRegionName`, nation references, adjacency-related data, and the source scenario. Use [AssetRipper or another compatible inspector](../docs/tools.md) to study the related bundle and object hierarchy; keep the original game files unchanged.

Trace the consuming code as well as the export: `TIRegionOutline` contains `poly2DList`, `regionShapes`, `regionSurfacePoints`, and `labelPositions`, with separate methods such as `ToMesh`, `ToMesh2`, and `ToBorder`. These fields belong to different processing stages. Retain their types and serialized structure when editing an exported asset.

## Prepare SVG input deliberately

`SVGRegionParser.ParseSVG` is a specialized reader, not a full SVG renderer. It reads Inkscape layers using `inkscape:groupmode="layer"` and `inkscape:label`, splits a layer label at `:`, and derives nation/region labels. A layer marked `display:none` is skipped. Paths contribute polygon data to the current region; text can contribute label positions.

Keep layer labels in the format expected by your source collection, for example `TAG: Region Name`, and preserve the required XML namespace declarations. Do not assume an arbitrary SVG `id` is the region's template identity. Ordinary groups, clipping, masks, nested transforms, and an editor's visual appearance do not establish support in this reader.

The parser reads document `width` and `height` and strips non-digits before parsing them. This makes values such as `1024.5` particularly unsafe: they can become `10245`. Use explicit integer dimensions and bake transforms into path coordinates. Do not rely on a `viewBox` or physical-unit conversion to repair dimensions automatically.

Prefer **absolute coordinates** in saved paths. In Inkscape, inspect the SVG output/path-data preferences and the actual saved `d` attributes. `ParsePath` handles some relative commands, but not every SVG feature. Use a simple subset and compare the resulting outline with its source before building a world map.

Tayta's Inkscape setup uses **Edit > Preferences > Input/Output > SVG output > Path data > Path string format: Absolute**, and **Smoothing: 8** when drawing freehand lines. The smoothing value is a practical starting point, not a parser requirement; inspect the resulting coastlines and borders.

## Preserve the coordinate space

`CurvedPolyPoint.NormalizeToRadial` transforms SVG anchors and Bezier control points through:

```text
x_rad = 2π × (x_svg / document_width  − 0.5)
y_rad =  π × (y_svg / document_height − 0.5)
```

Those values are the outline pipeline's radial coordinates. They are not a ready-to-use Unity `Vector3`, and the SVG y direction is not automatically geographic north. Match a known point in the source map, then follow the game's projection/mesh code to establish axis orientation. Do not feed latitude/longitude degrees directly into fields that expect radians or projected positions.

Use a polygon or SVG parser that retains nested arrays, curves, and topology when converting formats. Regex substitutions cannot reliably parse general SVG or nested JSON. Likewise, serializing a type tree at depth zero loses nested coordinates and references; a short JSON export is not proof that its geometry survived. Compare point counts, bounds, disconnected islands, holes, and closed edges before and after conversion.

## Complete the processing and deployment chain

1. Prepare and validate source outlines against a small known region, including layer names, integer dimensions, coordinates, closed shapes, and labels.
2. Run the matching outline-to-geometry authoring process for the chosen game revision. If working from an exported project, identify and restore the tools/components needed for that stage. The presence of parser code in the player does not imply an exposed mod-menu importer.
3. Inspect the generated surfaces/borders and any collision/selection objects. Ensure meshes, materials, required scripts, and references survived export/reimport. The generic bundle helper only packages assets that exist; it does not generate this geometry.
4. Build with the [Unity asset workflow](../docs/assets.md) and package content bundles with their manifests.
5. Update region/nation/scenario templates consistently. Verify every new identifier and reference, keeping gameplay data separate from projected mesh data. Use the [native JSON guide](Create_Template_JSON_mod.md).
6. Start a disposable new campaign in the intended scenario. Check visual borders, region selection, label position, neighbors and travel, ownership, claims, and save/reload. Check both sides of the date line and small islands when applicable.

If you cannot identify the geometry-generation step for your source assets, stop at a documented outline prototype and record that missing step. Do not publish a successful SVG parse or JSON conversion as a working-map result.

## Keep source assets and test results

Keep source SVG, conversion/export settings, tool versions, generated asset names, mod template entries, and the game build/branch/DLC. Capture one close view and one globe view, plus logs from campaign creation and reload. A visual-only check can miss incorrect selection or gameplay connectivity; a startup-only check misses both.

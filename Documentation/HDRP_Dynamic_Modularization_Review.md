# HDRP Dynamic Modularization Review

## Code check summary

### What was run
- `dotnet build Lumina.sln -c Release`
- `npm ci` (in `Lumina/`)
- `npm run build` (in `Lumina/`)
- `npx tsc --noEmit` (in `Lumina/`)

### Results
- `dotnet build Lumina.sln -c Release` could not run in this environment because `dotnet` is not installed.
- `npm ci` completed successfully.
- `npm run build` failed because `CSII_USERDATAPATH` is not set in the environment.
- `npx tsc --noEmit` completed successfully.

## Findings: why HDRP controls are not yet fully dynamic

The current architecture already exposes many HDRP controls, but they are assembled as a mostly hardcoded set of feature paths.

1. **Render update path is centralized and fixed**  
   `RenderEffectsSystem.OnUpdate` calls component handlers directly (`ColorAdjustments`, `WhiteBalance`, `ShadowsMidTonesHighlights`) each update loop, which makes growth more manual than declarative.

2. **UI binding registration is very large and static**  
   `UISystem.CreateBindings` manually registers a long list of sliders, toggles, and actions. This is powerful but means every new component requires touching core binding code.

3. **Configuration is property-per-feature**  
   `GlobalVariables` stores many explicit XML fields for each control. This is easy to debug, but extending to many additional HDRP components can become expensive in maintenance and migration.

## Suggestions: making HDRP component selection customer-driven

### 1) Introduce a component registry (first step)
Create a registry that declares each controllable HDRP feature as metadata, for example:

- `ComponentId` (`"colorAdjustments"`, `"whiteBalance"`, etc.)
- `VolumeComponentType` (`ColorAdjustments`, `WhiteBalance`, ...)
- `DisplayName`
- `Category` (Color, Lighting, Atmosphere, Post)
- `DefaultEnabled`
- `PropertyDescriptors` (name, type, min/max, default)

This allows customer-facing UI to render from data instead of hardcoded per-component UI blocks.

### 2) Add per-component enablement in persisted settings
Add a dictionary-style persisted setting such as:

- `Dictionary<string, bool> EnabledComponents`
- (optional) `Dictionary<string, Dictionary<string, object>> ComponentOverrides`

Keep existing fields for backward compatibility initially, then gradually migrate.

### 3) Refactor runtime updates to strategy handlers
Replace direct monolithic calls with handlers implementing a shared interface, e.g.:

- `IHdrpComponentHandler.Initialize(volumeProfile)`
- `IHdrpComponentHandler.Apply(settingsSnapshot)`
- `IHdrpComponentHandler.ResetToDefault()`

The main render system loops registered handlers and only executes enabled components.

### 4) Generate UI bindings from descriptors
In `UISystem`, generate bindings from registry descriptors:

- auto-create getter/setter binding names
- auto-wire slider ranges and checkbox state
- hide disabled components by policy/profile

This is the key to allowing customers to choose which component categories are shown/active.

### 5) Add customer profiles / presets by component groups
Support profiles such as:

- **Performance Profile** (minimal effects)
- **Cinematic Profile** (full effects)
- **Colorist Profile** (color stack only)

Each profile is a predefined set of enabled component IDs plus defaults.

### 6) Compatibility and safety gates
Before enabling a component at runtime, gate by:

- game version support
- installed incompatible mods
- GPU tier/performance mode

This aligns with existing compatibility checks and avoids user-exposed controls that cannot run safely on certain setups.

### 7) Migration plan (low risk)
1. Introduce registry + handlers for existing 3 core components.
2. Keep old fields and map them into new internal model.
3. Convert UI binding for one section (e.g., White Balance) to generated mode.
4. Expand to tonemapping, sky, and clouds.
5. Deprecate duplicated hardcoded paths.

## Practical next milestone

A pragmatic first deliverable:

- Add component registry classes.
- Register current components (`ColorAdjustments`, `WhiteBalance`, `ShadowsMidtonesHighlights`).
- Add persisted `EnabledComponents` with defaults set to current behavior.
- Render one dynamic UI section from descriptors.

This would prove the pattern while keeping current behavior stable.

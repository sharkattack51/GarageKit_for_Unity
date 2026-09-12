# Changelog

All notable changes to Graphy are documented in this file.

## [4.0.0] - 2026-08-29

### Added
- Added Universal Render Pipeline support while retaining Built-in Render Pipeline support.
- Added a global UI scale setting.
- Added a shared module manager base class for FPS, RAM, and Audio modules.

### Improved
- Reworked average FPS and 1% / 0.1% low calculations to use frame times.
- Replaced graph array shifting with circular buffers.
- Replaced full FPS sample sorting with partial low-sample selection.
- Reused incremental sums, shader initialization arrays, cached predicates, enum counts, XR subsystems, and debugger query results to reduce allocations and repeated work.
- Avoided unnecessary graph material and audio buffer reconstruction during parameter updates.
- Optimized graph fragment shader work and reduced threshold line opacity.
- Synchronized module states and presets so cosmetic changes no longer reset visibility.
- Separated Legacy Input Manager and Input System hotkey fields to keep serialized key values stable.

### Removed
- Removed the legacy customization scene package and import menu.

### Fixed
- Fixed graph corner positioning by updating module pivots.
- Fixed graph material replacement to destroy old runtime materials without destroying shared assets.
- Fixed FPS percent-low warm-up values and the normalized average graph line.
- Fixed GraphyDebugger ID lookups when no matching packet exists.
- Fixed audio text timing while the game is paused.
- Fixed RAM initialization range and update-rate type mismatches.
- Fixed Device Simulator crashes when the reported refresh rate is unavailable.
- Validated graph resolutions and spectrum sizes to prevent invalid shader array access and audio graph values.

### Compatibility
- Increased the minimum supported Unity version to 2022.3 LTS.

## [3.0.5]

- Fixed small issue with Safe Area not being updated when the screen resolution changes.

## [3.0.4]

- Fixed small oversight in Resolution.refreshRate is obsolete warning fix.

## [3.0.3]

- Fixed some comments and removed unused using statements.
- Fixed Resolution.refreshRate is obsolete warning for Unity 2022.2 and newer.
- Increased minimum version to recommended Unity 2019.4 LTS.

## [3.0.2]

- Renamed GRAPHY_VR to GRAPHY_XR
- Added GRAPHY_XR to missing places to fix code compilation

## [3.0.1]

- Added GRAPHY_VR scripting define.

## [3.0.0]

- Added beta support for VR!
- Added support for an offset in all modules.
- Big refactor and cleanup.

## [2.1.3]

- Added null check for Keyboard.current.

## [2.1.2]

- Fixed NullRef in AudioMonitor if no main camera is in the scene.

## [2.1.1]

- Small hotfix for a index of out bounds error.
- Simplified Singleton class to allow Graphy to be Destroyed.
- Deallocating cached strings on Destroy to free up memory.

## [2.1.0]

- Pretty big refactor.
- Lots of optimization.
- Removed min/max fps for 1% and 0.1% lows, which is the industry standard now.
- Improved UI. Added rounded corners and a better default color palette.

## [2.0.1]

- Updated package.json to 2.0.1.

## [2.0.0]

- Now supports UPM (Unity Package Manager).
- Minimum official supported version is now Unity 2019.3.0. If you want a version that supports 5.4+, go to Github to download it.
- Lots of minor tweaks, optimizations and bugfixes.

## [1.6.0]

This is the last release that will officially support Unity 5.4+. Next releases will be targeted towards 2019.3+.

- Minor tweaks, optimizations and bugfixes.
- Added support for VR single pass instanced rendering.

## [1.5.2]

- Removed asmdefs to avoid missing reference issues in new Unity alpha versions.
- Improvements avgFps calculation, changed list to array (thanks @Kaladrius2trip).

## [1.5.1]

- Hide Graphy in Game view when it's be disabled on startup.
- Fixed error if no camera is present in the scene.
- Added SceneManager namespace so to avoid possible conflicts.
- Added support for asmdefs (thanks @QSFW).

## [1.5]

- Fixed a number alignment in the audio module (thanks @SuperPenguin).
- Refactored some code to avoid warnings with the new NET framework 4.0.
- Shader sorting fix for Screen Space - Camera.
- Fixed a possible Null Reference Error in the debugger (thanks @strawlink).
- Fixed import settings in 2 textures (thanks @strawlink).
- Renamed all Action into System.Action to avoid possible namespace conflicts.
- Fixed the int rounding to prevent 59.99999FPS from turning into 59FPS (thanks @Rockylars).

## [1.4.3]

- Renamed all the classes with the "G_" prefix to avoid namespace issues with external code (thanks @Rockylars).
- Refactored some code and added explanations and regions in the G_ShaderGraph class.
- Assigned all the variables in their declaration to avoid a new NET framework warning.

## [1.4.2]

- Added the option to disable hotkeys.
- Disabled hotkey check when Editor is not focused (thanks @Rockylars).
- Refatored and cleaned up code (thanks @Rockylars).
- Fixed a bug where if the app was defocused and focused back, it would reset Graphy's module active values (thanks @Rockylars).

## [1.4.1]

- Introduced plenty of safety checks to avoid some null reference errors.
- Possibly fixed the graphs bug when the Editor is defocused and focused back.
- Code cleanup and refactoring.

## [1.4]

- Updated the header comments in all scripts.
- Added option to toggle active on start up (thanks @DarkMio).
- Removed a leftover raycast script in the Graphy UI.Canvas (thanks @DarkMio).
- Updated the shaders to use UnityObjectToClipPos() (thanks @DarkMio).
- Bug-Fix: NullRef for EditorStyles.boldlabel (thanks @Flavelius)

## [1.3]

- Added a second graph to the Audio module that shows the highest spectrum value.
- Added option to Toggle Active and Mode, as well as setting a specific Preset from the API.
- Fixed a bug that occured when Time.timeScale = 0 (thanks @xDavidLeon!).

## [1.2.2]

- Improved the dB calculations, now the values are much more precise.

## [1.2.1]

- Modified the default UI text values to more generic placeholders to increase clarity.
- Small fixes in the audio module.

## [1.2]

- MASSIVE reduction in garbage generation. From 8-10 KB every 2-3 seconds to just 200-300 bytes. Garbage generation right now is negligible.
- Some code optimizations.

## [1.1]

- New Feature: Added a modifiable MODE. If set to LIGHT it will reduce some features or maximum values (like graph resolution) but it will improve compatibility with older hardware.
- Small performance optimizations.
- Code refactoring.
- Improved the vetical alpha fade-off effect in the graph to make it more visible for lower values.
- Updated the "Customize Graphy" scene to account for these new changes.

## [1.0]

- First major update!
- Removed some leftover raycast targets from the Graphy UI to avoid interfering with users UI.
- Added a Customization Scene that allows changing all the parameters in runtime to improve the user experience when testing new values.
- Added a feature to rescale the background overlay of the Advanced Data module to the text with the highest width.
- Made ALL parameters modifiable from code using the API.
- Fixed a bug where sometimes the Graphy Manager would fail trying to retrieve the Audio Module.
- Improved stabilty.

## [0.6]

- Added a feature to choose if you want to apply a background overlay to Graphy, improving readability in cluttered scenes.
- Optimized the access to Shader parameters when updating them, improving performance.

## [0.5.1]

- Added a feature to choose if you want to keep Graphy alive through scene changes. Careful, if you activate it but Graphy is the child of another object, the root GameObject will also survive scene changes.
- Fixed a bug where setting Graphy as a child of another object would break the graphs.

## [0.5]

- Initial release!

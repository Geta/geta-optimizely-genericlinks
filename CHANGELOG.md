# Changelog

All notable changes to this project will be documented in this file.

## [1.1.4]

### Added

- Upper bound on package version constraint for `EPiServer.CMS.UI`.

## [1.1.3]

### Changed

- Adjusted reference nullability for methods `LoadData` and `ParseToSelf` in property implementations.
- Adjusted reference nullability for action in `SetAttribute` to represent actual functionality.
- Clarified supported behaviour in `NewtonsoftLinkDataConverter` and `SystemTextLinkDataConverter` by throwing appropriate exceptions.

## [1.1.2]

### Fixed

- Bug PropertyLinkData would not reset `IsModified` correctly when calling `MakeReadonly`.

## [1.1.1]

### Fixed

- Bug where changing Text property would not trigger `IsModified` for LinkData.

## [1.1.0]

### Added

- Added support for `Int32`, `Double` and `DateTime`.
- Examples in readme.

### Fixed

- Bug where `target` would not be persisted correctly due to mismatch in edit model.

## [1.0.2]

### Fixed

- Fixed editor inclusion

## [1.0.0]

### Added

- Added initial implementation

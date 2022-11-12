# Changelog

All notable changes to this project will be documented in this file.

## [1.4.3]

### Fixed

- Addressed an issue where content creation screen would be blank if `PropertyLinkData` based property was decorated with `[Required]`.

## [1.4.2]

### Fixed

- Addressed a suspected concurrency issue where `LinkData.GetAttributeKey` probably wasn't thread safe'.

## [1.4.1]

### Fixed

- Fixed potential issue with template cache collision.

## [1.4.0]

### Added

- Support for `EPiServer.ContentDeliveryApi`.

## [1.3.4]

### Fixed

- Addressed an issue where `SystemTextLinkDataConverter` was used during block creation by implementing reading.

## [1.3.3]

### Fixed

- Fixed an issue with "Select Content" button added in Episerver.CMS 12.7

## [1.3.1]

### Fixed

- Bug with `[Required]` not working properly for `LinkData` property `Text`.
- Bug with single editor not registering changes for items created with `...` window.
- Bug with items without links becoming saved as empty because of bug in `IsNull` evaluation.

## [1.3.0]

### Added

- Added extension methods `GetMappedHref`, `ToMappedLink` and `ToPermanentLink` for `LinkData` to further emulate behaviour of `LinkItem`.

## [1.2.0]

### Modified

- Removed `[Required]` attribute from `Href` on `LinkData`.

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

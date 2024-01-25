# Changelog

All notable changes to this project will be documented in this file.

## [1.8.3]

### Fixed

- Fixed an issue with data not being copied correctly due to issues with implementation of `Value` properties in `PropertyLinkData` and `PropertyLinkDataCollection` (#27).

## [1.8.2]

### Fixed

- Fixed an exception in `LinkDataBackingTypeResolverInterceptor` (#26).

## [1.8.1]

### Fixed

- Issue with breaking change in `Optimizely.CMS.UI 12.24.0` (#25).

## [1.8.0]

### Added

- Import/Export functionality (#22).

### Fixed

- Potential issue in `DefaultLinkHtmlSerializer` where data would be lost if no link url was present.

## [1.7.0]

### Added

- Default value handling for `LinkData` properties (#21).

## [1.6.2]

### Fixed

- Issue with change tracking and delete button (#20).

## [1.6.1]

### Fixed

- Issue with auto save on cancel (introduced in CMS ~12.19) (#17).
- Issue with comparison view when an external link is defined (#18).
- Issue with select content modal (introduced in CMS ~12.19) (#19).

## [1.6.0]

### Removed

- Removed `[Required]` on `LinkData.Text` property. Node will display as `{unnamed}` in editor if `Text` is omitted.

## [1.5.0]

### Added

- Added support for writing in `NewtonsoftLinkDataConverter` for edge cases where the default serialization handling has been modified.

## [1.4.5]

### Fixed

- Issue with `GenericItemEditor` where contents would not be initialized properly if `_setValueAttr` fires before `postCreate` (happens in on-page edit and quick edit).
- Issue with source control triggering change after every build because of `CopyZipFiles.targets`.

## [1.4.4]

### Fixed

- Added additional package information.

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

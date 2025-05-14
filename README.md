# Geta.Optimizely.GenericLinks

![](<http://tc.geta.no/app/rest/builds/buildType:(id:GetaPackages_Genericlinks_00ci),branch:master/statusIcon>)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Geta_geta-optimizely-genericlinks&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Geta_geta-optimizely-genericlinks)
[![Platform](https://img.shields.io/badge/Platform-.NET%206-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-orange.svg?style=flat)](http://world.episerver.com/cms/)

## What is does?

Provides a new property type `LinkDataCollection` which is an alternative to `LinkItemCollection`. It can be extended with additional properties.

## How to get started?

Requires using .NET 5.0 or higher and Optimizely 12

- `dotnet add package Geta.Optimizely.GenericLinks`

Make sure to read the first example about implementation.

### New

- [Example how to add soft links for content references](./docs/example-soft-links.md)

### Examples

- [Example link collection with thumbnails](./docs/example-link-data-collection.md)
- [Example single link item](./docs/example-single-link.md)

### Documentation

- [Adding support for EPiServer.ContentDeliveryApi](./docs/content-delivery-api.md)
- [Advanced property handling](./docs/advanced-property-handling.md)
- [Adding support for additional property types](./docs/adding-support-for-new-properties.md)

## Package maintainer

https://github.com/svenrog

## Changelog

[Changelog](CHANGELOG.md)

# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

## [1.3.0] - 2024-05-05

### Added
- Ability to patch prefabs

## [1.2.0] - 2024-03-31

### Added
- Event when a config is changed

## [1.1.3] - 2024-03-05

### Changed
- Tweaked error message of analysis exception

## [1.1.2] - 2024-03-02

### Fixed
- Fixed another exception when analyzing types that contain unreferenced assemblies

## [1.1.1] - 2024-03-02

### Fixed
- Fixed an exception when analyzing types that contain unreferenced assemblies

## [1.1.0] - 2024-01-24

### Added
- Added reflection Invoke with return value

### Changed
- Made logging more generic

## [1.0.0] - 2024-01-22

### Added
- Added Postfix to ``Object.Instantiate()`` methods
- Patched last missing instantiate method ``Object.Instantiate<T>(T original)``
- Added utils for creating prefabs
- Added automatic binding of ConfigDatas

## [0.1.0] - 2024-01-20

### Added
- Patched ``Object.Instantiate(Object original)`` methods to modify original prefabs before cloning
- Patched ``GameObject.AddComponent()`` to automatically add code to any new component
- InjectToComponent Attribute to automatically add any MonoBehaviour to specified component
- Initializer Attribute to simulate Unity's RuntimeInitializeOnLoad
- SceneConstructor Attribute to execute code after a scene is loaded
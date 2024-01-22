# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

//

## [1.0.0] - 2024-01-22

### Added
- Patched last missing instantiate method ``Object.Instantiate<T>(T original)``

## [0.1.0] - 2024-01-20

### Added
- Patched ``Object.Instantiate(Object original)`` methods to modify original prefabs before cloning
- Patched ``GameObject.AddComponent()`` to automatically add code to any new component
- InjectToComponent Attribute to automatically add any MonoBehaviour to specified component
- Initializer Attribute to simulate Unity's RuntimeInitializeOnLoad
- SceneConstructor Attribute to execute code after a scene is loaded
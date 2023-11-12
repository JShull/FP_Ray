# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.2] - 2023-11-12

### 0.2.2 Fixed

- [@JShull](https://github.com/jshull)
  - FP_Raycaster.cs
    - Now can more easily derived with the use of protected instead of private
  - Documentation~ folder
    - Relocated built in 2D documentation to the default Unity Documentation~ folder

### 0.2.2 Added

- [@JShull](https://github.com/jshull)
  - Reference to FP_Utility.git
  - Modified ASMDEF to include cleaning up references
  - Added in new 2D Examples built by [@BlimpCat](https://github.com/BlimpCat) which be found under the Samples~/SamplesURP/Additional2D
    - Various Scenes added as breakdowns of each added raycast system
    - See Additional2D Folder in the SamplesURP folder

### 0.2.2 Removed

- [@JShull](https://github.com/jshull)
  - Removed Utility references to Drawing Spheres/Circles/Cubes

## [0.2.1] - 2023-09-06

### 0.2.1 Fixed

- [@JShull](https://github.com/jshull)
  - FP_Raycaster.cs hot fix
    - Some scenarios would result in our RayOrigin transform being removed/destroyed prior to the raycaster being deactivated and in those cases the raycaster wasn't catching that issue
    - Now running two separate invoked events, one containing the origin information the other not.

## [0.2.0] - 2022-11-28

### 0.2.0 Added

- [@JShull](https://github.com/jshull).
  - FP_RayDebug.cs
    - Added in a new Mono class for visualizing the FP_RayCaster (eventually get this to be an editor script)
    - Only requirement is that this class is attached to a mono class and you need to drag the reference in via the editor
  - 2DReadme.md & 2DReadme.pdf
    - Readme file to walk through how to use the raycaster in a 2D system
    - Goes with the Raycast2DURP example scene and is packaged with the URP Samples

### 0.2.0 Changed

- FP_RayMono.cs
  - Removed the visual debugging information and have added this to a separate class
  - Due to Unity being weird with interfaces and until I get more code written to support In Editor Interface references I am doing a component work around
- Rayast2DURP.unity
  - Scene file has been updated to include the new debug mono class
- Raycast3DURP.unity
  - Scene file has been updated to include the new debug mono class

### 0.2.0 Fixed

- None

### 0.2.0 Removed

- None

## [0.1.0] - 2022-11-06

### 0.1.0 Added

- [@JShull](https://github.com/jshull).
  - Moved all test files to a Unity Package Distribution
  - Setup the ChangeLog.md
  - Setup the Package Layout according to [Unity cus-Layout](https://docs.unity3d.com/Manual/cus-layout.html)
  - Humble Design has been setup with the PlayerController.cs script
    - Imported Samples provide the remaining Humble setup with the FP_Controller.cs script
  - See Samples in the Unity Package Manager to import an example
  - FP_Controller uses both old and new input - dependency on new input but you can swap the project to only use the old one and it will work
  - Scriptable Object setup to work with all other FuzzPhyte packages.
  - Added LICENSE.MD under a dual license for education and for business use cases

### 0.1.0 Changed

- None... yet

### 0.1.0 Fixed

- Setup the contents to align with Unity naming conventions

### 0.1.0 Removed

- None... yet

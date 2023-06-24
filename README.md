_For questions and discussion, check out our [Discord](https://discord.gg/dJ2rsenN4K)._

What is Sedulous?
====================

[Join the chat at https://discord.gg/dJ2rsenN4K](https://discord.gg/dJ2rsenN4K)

Sedulous is a cross-platform, .NET game development framework written in C# and released under the [MIT License](http://opensource.org/licenses/MIT). It is heavily inspired by Microsoft's XNA Framework, and is intended to be easy for XNA developers to quickly pick up and start using. However, unlike [MonoGame](http://www.monogame.net/) and similar projects, Sedulous is not intended to be a drop-in replacement for XNA. Its current implementation is written on top of [SDL2](https://www.libsdl.org/) and [OpenGL](https://www.opengl.org/), but its modular design makes it (relatively) easy to re-implement using other technologies if it becomes necessary to do so in the future.

At present, Sedulous officially supports Windows, Linux, and macOS using .NET 6, as well as Android through Xamarin.

Some core features of the Sedulous Framework:

 * __A runtime content pipeline__
   
   Easily load game assets using Sedulous's content pipeline. Unlike XNA, Sedulous's content pipeline operates at runtime, meaning no special Visual Studio projects are required to make it work. Content preprocessing is supported in order to increase efficiency and decrease load times.
 
 * __High-level 2D rendering abstractions__
   
   Familiar classes like SpriteBatch allow you to efficiently render large numbers of 2D sprites. Sedulous includes built-in support for texture atlases and XML-driven sprite sheets.
 
 * __High-level 3D rendering abstractions__
   
   Built-in support for [glTF 2.0 models](https://www.khronos.org/gltf/) and skinned animation makes it easy to get started with 3D rendering. Alternatively, you can write your own GLSL shader programs to take full control of the rendering process, and support for additional model types can be provided by extending the runtime content pipeline.
 
 * __Low-level rendering functionality__
   
   In addition to the abstractions described above, Sedulous's graphics subsystem allows you to push polygons directly to the graphics device, giving you complete control.
 
 * __A powerful text formatting and layout engine__
   
   Do more than draw plain strings of text. Sedulous's text formatting engine allows you to change your text's font, style, and color on the fly. The layout engine allows you to easily position and align text wherever you need it.
 
 * __XML-driven object loader for easy content creation__
   
   Sedulous's object loader allows you to easily create complicated hierarchies of objects from simple XML files. This is more than just an XML serializer&mdash;because it is integrated with Sedulous, it has direct knowledge of your game's content assets and object lists, making it possible to reference them in a simple, flexible, and readable way.

The Sedulous Framework's source code is [available on GitHub](https://github.com/jayrulez/Sedulous).

Sedulous is a fork of the [Ultraviolet framework](https://github.com/tlgkccampbell/ultraviolet)

- Below is a list of the major changes since the fork:
- Removed .NET Core 2 support.
- Use StmImageSharp and StbImageWriteSharp to replace System.Drawing for cross platform projects.
- Updated all projects to .NET 6
- Updated third party packages to latest available versions.
- Reorganized projects to get rid of Shared projects since there is now only a .NET 6 version of each.
- Changed how Platform compatibility shim is made available to context - removing the need for hard-coded assembly names.
- Properly detect Mono runtime for .NET 6
- Fixed CopyTo of BinaryHeap to make use of arrayIndex parameter
- Fixed incorrect assignment in Ray.Intersects
- Fixed pool expansion being leaky in core: Collections/ExpandingPool.cs
- Added FMOD binaries and re-enabled FMOD audio tests
- Added FMOD targets
- Added Ultraviolet.FMOD.Native.nuspe_
- Updated nuspec metadata (package versions)
- Fixed issue where android wasn't passing check for runtime code generation: b8f8096cc27baeb4ee914c666dbe77ffca760e5a
- Fixed shader compilation for GLES2 (in = varying for fragment shaders)
- Added missed x64 libs to Dependencies
- Packaging - Added android x64 libs to targets
- Renamed NETCore3 to NETCore
- Renamed packages from Ultraviolet to Sedulous to make publishing nuget packages possible
- Added template projects
- Package StbImageSharp and StbImageWriteSharp as Sedulous.StbImageSharp and Sedulous.StbImageWriteSharp respectively
- Removed most shared projects and .projitems files
- Reorganized files in Framework project to corresponding folders
- Renamed GL class to be upper-case
- Separated context creation from initialization. Context must now be initialized by the application. An exception will be thrown if you try to initialize a context twice.
- Added a Configure method to Plugin interface, similar to Initialization, but it is called before the context is initialized while - Initialize is called after.
- Explicitly register factories in plugins instead of depending on reflection and the factory initializer interface.
- Explicitly register shim factories.
- Arbitrary code can register factories on a context (only before the factory is initialized) by calling context.ConfigureFactory()
- Manually register factories for creating audio and graphics modules instead of loading them via reflection.
- Removed ViewProviderAssembly config - It is no longer necessary to register factories in view providers by reflection as they are manually registered.
- Call Interpolate explicitly to avoid reflection (Plane and Ray math types)
- Support Byte4 vertex format - Graphics/VertexDeclaration.cs (a9a9fa923cb8d65829b9fb8b28ed6aaff933e171)
- Fix STL model importer: 283c0d901a42549f1f6d7a73d873c8cb954a8005
- Updated SharpGLTF.Core package to version 1.0.0-alpha0029
- Added sample projects to repository
- Updated android projects to remove warnings
- Updated netstandard2.1 projects to net6.0
- Fixed BASS crash on Android: 6f2adec05df1a8400e75cb22b23e14c623f2a9bc
- Don't use git lfs for dependencies
- Simplified context initialization




Getting Started
===============

If you don't want to build Sedulous yourself, official packages are available through [NuGet](https://www.nuget.org/packages?q=sedulous).

The wiki contains a [quick start guide](https://github.com/jayrulez/Sedulous/wiki/Getting-Started-with-.NET-Core-3.1) for development using .NET 6.

The [Samples](https://github.com/jayrulez/Sedulous/tree/main/Samples) directory contains a number of sample projects which demonstrate various features of the Framework.

Requirements
============

Sedulous can be used with any version of .NET which supports .NET 6.

Building Sedulous requires .NET SDK 6.

Building the mobile projects requires the appropriate Xamarin tools to be installed.

The following platforms are supported for building the Framework:
* Windows
* Linux (Ubuntu)
* Android
* macOS

Please file an issue if you encounter any difficulty building on any of these platforms. Linux distributions other than Ubuntu should work, assuming that they can run .NET 6 and you can provide appropriate versions of the native dependencies, but only Ubuntu has been thoroughly tested.

Building
========

__Desktop Platforms__

The `Sources` folder contains several solution files for the various platforms which Sedulous supports. Alternatively, you can run `msbuild Sedulous.proj` from the command line in the repository's root directory; this will automatically select and build the correct solution for your current platform, and additionally will copy the build results into a single `Binaries` folder.

__Mobile Platforms__

Building Sedulous for Android requires that Xamarin be installed. As with the desktop version of the Framework, you can either build the appropriate solution file or `Sedulous.proj`, but in the latter case you must also explicitly specify that you want to use one of the mobile build targets, i.e.:

    msbuild Sedulous.proj /t:BuildAndroid

﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(@"Platform Compatibility Shim for Desktop")]
[assembly: AssemblyDescription(
    @"Provides Ultraviolet type implementations which are specific to the Windows and Linux platforms.")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("cf866f96-dc7a-4743-9073-c78921595c8b")]
using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly:AssemblyTitleAttribute("Nuclex.Fonts.Content.TrueTypeImporter")];
#ifdef XNA_4
[assembly:AssemblyDescriptionAttribute("TrueType font importer for XNA 4.0")];
#else
[assembly:AssemblyDescriptionAttribute("TrueType font importer for XNA 3.1")];
#endif
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("Nuclex Development Labs")];
[assembly:AssemblyProductAttribute("Nuclex.Fonts.Content.TrueTypeImporter")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) Nuclex Development Labs 2008-2010")];
[assembly:AssemblyTrademarkAttribute("")];
[assembly:AssemblyCultureAttribute("")];

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the value or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly:AssemblyVersionAttribute("4.0.0.0")];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];

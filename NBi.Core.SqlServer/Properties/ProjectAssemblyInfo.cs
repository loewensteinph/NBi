using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SqlServer2008R2
[assembly: AssemblyTitle("NBi.Core for SQL Server 2008R2")]
#elif SqlServer2012
[assembly: AssemblyTitle("NBi.Core for SQL Server 2012")]
#elif SqlServer2014
[assembly: AssemblyTitle("NBi.Core for SQL Server 2014")]
#elif SqlServer2016
[assembly: AssemblyTitle("NBi.Core for SQL Server 2016")]
#elif SqlServerVNext
[assembly: AssemblyTitle("NBi.Core for SQL Server vNext")]
#else
[assembly: AssemblyTitle("NBi.Core for SQL Server (unspecified version)")]
#endif
[assembly: AssemblyConfiguration("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8f55dfbb-fad0-4bde-8e1d-dc529db9cda0")]

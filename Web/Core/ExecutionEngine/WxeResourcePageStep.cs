using System;
using System.Reflection;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
///   Calls pages that are stored in the resource directory.
/// </summary>
/// <remarks>
///   The resource directory is <c>&lt;ApplicationRoot&gt;/res/&lt;AssemblyName&gt;/</c>.
/// </remarks>
public class WxeResourcePageStep: WxePageStep
{
  /// <summary>
  ///   Calls the page using the calling assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (string pageName)
    : this (Assembly.GetCallingAssembly(), pageName)
  {
  }

  /// <summary>
  ///   Calls the page using the resource directory of the assembly's type.
  /// </summary>
  public WxeResourcePageStep (Type resourceType, string pageName)
    : this (resourceType.Assembly, pageName)
  {
  }

  /// <summary>
  ///   Calls the page using the assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (Assembly resourceAssembly, string pageName)
    : base (GetFullName (resourceAssembly, pageName))
  {
  }

  private static string GetFullName (Assembly resourceAssembly, string pageName)
  {
    string assemblyName = resourceAssembly.FullName;
    int comma = assemblyName.IndexOf (',');
    if (comma >= 0)
      assemblyName = assemblyName.Substring (0, comma);
    return "res/" + assemblyName + "/" + pageName;
  }
}

}

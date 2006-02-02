using System;
using System.Reflection;
using System.Web.Compilation;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Globalization
{

/// <remarks>
///   <para>
///     net-1.1 rubicon globalization system: 
///     <list type="bullet">
///       <item> The resource files are located in the folder <c>Globalization</c>. </item>
///       <item> BaseName: Page-Namespace.Globalization.NameOfTheResxFileWithoutExtension </item>
///       <item> The <see cref="MultilingualResourcesAttribute"/> is used in web projects. </item>
///     </list>
///   </para>
///   <para>
///     net-2.0 rubicon globalization system: 
///     <list type="bullet">
///       <item> The resource files are located in the folder <c>App_GlobalResources</c>.</item>
///       <item> BaseName: Resources.NameOfTheResxFileWithoutExtension </item>
///       <item> The <see cref="WebMultilingualResourcesAttribute"/> is used in web projects. </item>
///     </list>
///   </para>
///   <para>
///     When a project is upgraded, the Wizzard adds the prefix <c>Globalization.</c> to the name of the resource 
///     files and moves them into the <c>App_GlobalResources</c> folder. This prefix must be removed. The easiest 
///     way to accomlish this, is by moving the resource files into the project's root folder prior to the upgrade.
///   </para>
///   <para>
///     In addition, the namespace must be replaced with the namespace <c>Resources</c> when specified with the 
///     <see cref="WebMultilingualResourcesAttribute"/>. This can be accomplished with a simple Search & Replace.
///   </para>
///   <para>
///     For new projects, it is recommended to specify the <see cref="Type"/> of the resource, instead of its 
///     base name (i.e. <c>typeof (Resources.NameOfTheResxFileWithoutExtension)</c>).
///   </para>
/// </remarks>
public class WebMultiLingualResourcesAttribute : MultiLingualResourcesAttribute
{

  public WebMultiLingualResourcesAttribute (string baseName) : base (baseName)
  {
    Initialize (baseName);
  }

#if ! NET11
  public WebMultiLingualResourcesAttribute (Type resourceType) : base (resourceType.FullName)
  {
    ArgumentUtility.CheckNotNull ("resourceType", resourceType);
    SetResourceAssembly (resourceType.Assembly);
  }
#endif

  public virtual void Initialize (string baseName)
  {
#if ! NET11
    ArgumentUtility.CheckNotNullOrEmpty ("baseName", baseName);
    Type type = BuildManager.GetType (baseName, true);
    SetResourceAssembly (type.Assembly);
#endif
  }

}

}

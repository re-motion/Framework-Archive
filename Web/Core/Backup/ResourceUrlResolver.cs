using System;
using System.Web.UI;
using System.Web;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class ResourceUrlResolver
{
  private const string c_designTimeRootDefault = "C:\\Rubicon.Resources";
  private const string c_designTimeRootEnvironmentVaribaleName = "RUBICONRESOURCES";

  private static bool s_resolverInitialized = false;
  private static IResourceUrlResolver s_resolver = null;
  /// <summary>
  ///   Returns the physical URL of a resource item.
  ///   <seealso cref="IResourceUrlResolver"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If the current ASP.NET application object implements <see cref="IResourceUrlResolver"/>, the application 
  ///     object creates the URL string. 
  ///     Otherwise, the URL /&lt;AppDir&gt;/res/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl 
  ///     is used. (e.g., WebApplication/res/Rubicon.Web/Image/Help.gif).
  ///   </para><para>
  ///     During design time, the root section <c>/&lt;AppDir&gt;/res</c> is mapped to the environment variable
  ///     <c>RUBICONRESOURCES</c>, or if the variable does not exist, <c>C:\Rubicon.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> The current <see cref="Control"/>. Currently, this parameter is ignored. </param>
  /// <param name="context"> The current <see cref="HttpContext"/>. </param>
  /// <param name="definingType"> The type that this resource item is associated with. </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) </param>
  /// <param name="relativeUrl"> The relative URL of the item. </param>
  public static string GetResourceUrl (
      Control control, 
      HttpContext context,
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    if (context != null)
    {
      if (! s_resolverInitialized)
      {
        lock (typeof (ResourceUrlResolver))
        {
          if (! s_resolverInitialized)
          {
            s_resolver = context.ApplicationInstance as IResourceUrlResolver;
            s_resolverInitialized = true;
          }
        }
      }
    }
    if (s_resolver != null)
    {
      return s_resolver.GetResourceUrl (definingType, resourceType, relativeUrl);
    }
    else
    {
      string assemblyName = definingType.Assembly.FullName.Split(',')[0];
      string root;
      if (context == null && Rubicon.Web.Utilities.ControlHelper.IsDesignMode (control))
         root = GetTheDesignTimeRoot() + "/";
      else 
        root = HttpRuntime.AppDomainAppVirtualPath + "/res/";
      return root + assemblyName + "/" + resourceType.Name + "/" + relativeUrl;
    }
  }

  private static string GetTheDesignTimeRoot()
  { 
    string designTimeRoot = System.Environment.GetEnvironmentVariable (c_designTimeRootEnvironmentVaribaleName);
    if (StringUtility.IsNullOrEmpty (designTimeRoot))
      return c_designTimeRootDefault;
    else
      return designTimeRoot;

    //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
    //if(environment != null)
    //{
    //  EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;          
    //  //  project.Properties uses a 1-based index
    //  for (int i = 1; i <= project.Properties.Count; i++)
    //  {
    //    if(project.Properties.Item (i).Name == "ActiveFileSharePath")
    //      return project.Properties.Item (i).Value.ToString();
    //  }
    //}
  }
}

}

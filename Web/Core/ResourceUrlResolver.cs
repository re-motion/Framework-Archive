using System;
using System.Web.UI;
using System.Web;

using Rubicon.Web.UI.Controls;

namespace Rubicon.Web
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class ResourceUrlResolver
{
  private static bool s_resolverInitialized = false;
  private static IResourceUrlResolver s_resolver = null;

  /// <summary>
  ///   Returns the physical URL of a resource item.
  ///   <seealso cref="IResourceUrlResolver"/>.
  /// </summary>
  /// <remarks>
  ///   If the current ASP.NET application object implements <see cref="IResourceUrlResolver"/>, the application object
  ///   creates the URL string. Otherwise, the URL /&lt;AppDir&gt;/res/&ltdefiningType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl 
  ///   is used. (e.g., /rubicon.res/Rubicon.Web/Image/Help.gif)
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

    if (s_resolver != null)
    {
      return s_resolver.GetResourceUrl (definingType, resourceType, relativeUrl);
    }
    else
    {
      string assemblyName = definingType.Assembly.FullName.Split(',')[0];
      return HttpRuntime.AppDomainAppVirtualPath + "/res/" + assemblyName + "/" + resourceType.Name + "/" + relativeUrl;
    }
  }
}

}

using System;
using System.Web.UI;
using System.Web;

using Rubicon.Web.UI.Controls;

namespace Rubicon.Web
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class ResourceUrlResolver
{
  /// <summary>
  ///   Searches the control hierarchy for an implementation of <see cref="IResourceUrlResolver"/>
  ///   and resolves the <paramref name="relativeUrl"/> using 
  ///   <see cref="IResourceUrlResolver.GetResourceUrl"/>.
  /// </summary>
  /// <remarks>
  ///   Walks the control hierarchy upwards until an implementation of 
  ///   <see cref="IResourceUrlResolver"/> is found. If none is found, the 
  ///   <see cref="HttpContext.ApplicationInstance"/> is tested.
  /// </remarks>
  /// <param name="control"> The <see cref="Control"/> where to start the hierarchy walk.</param>
  /// <param name="context"> The <see cref="HttpContext"/> of the control initializing the search. </param>
  /// <param name="definingType"> The type of the control initializing the search. </param>
  /// <param name="resourceType"> An instance of type <see cref="ResourceType"/>. </param>
  /// <param name="relativeUrl">
  ///   The relative URL used by the implementation of <see cref="ResourceUrlResolver"/>
  ///   to build a URL.
  /// </param>
  /// <returns>
  ///   The URL or <see langword="null"/> if no <see cref="ResourceUrlResolver"/> could be found.
  /// </returns>
  public static string GetResourceUrl (
      Control control, 
      HttpContext context,
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    if (control == null)
      return null;

    IResourceUrlResolver resourceUrlResolver = control as IResourceUrlResolver;
    if (resourceUrlResolver == null)
    {
      if (control.Parent != null)
      {
        return ResourceUrlResolver.GetResourceUrl (control.Parent, context, definingType, resourceType, relativeUrl);
      }
      else
      {
        resourceUrlResolver = context.ApplicationInstance as IResourceUrlResolver;
        if (resourceUrlResolver == null)
          return null;
      }
    }

    string resourceUrl = resourceUrlResolver.GetResourceUrl (definingType, resourceType, relativeUrl);
    if (resourceUrl == null)
      return ResourceUrlResolver.GetResourceUrl (control.Parent, context, definingType, resourceType, relativeUrl);

    return resourceUrl;
  }
}

}

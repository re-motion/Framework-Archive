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
  ///   Walks the control hierarchy upwards until an implementation of 
  ///   <see cref="ResourceUrlResolver"/> is found.
  /// </summary>
  /// <param name="control"> The <see cref="Control"/> where to start the hierarchy walk.</param>
  /// <param name="relativeUrl">
  ///   The relative URL used by the implementation of <see cref="ResourceUrlResolver"/> to build a 
  ///   URL.
  /// </param>
  /// <returns>
  ///   The URL or <see langword="null"/> if no <see cref="ResourceUrlResolver"/> could be found.
  /// </returns>
  public static string GetResourceUrl (
      Control control, 
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    if (control == null)
      return null;

    IResourceUrlResolver resourceUrlResolver = control as IResourceUrlResolver;
    
    if (resourceUrlResolver == null)
      return ResourceUrlResolver.GetResourceUrl (control.Parent, definingType, resourceType, relativeUrl);
    
    string imageUrl = resourceUrlResolver.GetResourceUrl (definingType, resourceType, relativeUrl);

    if (imageUrl == null)
      return ResourceUrlResolver.GetResourceUrl (control.Parent, definingType, resourceType, relativeUrl);

    return imageUrl;
  }
}

}

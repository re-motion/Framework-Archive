using System;
using System.Web.UI;
using System.Web;

using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Utilities
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class UrlResolverUtility
{
  /// <summary>
  ///   Walks the control hierarchy upwards until an implementation of 
  ///   <see cref="IImageUrlResolver"/> is found.
  /// </summary>
  /// <param name="control"> The <see cref="Control"/> where to start the hierarchy walk.</param>
  /// <param name="relativeUrl">
  ///   The relative URL used by the implementation of <see cref="IImageUrlResolver"/> to build a 
  ///   URL.
  /// </param>
  /// <returns>
  ///   The URL or <see langword="null"/> if no <see cref="IImageUrlResolver"/> could be found.
  /// </returns>
  public static string GetImageUrl (Control control, string relativeUrl)
  {
    if (control == null)
      return null;

    IImageUrlResolver imageUrlResolver = control as IImageUrlResolver;
    
    if (imageUrlResolver == null)
      return UrlResolverUtility.GetImageUrl (control.Parent, relativeUrl);
    
    string imageUrl = imageUrlResolver.GetImageUrl (relativeUrl);

    if (imageUrl == null)
      return UrlResolverUtility.GetImageUrl (control.Parent, relativeUrl);

    return imageUrl;
  }

  /// <summary>
  ///   Walks the control hierarchy upwards until an implementation of 
  ///   <see cref="IHelpUrlResolver"/> is found.
  /// </summary>
  /// <param name="control"> The <see cref="Control"/> where to start the hierarchy walk.</param>
  /// <param name="relativeUrl">
  ///   The relative URL used by the implementation of <see cref="IHelpUrlResolver"/> to build a 
  ///   URL.
  /// </param>
  /// <returns>
  ///   The URL or <see langword="null"/> if no <see cref="IHelpUrlResolver"/> could be found.
  /// </returns>
  public static string GetHelpUrl (Control control, string relativeUrl)
  {
    if (control == null)
      return null;

    IHelpUrlResolver helpUrlResolver = control as IHelpUrlResolver;
    
    if (helpUrlResolver == null)
      return UrlResolverUtility.GetHelpUrl (control.Parent, relativeUrl);
    
    string helpUrl = helpUrlResolver.GetHelpUrl (relativeUrl);

    if (helpUrl == null)
      return UrlResolverUtility.GetHelpUrl (control.Parent, relativeUrl);

    return helpUrl;
  }
}

}

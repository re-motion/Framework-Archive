using System;
using System.IO;
using System.Web.UI;

namespace Rubicon.Web.Utilities
{
/// <summary>
/// Provided helper functions for working with URLs.
/// </summary>
public class UrlUtility
{
  /// <summary>
  /// Makes a relative URL absolute.
  /// </summary>
  /// <param name="page">The requesting page.</param>
  /// <param name="relativeUrl">The relative URL.</param>
  /// <returns>The absolute URL.</returns>
  public static string GetAbsoluteUrl (Page page, string relativeUrl)
  {
    if (relativeUrl.StartsWith ("http"))
    {
      return relativeUrl;
    }
    else
    {
      string serverPart = page.Request.Url.GetLeftPart (System.UriPartial.Authority);  
      string pathPart = page.ResolveUrl (relativeUrl);

      return serverPart + pathPart;
    }
  }

  /// <summary>
  /// Returns an absolute URL from a relative URL and removes the protocol part (e.g. "http://")
  /// </summary>
  /// <param name="page">The requesting page.</param>
  /// <param name="relativeUrl">The relative URL.</param>
  /// <returns>The absolute URL without the protocol part.</returns>
  public static string GetAbsoluteUrlWithoutProtocol (Page page, string relativeUrl)
  {
    string absoluteUrl = GetAbsoluteUrl (page, relativeUrl);

    absoluteUrl = absoluteUrl.Replace ("https://", string.Empty);
    absoluteUrl = absoluteUrl.Replace ("http://", string.Empty);

    return absoluteUrl;
  }

  public static string GetAbsolutePageUrl (Page page)
  {
    return GetAbsoluteUrl (page, Path.GetFileName (page.Request.Url.AbsolutePath));
  }

  /// <summary>
  /// Combines 2 web URLs. 
  /// </summary>
  /// <param name="path1">Can be a relative or a absolute URL.</param>
  /// <param name="path2">Must be a relative URL or a filename.</param>
  /// <returns>The combined path.</returns>
  public static string Combine (string path1, string path2)
  {
    string path = Path.Combine (path1, path2);
    return path.Replace (@"\", "/");
  }

	private UrlUtility()
	{
	}
}
}

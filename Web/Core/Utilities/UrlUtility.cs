using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;

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

  /// <summary> Makes a relative URL absolute. </summary>
  /// <param name="context"> The <see cref="HttpContext"/> to be used. Must not be <see langword="null"/>. </param>
  /// <param name="relativeUrl"> The relative URL. Must not be <see langword="null"/> or empty. </param>
  /// <returns> The absolute URL. </returns>
  public static string GetAbsoluteUrl (HttpContext context, string relativeUrl)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

    if (relativeUrl.StartsWith ("http"))
    {
      return relativeUrl;
    }
    else
    {
      string serverPart = context.Request.Url.GetLeftPart (System.UriPartial.Authority);  
      string pathPart = context.Response.ApplyAppPathModifier (relativeUrl);

      return serverPart + pathPart;
    }
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

  /// <summary>
  /// Formats a URL string with URL encoding. (The <c>format</c> argument is not encoded.)
  /// </summary>
  public static string FormatUrl (string format, params object[] args)
  {
    if (args == null)
      return format;

    string[] encodedArgs = new string[args.Length];
    System.Text.Encoding encoding = HttpContext.Current.Response.ContentEncoding;
    for (int i = 0; i < args.Length; ++i)
      encodedArgs[i] = HttpUtility.UrlDecode (args.ToString(), encoding);
  
    return string.Format (format, encodedArgs);
  }

  
  /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/*' />
  public static string AddParameter (string url, string name, string value, Encoding encoding)
  {
    ArgumentUtility.CheckNotNull ("url", url);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);
    ArgumentUtility.CheckNotNull ("value", value);
    ArgumentUtility.CheckNotNull ("encoding", encoding);

    string delimiter;
    bool hasQueryString = url.IndexOf ('?') != -1;
    if (hasQueryString)
    {
      char lastChar = url[url.Length - 1];
      if (UrlUtility.IsParameterDelimiter (lastChar))
        delimiter = string.Empty;
      else
        delimiter = "&";
    }
    else
    {
      delimiter = "?";
    }

    value = HttpUtility.UrlEncode (value, encoding);
    url += delimiter + name + "=" + value;

    return url;
  }

  /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/param[@name="url" or @name="name" or @name="value"]' />
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/returns' />
  public static string AddParameter (string url, string name, string value)
  {
    return UrlUtility.AddParameter (url, name, value, HttpContext.Current.Response.ContentEncoding);
  }

  
  /// <summary> 
  ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
  /// </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/*' />
  public static string AddParameters (string url, NameValueCollection queryStringCollection, Encoding encoding)
  {
    ArgumentUtility.CheckNotNull ("queryStringCollection", queryStringCollection);

    for (int i = 0; i < queryStringCollection.Count; i++)
    {
      url = UrlUtility.AddParameter (url, queryStringCollection.GetKey(i), queryStringCollection.Get(i), encoding);
    }
    return url;
  }

  /// <summary> 
  ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
  /// </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/param[@name="url" or @name="queryStringCollection"]' />
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/returns' />
  public static string AddParameters (string url, NameValueCollection queryStringCollection)
  {
    return UrlUtility.AddParameters (url, queryStringCollection, HttpContext.Current.Response.ContentEncoding);
  }

  
  /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/*' />
  public static string FormatQueryString (NameValueCollection queryStringCollection, Encoding encoding)
  {
    return UrlUtility.AddParameters (string.Empty, queryStringCollection, encoding);
  }

  /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/param[@name="queryStringCollection"]' />
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/returns' />
  public static string FormatQueryString (NameValueCollection queryStringCollection)
  {
    return UrlUtility.FormatQueryString (queryStringCollection, HttpContext.Current.Response.ContentEncoding);
  }

  
  /// <summary> Removes a <paramref name="name"/>/value pair from the <paramref name="url"/>. </summary>
  /// <include file='doc\include\Utilities\UrlUtility.xml' path='UrlUtility/DeleteParameter/*' />
  public static string DeleteParameter (string url, string name)
  {
    ArgumentUtility.CheckNotNull ("url", url);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    int indexOfParameter = UrlUtility.GetParameterPosition (url, name);
    
    if (indexOfParameter != -1)
    {
      int indexOfNextDelimiter = url.IndexOf ('&', indexOfParameter + name.Length);
      if (indexOfNextDelimiter == -1)
      {
        int start = indexOfParameter - 1;
        int length = url.Length - start;
        url = url.Remove (start, length);
      }
      else
      {
        int indexOfNextParameter = indexOfNextDelimiter + 1;
        int length = indexOfNextParameter - indexOfParameter;
        url = url.Remove (indexOfParameter, length);
      }
    }
    
    return url;
  }


  public static string GetParameter (string url, string name, Encoding encoding)
  {
    ArgumentUtility.CheckNotNull ("url", url);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);
    ArgumentUtility.CheckNotNull ("encoding", encoding);

    string value = null;

    int indexOfParameter = UrlUtility.GetParameterPosition (url, name);
    if (indexOfParameter != -1)
    {
      int indexOfValueDelimiter = indexOfParameter + name.Length;
      if (indexOfValueDelimiter < url.Length && url[indexOfValueDelimiter] == '=')
      {
        int indexOfValue = indexOfValueDelimiter + 1;
        int length;
        int indexOfNextDelimiter = url.IndexOf ('&', indexOfValue);
        if (indexOfNextDelimiter == -1)
          length = url.Length - indexOfValue;
        else
          length = indexOfNextDelimiter - indexOfValue;

        value = url.Substring (indexOfValue, length);
        value = HttpUtility.UrlDecode (value, encoding);
      }
    }

    return value;
  }

  public static string GetParameter (string url, string name)
  {
    return UrlUtility.GetParameter (url, name, HttpContext.Current.Response.ContentEncoding);
  }

  /// <summary> Gets the index of the <paramref name="parameter"/> in the <paramref name="url"/>. </summary>
  /// <returns> The index of the <paramref name="parameter"/> or -1 if it is not part of the <paramref name="url"/>. </returns>
  private static int GetParameterPosition (string url, string parameter)
  {
    if (url.Length < parameter.Length + 1)
      return -1;

    int indexOfParameter;
    for (indexOfParameter = 1; indexOfParameter < url.Length; indexOfParameter++)
    {
      indexOfParameter = url.IndexOf (parameter, indexOfParameter);
      if (indexOfParameter == -1)
        break;
      if (UrlUtility.IsParameterDelimiter (url[indexOfParameter - 1]))
        break;
    }
    return indexOfParameter;
  }

  private static bool IsParameterDelimiter (char c)
  {
    return c == '?' || c == '&';
  }

	private UrlUtility()
	{
	}
}
}

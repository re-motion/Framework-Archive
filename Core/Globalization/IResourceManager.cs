using System;
using System.Collections.Specialized;

namespace Rubicon.Globalization
{

/// <summary>
///   An interface for defining a string resource manager.
/// </summary>
/// <remarks>
///   Recommended for all applications requiring string resources or automatic labeling
/// </remarks>
public interface IResourceManager
{
  /// <summary>
  ///   Returns all string resources inside the resource manager.
  /// </summary>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  NameValueCollection GetAllStrings ();

  /// <summary>
  ///   Searches for all string resources inside the resource manager whose name is prefixed 
  ///   with a matching tag.
  /// </summary>
  /// <param name="prefix">The prefix all returned string resources must have</param>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  NameValueCollection GetAllStrings (string prefix);

  /// <summary>
  ///   Gets the value of the specified String resource.
  /// </summary>
  /// <param name="id">The ID of the resource to get</param>
  /// <returns>
  ///   The value of the resource. If a match is not possible, a null reference is returned
  /// </returns>
  string GetString (string id);

  /// <summary>
  ///   Gets the root names of the resource files that the <c>IResourceManager</c>
  ///   searches for resources. Multiple roots are seperated by a comma.
  /// </summary>
  string BaseNameList { get; }
}

}

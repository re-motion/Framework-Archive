using System;
using System.Collections.Specialized;

namespace Rubicon.Globalization
{

/// <summary>
///   An interface for defining a string resource manager.
/// </summary>
public interface IResourceManager
{
  /// <summary>
  ///   Returns all string resources inside the resource manager.
  /// </summary>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the value being the string.
  /// </returns>
  NameValueCollection GetAllStrings ();

  /// <summary>
  ///   Searches for all string resources inside the resource manager whose name is prefixed 
  ///   with a matching tag.
  /// </summary>
  /// <param name="prefix"> The prefix all returned string resources must have. </param>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  NameValueCollection GetAllStrings (string prefix);

  /// <summary>
  ///   Gets the value of the specified String resource.
  /// </summary>
  /// <param name="id">The ID of the resource to get. </param>
  /// <returns>
  ///   The value of the resource. If no match is possible, the identifier is returned.
  /// </returns>
  string GetString (string id);

  /// <summary>
  ///   Gets the value of the specified string resource. The resource is identified by
  ///   concatenating type and value name.
  /// </summary>
  /// <remarks> See <see cref=""/> for resource identifier syntax. </remarks>
  /// <returns>
  ///   The value of the resource. If no match is possible, the identifier is returned.
  /// </returns>
  string GetString (Enum enumValue);

  string Name { get; }
}

}

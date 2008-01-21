using System;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{
  /// <summary>
  /// Encapsulates the algorithms used to access resource containers defined by resource attributes.
  /// </summary>
  /// <typeparam name="TAttribute">The type of the resource attribute to be resolved by this class.</typeparam>
  /// <remarks>
  /// This class is an attribute type-agnostic encapsulation of the algorithms used by <see cref="MultiLingualResources"/>.
  /// </remarks>
  public static class ResourceManagerResolverUtility<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    /// <summary>
    ///   Loads a string resource for a given type, identified by ID.
    /// </summary>
    /// <param name="resolver">The resolver to use.</param>
    /// <param name="objectTypeToGetResourceFor">
    ///   The type for which to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    public static string GetResourceText (ResourceManagerResolverImplementation<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      IResourceManager rm = resolver.GetResourceManager (objectTypeToGetResourceFor, false);
      string text = rm.GetString (name);
      if (text == name)
        return String.Empty;
      return text;
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="resolver">The resolver to use.</param>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    public static bool ExistsResourceText (ResourceManagerResolverImplementation<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      try
      {
        IResourceManager rm = resolver.GetResourceManager (objectTypeToGetResourceFor, false);
        string text = rm.GetString (name);
        return (text != name);
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="resolver">The resolver to use.</param>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
    public static bool ExistsResource (ResourceManagerResolverImplementation<TAttribute> resolver, Type objectTypeToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);

      Type definingType;
      TAttribute[] resourceAttributes;

      resolver.FindFirstResourceDefinitions (objectTypeToGetResourceFor, true, out definingType, out resourceAttributes);
      return resourceAttributes.Length > 0;
    }
  }
}
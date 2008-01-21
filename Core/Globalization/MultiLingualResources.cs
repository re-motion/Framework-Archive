using System;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{
  /// <summary>
  /// Provides the public API for classes working with and analyzing instances of <see cref="MultiLingualResourcesAttribute"/>.
  /// </summary>
  public static class MultiLingualResources
  {
    private static readonly ResourceManagerResolver<MultiLingualResourcesAttribute> s_resolver =
        new ResourceManagerResolver<MultiLingualResourcesAttribute>();

    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
    public static IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);
      return s_resolver.GetResourceManager (objectType, includeHierarchy);
    }

    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType"]' />
    public static IResourceManager GetResourceManager (Type objectType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      return GetResourceManager (objectType, false);
    }

    /// <summary>
    ///   Loads a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);
      
      return ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.GetResourceText (s_resolver, objectTypeToGetResourceFor, name);
    }

    /// <summary>
    ///   Loads a string resource for the object's type, identified by ID.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    public static string GetResourceText (object objectToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return GetResourceText (objectToGetResourceFor.GetType(), name);  
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      return ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResourceText (s_resolver, objectTypeToGetResourceFor, name);
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    public static bool ExistsResourceText (object objectToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ExistsResourceText (objectToGetResourceFor.GetType(), name);  
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
    public static bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      return ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (s_resolver, objectTypeToGetResourceFor);
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
    public static bool ExistsResource (object objectToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      return ExistsResource (objectToGetResourceFor.GetType());
    }
  }
}
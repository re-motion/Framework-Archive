using System;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Globalization
{
  /// <summary>
  /// Provides a variant of <see cref="MultiLingualResources"/> that can be used to have mixins add resource identifiers to a target
  /// class. With this class, attributes are not only retrieved from the class and its base classes, but also from its mixins.
  /// </summary>
  public class MixedMultiLingualResources
  {
    private static readonly MixedResourceManagerResolver<MultiLingualResourcesAttribute> s_resolver =
        new MixedResourceManagerResolver<MultiLingualResourcesAttribute> ();

    /// <summary>
    ///   Returns an instance of <see cref="IResourceManager"/> for the resource container specified in the class declaration of the type.
    /// </summary>
    /// <param name="objectType">The type to return an <see cref="IResourceManager"/> for.</param>
    /// <param name="includeHierarchy">If set to true, <see cref="MultiLingualResourcesAttribute"/> applied to base classes and mixins will be
    /// included in the resource manager; otherwise, only the <paramref name="objectType"/> is searched for such attributes.</param>
    /// <returns>An instance of <see cref="IResourceManager"/> for <paramref name="objectType"/>.</returns>
    public static IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      return s_resolver.GetResourceManager (objectType, includeHierarchy);
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource set can be found. </returns>
    public static bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      return ResourceManagerResolverUtility<MultiLingualResourcesAttribute>.ExistsResource (s_resolver, objectTypeToGetResourceFor);
    }
  }
}
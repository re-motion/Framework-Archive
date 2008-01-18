using System;
using System.Collections;
using System.Reflection;
using System.Resources;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{
  /// <summary>
  /// Provides a generalized implementation of the algorithms used to translate resource attributes into <see cref="IResourceManager"/> instances.
  /// </summary>
  /// <typeparam name="TAttribute">The type of the resource attribute to be resolved by this class.</typeparam>
  public class ResourceManagerResolverImplementation<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    private readonly InterlockedCache<string, ResourceManager> _resourceManagersCache = new InterlockedCache<string, ResourceManager> ();
    private readonly InterlockedCache<string, ResourceManagerSet> _resourceManagerWrappersCache = new InterlockedCache<string, ResourceManagerSet>();

    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
    public virtual IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      Type definingType;
      return GetResourceManager (objectType, includeHierarchy, out definingType);
    }

    /// <summary>
    ///   Returns a <c>IResourceManager</c> set for the resource containers specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy" or @name="definingType"]' />
    public virtual IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      TAttribute[] resourceAttributes;
      //  Current hierarchy level, always report missing TAttribute

      Type retrievedDefiningType;
      GetResourceNameAndType (objectType, false, out retrievedDefiningType, out resourceAttributes);
      definingType = retrievedDefiningType;

      string key = retrievedDefiningType.AssemblyQualifiedName + "/" + includeHierarchy;

      //  Look in cache and continue with the cached resource manager wrapper, if one is found
      return _resourceManagerWrappersCache.GetOrCreateValue (
          key,
          delegate
          {
            return CreateResourceManagerSet (retrievedDefiningType, resourceAttributes, includeHierarchy);
          });
    }

    private ResourceManagerSet CreateResourceManagerSet (Type definingType, TAttribute[] resourceAttributes,
        bool includeHierarchy)
    {
      ArrayList resourceManagers = new ArrayList ();
      resourceManagers.AddRange (GetResourceManagers (definingType.Assembly, resourceAttributes));

      if (includeHierarchy)
        WalkHierarchyAndPrependResourceManagers (resourceManagers, definingType);

      //  Create a new resource mananger wrapper set and return it.
      ResourceManager[] resourceManagerArray = (ResourceManager[]) resourceManagers.ToArray (typeof (ResourceManager));
      return ResourceManagerWrapper.CreateWrapperSet (resourceManagerArray);
    }

    protected virtual void WalkHierarchyAndPrependResourceManagers (ArrayList resourceManagers, Type definingType)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);
      ArgumentUtility.CheckNotNull ("definingType", definingType);

      Type currentType = definingType.BaseType;
      while (currentType != null)
      {
        TAttribute[] resourceAttributes;
        GetResourceNameAndType (currentType, true, out currentType, out resourceAttributes);

        //  No more base types defining the TAttribute
        if (currentType != null)
        {
          //  Insert the found resources managers at the beginning of the list
          resourceManagers.InsertRange (0, GetResourceManagers (currentType.Assembly, resourceAttributes));

          currentType = currentType.BaseType;
        }
      }
    }

    /// <summary>
    ///   Returns an <b>ResourceManager</b> array for the resource containers specified through the 
    ///   <paramref name="resourceAttributes"/>.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManagers/*' />
    public ResourceManager[] GetResourceManagers (Assembly assembly, TAttribute[] resourceAttributes)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("resourceAttributes", resourceAttributes);

      ResourceManager[] resourceManagers = new ResourceManager[resourceAttributes.Length];

      //  Load the ResourceManagers for the type's resources

      for (int index = 0; index < resourceAttributes.Length; index++)
      {
        Assembly resourceAssembly = resourceAttributes[index].ResourceAssembly;
        if (resourceAssembly == null)
          resourceAssembly = assembly;
        string key = resourceAttributes[index].BaseName + " in " + resourceAssembly.FullName;

        //  Look in cache 
        resourceManagers[index] = _resourceManagersCache.GetOrCreateValue (
            key,
            delegate
            {
              string baseName = resourceAttributes[index].BaseName;
              return new ResourceManager (baseName, resourceAssembly);
            });
      }

      return resourceManagers;
    }

    /// <summary>
    /// Finds the class where the <typeparamref name="TAttribute"/> was defined and returns
    /// its type and all the <typeparamref name="TAttribute"/> instances.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceNameAndType/*' />
    public void GetResourceNameAndType (
        Type concreteType,
        bool noUndefinedException,
        out Type definingType,
        out TAttribute[] resourceAttributes)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("noUndefinedException", noUndefinedException);

      Type type = concreteType;
      resourceAttributes = GetResourceAttributes (type);

      //  Find the base type where MultiLingualResourceAttribute was specified for this type
      //  Base type can be identical to type

      while (resourceAttributes.Length == 0)
      {
        type = type.BaseType;
        if (type == null)
        {
          if (noUndefinedException)
            break;
          else
            throw new ResourceException ("Type " + concreteType.FullName + " and its base classes do not define the attribute " + typeof (TAttribute).Name + ".");
        }

        resourceAttributes = GetResourceAttributes (type);
      }

      definingType = type;
    }

    /// <summary>
    ///   Returns the <typeparamref name="TAttribute"/> attributes defined for the 
    ///   <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to analyze.</param>
    /// <returns>An array of <typeparamref name="TAttribute"/> attributes.</returns>
    private TAttribute[] GetResourceAttributes (Type type)
    {
      return AttributeUtility.GetCustomAttributes<TAttribute> (type, false);
    }
  }
}
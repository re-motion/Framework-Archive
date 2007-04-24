using System;
using System.ComponentModel;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Design
{
  /// <summary>
  /// Design mode attribute used to associate a design mode specifc mapping loader with it's run-time version.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class DesignModeMappingLoaderAttribute: Attribute
  {
    private readonly Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignModeMappingLoaderAttribute"/> class with a design mode mapping loader
    /// </summary>
    /// <param name="type">
    /// A <see cref="Type"/> implementing <see cref="IMappingLoader"/>. The <paramref name="type"/> must also have a constructor receiving an 
    /// instance of a <see cref="Type"/> implementing <see cref="ISite"/>.
    /// </param>
    public DesignModeMappingLoaderAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (IMappingLoader));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IMappingLoader CreateInstance (ISite site)
    {
      ArgumentUtility.CheckNotNull ("site", site);
      return (IMappingLoader) TypesafeActivator.CreateInstance (_type).With (site);
    }
  }
}
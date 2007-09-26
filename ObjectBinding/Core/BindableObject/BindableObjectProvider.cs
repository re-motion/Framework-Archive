using System;
using System.Collections;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectProvider : BusinessObjectProvider
  {
    private static readonly DoubleCheckedLockingContainer<BindableObjectProvider> s_current =
        new DoubleCheckedLockingContainer<BindableObjectProvider> (CreateBindableObjectProvider);

    public static BindableObjectProvider Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (BindableObjectProvider provider)
    {
      s_current.Value = provider;
    }

    public static BindableObjectProvider CreateDesignModeBindableObjectProvider ()
    {
      return new DesignModeBindableObjectProvider();
    }

    private static BindableObjectProvider CreateBindableObjectProvider ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService (typeof (IBindableObjectGlobalizationService), new BindableObjectGlobalizationService());
      provider.AddService (typeof (IBusinessObjectStringFormatterService), new BusinessObjectStringFormatterService());

      return provider;
    }

    private readonly InterlockedCache<Type, BindableObjectClass> _businessObjectClassCache = new InterlockedCache<Type, BindableObjectClass>();
    private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService>();

    public BindableObjectProvider ()
    {
    }

    /// <summary>
    /// Gets the <see cref="BindableObjectClass"/> for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to get a <see cref="BindableObjectClass"/> for. This type must have a mixin derived from
    /// <see cref="BindableObjectMixinBase{TBindableObject}"/> configured, and it is recommended to specify the simple target type rather then the
    /// generated mixed type.</param>
    /// <returns>A <see cref="BindableObjectClass"/> for the given <paramref name="type"/>.</returns>
    public virtual BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassReflector classReflector = new ClassReflector (type, this, GetMetadataFactoryForType (type));
      return classReflector.GetMetadata();
    }

    private IMetadataFactory GetMetadataFactoryForType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return DefaultMetadataFactory.Instance;
    }

    public InterlockedCache<Type, BindableObjectClass> BusinessObjectClassCache
    {
      get { return _businessObjectClassCache; }
    }

    /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    protected override ICache<Type, IBusinessObjectService> ServiceCache
    {
      get { return _serviceCache; }
    }
  }
}
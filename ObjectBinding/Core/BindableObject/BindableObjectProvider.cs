using System;
using System.Collections;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectProvider : BusinessObjectProvider
  {
    private static readonly BindableObjectProvider s_instance = new BindableObjectProvider();

    public static BindableObjectProvider Instance
    {
      get { return s_instance; }
    }

    private readonly InterlockedCache<Type, BindableObjectClass> _businessObjectClassCache = new InterlockedCache<Type, BindableObjectClass>();
    private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService> ();

    public BindableObjectProvider ()
    {
    }

    public BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassReflector classReflector = new ClassReflector (type, this);
      return classReflector.GetMetadata ();
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
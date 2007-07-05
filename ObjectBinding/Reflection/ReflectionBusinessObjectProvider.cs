using System;
using System.Collections;
using Rubicon.Collections;
using Rubicon.ObjectBinding.Web;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProvider: BusinessObjectProvider
{
  private readonly static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private ReflectionBusinessObjectProvider()
  {
    _serviceCache.Add (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService ());
  }

  private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService> ();

  protected override ICache<Type, IBusinessObjectService> ServiceCache
  {
    get { return _serviceCache; }
  }

}

}

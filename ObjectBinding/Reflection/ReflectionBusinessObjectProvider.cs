using System;
using System.Collections;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProvider: BusinessObjectProvider
{
  public static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private ReflectionBusinessObjectProvider()
  {
    _serviceDictionary.Add (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());
  }

  private Hashtable _serviceDictionary = new Hashtable();

  protected override IDictionary ServiceDictionary
  {
    get { return _serviceDictionary; }
  }

}

}

using System;
using System.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProvider: BusinessObjectProvider
{
  public static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private Hashtable _serviceDictionary = new Hashtable();

  protected override IDictionary ServiceDictionary
  {
    get { return _serviceDictionary; }
  }

}

}

using System;
using System.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{

public class DomainObjectProvider: BusinessObjectProvider
{
  public static DomainObjectProvider s_instance = new DomainObjectProvider();

  public static DomainObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private DomainObjectProvider()
  {
  }

  private Hashtable _serviceDictionary = new Hashtable();

  protected override IDictionary ServiceDictionary
  {
    get { return _serviceDictionary; }
  }

}

}

using System;
using System.Collections;

using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class QueryProvider : BusinessObjectProvider
{
  private static QueryProvider s_instance = new QueryProvider ();
  
  public static QueryProvider Instance
  {
    get { return s_instance; }
  }

	private QueryProvider()
	{
	}

  private Hashtable _serviceDictionary = new Hashtable();

  protected override IDictionary ServiceDictionary
  {
    get { return _serviceDictionary; }
  }
}
}

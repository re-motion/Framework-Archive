using System;
using System.Collections;

using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO: Why have a different SearchObjectProvider? Remove this class?
public class SearchObjectProvider : BusinessObjectProvider
{
  private static SearchObjectProvider s_instance = new SearchObjectProvider ();
  
  public static SearchObjectProvider Instance
  {
    get { return s_instance; }
  }

	private SearchObjectProvider()
	{
	}

  private Hashtable _serviceDictionary = new Hashtable();

  protected override IDictionary ServiceDictionary
  {
    get { return _serviceDictionary; }
  }
}
}

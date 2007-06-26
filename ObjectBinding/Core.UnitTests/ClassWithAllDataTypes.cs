using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubicon.ObjectBinding.UnitTests
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private string _string;
    private string[] _stringArray;
    private List<string> _listOfString = new List<string>();
    private ArrayList _stringArrayList = new ArrayList();

    public ClassWithAllDataTypes ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    public string[] StringArray
    {
      get { return _stringArray; }
      set { _stringArray = value; }
    }

    public List<string> ListOfString
    {
      get { return _listOfString; }
    }

    [ItemType(typeof (string))]
    public ArrayList StringArrayList
    {
      get { return _stringArrayList; }
    }
  }
}
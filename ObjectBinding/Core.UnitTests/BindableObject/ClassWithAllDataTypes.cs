using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private string _string;
    private UnboundClass _object;
    private UnboundClass[] _objectArray;
    private List<UnboundClass> _listOfObject = new List<UnboundClass> ();
    private ArrayList _objectArrayList = new ArrayList ();

    public ClassWithAllDataTypes ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    public UnboundClass Object
    {
      get { return _object; }
      set { _object = value; }
    }

    public UnboundClass[] ObjectArray
    {
      get { return _objectArray; }
      set { _objectArray = value; }
    }

    public List<UnboundClass> ListOfObject
    {
      get { return _listOfObject; }
    }

    [ItemType (typeof (UnboundClass))]
    public ArrayList ObjectArrayList
    {
      get { return _objectArrayList; }
    }
  }
}
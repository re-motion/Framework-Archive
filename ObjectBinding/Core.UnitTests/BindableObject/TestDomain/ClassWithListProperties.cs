using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithListProperties
  {
    private SimpleReferenceType[] _array;
    private List<SimpleReferenceType> _listOfT = new List<SimpleReferenceType> ();
    private ArrayList _arrayList = new ArrayList ();

    public ClassWithListProperties ()
    {
    }

    public SimpleReferenceType[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public List<SimpleReferenceType> ListOfT
    {
      get { return _listOfT; }
    }

    [ItemType (typeof (SimpleReferenceType))]
    public ArrayList ArrayList
    {
      get { return _arrayList; }
    }
  }
}
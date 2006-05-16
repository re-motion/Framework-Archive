using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class EnumValueInfo
  {
    // types

    // static members

    // member fields

    private string _id;
    private int _value;
    private string _name;

    // construction and disposing

    public EnumValueInfo (int value, string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _value = value;
      _name = name;
    }

    // methods and properties

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public int Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("Name", value);
        _name = value;
      }
    }
  }
}
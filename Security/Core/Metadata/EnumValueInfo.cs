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

    private int _id;
    private string _name;

    // construction and disposing

    public EnumValueInfo (int id, string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _id = id;
      _name = name;
    }

    // methods and properties

    public int ID
    {
      get { return _id; }
      set { _id = value; }
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
using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Security.Metadata
{

  public class StatePropertyInfo
  {
    // types

    // static members

    // member fields

    private string _id;	
    private string _name;
    private List<EnumValueInfo> _values = new List<EnumValueInfo>();
	
    // construction and disposing

    public StatePropertyInfo ()
    {
    }

    // methods and properties

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }
   
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public List<EnumValueInfo> Values
    {
      get { return _values; }
      set { _values = value; }
    }
  }

}
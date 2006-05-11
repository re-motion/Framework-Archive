using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class SecurableClassInfo
  {
    // types

    // static members

    // member fields

    private Guid _id;
    private string _name;
    private List<StatePropertyInfo> _properties;
    private SecurableClassInfo _baseClass;
    private List<SecurableClassInfo> _derivedClasses = new List<SecurableClassInfo> ();

    // construction and disposing

    public SecurableClassInfo ()
    {
    }

    // methods and properties

    public Guid ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public List<StatePropertyInfo> Properties
    {
      get { return _properties; }
      set { _properties = value; }
    }

    public SecurableClassInfo BaseClass
    {
      get { return _baseClass; }
      set { _baseClass = value; }
    }

    public List<SecurableClassInfo> DerivedClasses
    {
      get { return _derivedClasses; }
    }
	
  }
}
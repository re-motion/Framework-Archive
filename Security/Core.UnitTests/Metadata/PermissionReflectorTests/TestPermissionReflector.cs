using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Security.Metadata;
using Rubicon.Collections;
using System.Reflection;

namespace Rubicon.Security.UnitTests.Metadata.PermissionReflectorTests
{
  public class TestPermissionReflector : PermissionReflector
  {
    // types

    // static members

    public new static Cache<Tupel<Type, Type, string, BindingFlags>, Enum[]> Cache
    {
      get { return PermissionReflector.Cache; }
    }

    // member fields

    // construction and disposing

    public TestPermissionReflector ()
    {
    }

    // methods and properties
  }
}
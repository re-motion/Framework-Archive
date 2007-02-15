using System;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata.PermissionReflectorTests
{
  public class TestPermissionReflector : PermissionReflector
  {
    // types

    // static members

    public new static Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]> Cache
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
using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.Domain;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class TypeReflectorTest
  {
    // types

    // static members

    // member fields

    private TypeReflector _typeReflector;

    // construction and disposing

    public TypeReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _typeReflector = new TypeReflector ();
    }

    //[Test]
    //[ExpectedException (typeof (ArgumentException),
    //    "Type 'Rubicon.Security.UnitTests.Domain.SimpleType does not implement interface 'Rubicon.Security.ISecurableType'.\r\nArgumentName: type")]
    //public void GetMetadataWithInvalidType ()
    //{
    //  _typeReflector.GetMetadata (typeof (SimpleType));
    //}
  }
}
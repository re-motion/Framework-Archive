// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class AttributeUsageTest
  {
    private class ImplicitUsageAttribute : Attribute
    {
    }

    [AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
    private class NotMultipleAttribute : Attribute
    {
    }

    [AttributeUsage (AttributeTargets.Method, Inherited = false)]
    private class NotInheritedAttribute : Attribute
    {
    }

    [AttributeUsage (AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    private class InheritedAttribute : Attribute
    {
    }

    [AttributeUsage (AttributeTargets.Assembly, Inherited = true, AllowMultiple = true)]
    private class MultipleAttribute : Attribute
    {
    }

    [Test]
    public void GetAttributeUsage ()
    {
      var attribute = AttributeUtility.GetAttributeUsage (typeof (MultipleAttribute));
      Assert.That (attribute, Is.EqualTo (typeof (MultipleAttribute).GetCustomAttributes (typeof (AttributeUsageAttribute), true)[0]));
    }

    [Test]
    public void GetAttributeUsage_ParameterIsNull_ThrowsArgumentNullException ()
    {
      Assert.That (
          () => AttributeUtility.GetAttributeUsage (null),
          Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo ("Value cannot be null.\r\nParameter name: attributeType"));
    }

    [Test]
    public void GetAttributeUsage_NeverNull ()
    {
      var attribute = AttributeUtility.GetAttributeUsage (typeof (ImplicitUsageAttribute));
      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute, Is.EqualTo (new AttributeUsageAttribute (AttributeTargets.All)));
    }

    [Test]
    public void GetAttributeUsage_ReturnsCopy ()
    {
      var multipleAttribute1 = AttributeUtility.GetAttributeUsage (typeof (MultipleAttribute));
      var multipleAttribute2 = AttributeUtility.GetAttributeUsage (typeof (MultipleAttribute));
      Assert.That (multipleAttribute2, Is.Not.SameAs (multipleAttribute1));
      Assert.That (multipleAttribute1.AllowMultiple, Is.True);
      Assert.That (multipleAttribute1.ValidOn, Is.EqualTo (AttributeTargets.Assembly));

      var notInheritedAttribute1 = AttributeUtility.GetAttributeUsage (typeof (NotInheritedAttribute));
      var notInheritedAttribute2 = AttributeUtility.GetAttributeUsage (typeof (NotInheritedAttribute));
      Assert.That (notInheritedAttribute2, Is.Not.SameAs (notInheritedAttribute1));
      Assert.That (notInheritedAttribute1.Inherited, Is.False);
      Assert.That (notInheritedAttribute1.ValidOn, Is.EqualTo (AttributeTargets.Method));
    }

    [Test]
    public void GetAttributeUsage_WithNoAttribute ()
    {
      var attribute = AttributeUtility.GetAttributeUsage (typeof (object));
      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute, Is.EqualTo (new AttributeUsageAttribute (AttributeTargets.All)));
    }

    [Test]
    public void AllowMultipleTrue ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (MultipleAttribute)), Is.True);
    }

    [Test]
    public void AllowMultipleFalse ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (NotMultipleAttribute)), Is.False);
    }

    [Test]
    public void DefaultAllowMultiple ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (ImplicitUsageAttribute)), Is.False);
    }

    [Test]
    public void InheritedTrue ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited (typeof (InheritedAttribute)), Is.True);
    }

    [Test]
    public void InheritedFalse ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited (typeof (NotInheritedAttribute)), Is.False);
    }

    [Test]
    public void DefaultInherited ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited (typeof (ImplicitUsageAttribute)), Is.True);
    }
  }
}
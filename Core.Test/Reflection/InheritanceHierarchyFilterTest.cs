using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Reflection;

namespace Rubicon.Core.UnitTests.Reflection
{
  [TestFixture]
  public class InheritanceHierarchyFilterTest
  {
    private class Base1Class
    {
    }

    private class Derived11Class : Base1Class
    {
    }

    private class Leaf111Class : Derived11Class
    {
    }

    private class Leaf112Class : Derived11Class
    {
    }

    private class Base2Class
    {
    }

    private class Derived21Class : Base2Class
    {
    }

    private class Derived22Class : Base2Class
    {
    }

    private class Leaf211Class : Derived21Class
    {
    }

    private class Leaf221Class : Derived22Class
    {
    }

    [Test]
    public void name ()
    {
      Type[] types = typeof (InheritanceHierarchyFilterTest).GetNestedTypes (BindingFlags.NonPublic);
      InheritanceHierarchyFilter typeFilter = new InheritanceHierarchyFilter (types);

      Assert.That (
          typeFilter.GetLeafTypes(),
          Is.EqualTo (new Type[] {typeof (Leaf111Class), typeof (Leaf112Class), typeof (Leaf211Class), typeof (Leaf221Class)}));
    }
  }
}
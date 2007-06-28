using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class TypeReflectorTest
  {
    [Test]
    public void GetProperties ()
    {
      Type type = typeof (SimpleClass);
      TypeReflector typeReflector = new TypeReflector (type);
      
      PropertyBase[] properties = typeReflector.GetProperties();

      Assert.That (properties[0].Identifier, Is.EqualTo ("String"));
    }
  }
}
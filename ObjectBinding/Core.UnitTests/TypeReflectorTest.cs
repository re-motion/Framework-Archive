using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests
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
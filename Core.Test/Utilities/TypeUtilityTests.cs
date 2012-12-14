using System;
using Rubicon.Utilities;
using NUnit.Framework;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class TypeUtilityTests
{
  [Test]
  public void TestParseAbbreviatedTypeName()
  {
    string standardName = TypeUtility.ParseAbbreviatedTypeName ("Rubicon.Core.UnitTests::Utilities.TypeUtilityTests");
    Assert.AreEqual ("Rubicon.Core.UnitTests.Utilities.TypeUtilityTests, Rubicon.Core.UnitTests", standardName);
  }

  [Test]
  public void TestGetType()
  {
    TypeUtility.GetType ("Rubicon.Core.UnitTests::Utilities.TypeUtilityTests", true);
  }
}

}

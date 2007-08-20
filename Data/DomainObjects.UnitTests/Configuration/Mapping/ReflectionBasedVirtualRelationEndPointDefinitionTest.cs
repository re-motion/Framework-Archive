using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedVirtualRelationEndPointDefinitionTest : StandardMappingTest
  {
    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition employeeClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Employee));
      ReflectionBasedVirtualRelationEndPointDefinition relationEndPointDefinition =
          (ReflectionBasedVirtualRelationEndPointDefinition) employeeClassDefinition.GetRelationEndPointDefinition (typeof (Employee) + ".Computer");
      Assert.AreEqual (typeof (Employee).GetProperty ("Computer"), relationEndPointDefinition.PropertyInfo);
    }
  }
}
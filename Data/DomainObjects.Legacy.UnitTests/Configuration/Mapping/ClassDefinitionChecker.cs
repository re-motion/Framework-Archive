using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  public class ClassDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ClassDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (XmlBasedClassDefinition expectedDefinition, XmlBasedClassDefinition actualDefinition)
    {
      Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, "IDs of class definitions do not match");

      Assert.AreEqual (expectedDefinition.ClassType, actualDefinition.ClassType,
          "ClassType of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (expectedDefinition.ClassTypeName, actualDefinition.ClassTypeName,
          "ClassTypeName of class definition '{0}' does not match. ",
          expectedDefinition.ID);

      Assert.AreEqual (expectedDefinition.IsClassTypeResolved, actualDefinition.IsClassTypeResolved,
          "IsClassTypeResolved of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (expectedDefinition.IsAbstract, actualDefinition.IsAbstract,
          "IsAbstract of class definition '{0}' does not match. ",
          expectedDefinition.ID);

      Assert.AreEqual (expectedDefinition.StorageProviderID, actualDefinition.StorageProviderID,
          "StorageProviderID of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (expectedDefinition.MyEntityName, actualDefinition.MyEntityName,
          "EntityName of class definition '{0}' does not match.",
          expectedDefinition.ID);

      if (expectedDefinition.BaseClass == null)
      {
        Assert.IsNull (actualDefinition.BaseClass, "actualDefinition.BaseClass");
      }
      else
      {
        Assert.IsNotNull (actualDefinition.BaseClass, "actualDefinition.BaseClass");

        Assert.AreEqual (expectedDefinition.BaseClass.ID, actualDefinition.BaseClass.ID,
            string.Format ("BaseClass of class definition '{0}' does not match. Expected: {1}, actual: {2}",
            expectedDefinition.ID, expectedDefinition.BaseClass.ID, actualDefinition.BaseClass.ID));
      }

      CheckDerivedClasses (expectedDefinition.DerivedClasses, actualDefinition.DerivedClasses, expectedDefinition);
      CheckPropertyDefinitions (expectedDefinition.MyPropertyDefinitions, actualDefinition.MyPropertyDefinitions, expectedDefinition);
    }

    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions)
    {
      Check (expectedDefinitions, actualDefinitions, true);
    }

    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions, bool checkRelations)
    {
      Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, "Number of class definitions does not match.");

      Assert.AreEqual (expectedDefinitions.AreResolvedTypesRequired, actualDefinitions.AreResolvedTypesRequired,
          "AreResolvedTypesRequired does not match.");

      foreach (XmlBasedClassDefinition expectedDefinition in expectedDefinitions)
      {
        ClassDefinition actualDefinition = actualDefinitions[expectedDefinition.ClassType];
        Assert.That (actualDefinition, Is.InstanceOfType (typeof (XmlBasedClassDefinition)));
        Check (expectedDefinition, (XmlBasedClassDefinition) actualDefinition);

        if (checkRelations)
        {
          RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
          relationDefinitionChecker.Check (expectedDefinition.MyRelationDefinitions, actualDefinition.MyRelationDefinitions);
        }
      }
    }

    private void CheckDerivedClasses (
        ClassDefinitionCollection expectedDerivedClasses,
        ClassDefinitionCollection actualDerivedClasses,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (expectedDerivedClasses.Count, actualDerivedClasses.Count,
          "Number of derived classes of class definition '{0}' does not match",
          expectedClassDefinition.ID);

      Assert.AreEqual (expectedDerivedClasses.AreResolvedTypesRequired, actualDerivedClasses.AreResolvedTypesRequired,
          "AreResolvedTypeNamesRequired of DerivedClasses collection of class definition '{0}' does not match",
          expectedClassDefinition.ID);

      foreach (ClassDefinition expectedDerivedClass in expectedDerivedClasses)
      {
        Assert.IsNotNull (
            actualDerivedClasses[expectedDerivedClass.ID],
            string.Format ("Actual class definition '{0}' does not contain expected derived class '{1}'.",
                expectedClassDefinition.ID, expectedDerivedClass.ID));
      }
    }

    private void CheckPropertyDefinitions (
        PropertyDefinitionCollection expectedDefinitions,
        PropertyDefinitionCollection actualDefinitions,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count,
          string.Format ("Number of property definitions in class definition '{0}' does not match. Expected: {1}, actual: {2}",
          expectedClassDefinition.ID, expectedDefinitions.Count, actualDefinitions.Count));

      foreach (XmlBasedPropertyDefinition expectedDefinition in expectedDefinitions)
      {
        XmlBasedPropertyDefinition actualDefinition = (XmlBasedPropertyDefinition) actualDefinitions[expectedDefinition.PropertyName];
        CheckPropertyDefinition (expectedDefinition, actualDefinition, expectedClassDefinition);
      }
    }

    private void CheckPropertyDefinition (
        XmlBasedPropertyDefinition expectedDefinition,
        XmlBasedPropertyDefinition actualDefinition,
        ClassDefinition classDefinition)
    {
      Assert.AreEqual (expectedDefinition.PropertyName, actualDefinition.PropertyName,
          string.Format ("PropertyNames of property definitions (class definition: '{0}') do not match.",
          classDefinition.ID));

      Assert.AreEqual (expectedDefinition.ClassDefinition.ID, actualDefinition.ClassDefinition.ID,
          string.Format ("ClassDefinitionID of property definition '{0}' does not match.",
          expectedDefinition.PropertyName));

      Assert.AreEqual (expectedDefinition.StorageSpecificName, actualDefinition.StorageSpecificName,
          string.Format ("StorageSpecificName of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));

      Assert.AreEqual (expectedDefinition.MaxLength, actualDefinition.MaxLength,
          string.Format ("MaxLength of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));

      Assert.AreEqual (expectedDefinition.PropertyType, actualDefinition.PropertyType,
          string.Format ("PropertyType of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));

      Assert.AreEqual (expectedDefinition.IsNullable, actualDefinition.IsNullable,
          string.Format ("IsNullable of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));

      Assert.AreEqual (expectedDefinition.MappingTypeName, actualDefinition.MappingTypeName,
          string.Format ("MappingTypeName of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));

      Assert.AreEqual (expectedDefinition.IsObjectID, actualDefinition.IsObjectID,
          string.Format ("IsObjectID of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName, classDefinition.ID));
    }
  }
}

using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class ClassDefinitionChecker
  {
    public ClassDefinitionChecker()
    {
    }

    public void Check (ClassDefinition expectedDefinition, ClassDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull ("actualDefinition", actualDefinition);

      Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, "IDs of class definitions do not match.");

      Assert.AreEqual (
          expectedDefinition.ClassType,
          actualDefinition.ClassType,
          "ClassType of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.ClassTypeName,
          actualDefinition.ClassTypeName,
          "ClassTypeName of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsClassTypeResolved,
          actualDefinition.IsClassTypeResolved,
          "IsClassTypeResolved of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.IsAbstract,
          actualDefinition.IsAbstract,
          "IsAbstract of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.StorageProviderID,
          actualDefinition.StorageProviderID,
          "StorageProviderID of class definition '{0}' does not match. ",
          expectedDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.MyEntityName,
          actualDefinition.MyEntityName,
          "EntityName of class definition '{0}' does not match.",
          expectedDefinition.ID);

      if (expectedDefinition.BaseClass == null)
      {
        Assert.IsNull (actualDefinition.BaseClass, "actualDefinition.BaseClass is not null.");
      }
      else
      {
        Assert.IsNotNull (actualDefinition.BaseClass, "actualDefinition.BaseClass is null.");

        Assert.AreEqual (
            expectedDefinition.BaseClass.ID,
            actualDefinition.BaseClass.ID,
            "BaseClass of class definition '{0}' does not match.",
            expectedDefinition.ID);
      }

      CheckDerivedClasses (expectedDefinition.DerivedClasses, actualDefinition.DerivedClasses, expectedDefinition);
      CheckPropertyDefinitions (expectedDefinition.MyPropertyDefinitions, actualDefinition.MyPropertyDefinitions, expectedDefinition);
    }


    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions)
    {
      Check (expectedDefinitions, actualDefinitions, true, false);
    }

    public void Check (ClassDefinitionCollection expectedDefinitions, ClassDefinitionCollection actualDefinitions, bool checkRelations)
    {
      Check (expectedDefinitions, actualDefinitions, checkRelations, false);
    }

    public void Check (
        ClassDefinitionCollection expectedDefinitions,
        ClassDefinitionCollection actualDefinitions,
        bool checkRelations,
        bool ignoreUnknown)
    {
      ArgumentUtility.CheckNotNull ("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull ("actualDefinitions", actualDefinitions);

      if (!ignoreUnknown)
        Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, "Number of class definitions does not match.");

      Assert.AreEqual (
          expectedDefinitions.AreResolvedTypesRequired,
          actualDefinitions.AreResolvedTypesRequired,
          "AreResolvedTypesRequired does not match.");

      foreach (ClassDefinition expectedDefinition in expectedDefinitions)
      {
        ClassDefinition actualDefinition = actualDefinitions[expectedDefinition.ClassType];
        Check (expectedDefinition, actualDefinition);

        if (checkRelations)
        {
          RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
          relationDefinitionChecker.Check (expectedDefinition.MyRelationDefinitions, actualDefinition.MyRelationDefinitions, ignoreUnknown);
        }
      }
    }

    private void CheckDerivedClasses (
        ClassDefinitionCollection expectedDerivedClasses,
        ClassDefinitionCollection actualDerivedClasses,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (
          expectedDerivedClasses.Count,
          actualDerivedClasses.Count,
          string.Format (
              "Number of derived classes of class definition '{0}' does not match.",
              expectedClassDefinition.ID));

      foreach (ClassDefinition expectedDerivedClass in expectedDerivedClasses)
      {
        Assert.IsNotNull (
            actualDerivedClasses[expectedDerivedClass.ID],
            "Actual class definition '{0}' does not contain expected derived class '{1}'.",
            expectedClassDefinition.ID,
            expectedDerivedClass.ID);
      }
    }

    private void CheckPropertyDefinitions (
        PropertyDefinitionCollection expectedDefinitions,
        PropertyDefinitionCollection actualDefinitions,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual (
          expectedDefinitions.Count,
          actualDefinitions.Count,
          "Number of property definitions in class definition '{0}' does not match.",
          expectedClassDefinition.ID);

      foreach (PropertyDefinition expectedDefinition in expectedDefinitions)
      {
        PropertyDefinition actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.IsNotNull (actualDefinition, "Class '{0}' has no property '{1}'.", expectedClassDefinition.ID, expectedDefinition.PropertyName);
        CheckPropertyDefinition (expectedDefinition, actualDefinition, expectedClassDefinition);
      }
    }

    private void CheckPropertyDefinition (
        PropertyDefinition expectedDefinition,
        PropertyDefinition actualDefinition,
        ClassDefinition classDefinition)
    {
      Assert.AreEqual (
          expectedDefinition.PropertyName,
          actualDefinition.PropertyName,
          "PropertyNames of property definitions (class definition: '{0}') do not match.",
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.ClassDefinition.ID,
          actualDefinition.ClassDefinition.ID,
          "ClassDefinitionID of property definition '{0}' does not match.",
          expectedDefinition.PropertyName);

      Assert.AreEqual (
          expectedDefinition.StorageSpecificName,
          actualDefinition.StorageSpecificName,
          "ColumnName of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.MaxLength,
          actualDefinition.MaxLength,
          "MaxLength of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.PropertyType,
          actualDefinition.PropertyType,
          "PropertyType of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);

      Assert.AreEqual (
          expectedDefinition.MappingTypeName,
          actualDefinition.MappingTypeName,
          "PropertyTypeName of property definition '{0}' (class definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          classDefinition.ID);
    }
  }
}
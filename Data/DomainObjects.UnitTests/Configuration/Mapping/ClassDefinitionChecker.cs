using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
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

  public void Check (
      ClassDefinitionCollection expectedDefinitions, 
      ClassDefinitionCollection actualDefinitions)
  {
    Check (expectedDefinitions, actualDefinitions, true);
  }

  public void Check (
      ClassDefinitionCollection expectedDefinitions, 
      ClassDefinitionCollection actualDefinitions,
      bool checkRelations)
  {
    Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, 
        string.Format ("Number of class definitions does not match. Expected: {0}, actual: {1}", 
        expectedDefinitions.Count, actualDefinitions.Count));

    foreach (ClassDefinition expectedDefinition in expectedDefinitions)
    {
      ClassDefinition actualDefinition = actualDefinitions[expectedDefinition.ClassType];
      CheckClassDefinition (expectedDefinition, actualDefinition);
      
      if (checkRelations)
      {
        RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
        relationDefinitionChecker.Check (expectedDefinition.MyRelationDefinitions, actualDefinition.MyRelationDefinitions);
      }
    }
  }

  private void CheckClassDefinition (
      ClassDefinition expectedDefinition, 
      ClassDefinition actualDefinition)
  {
    Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, 
        string.Format ("IDs of class definitions do not match. Expected: {0}, actual: {1}", 
        expectedDefinition.ID, actualDefinition.ID));

    Assert.AreEqual (expectedDefinition.ClassType, actualDefinition.ClassType, 
        string.Format ("ClassType of class definition '{0}' does not match. Expected: {1}, actual: {2}", 
        expectedDefinition.ID, expectedDefinition.ClassType, actualDefinition.ClassType));

    Assert.AreEqual (expectedDefinition.StorageProviderID, actualDefinition.StorageProviderID, 
        string.Format ("StorageProviderID of class definition '{0}' does not match. Expected: {1}, actual: {2}", 
        expectedDefinition.ID, expectedDefinition.StorageProviderID, actualDefinition.StorageProviderID));

    Assert.AreEqual (expectedDefinition.EntityName, actualDefinition.EntityName, 
        string.Format ("EntityName of class definition '{0}' does not match. Expected: {1}, actual: {2}", 
        expectedDefinition.ID, expectedDefinition.EntityName, actualDefinition.EntityName));

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

  private void CheckDerivedClasses (
      ClassDefinitionCollection expectedDerivedClasses, 
      ClassDefinitionCollection actualDerivedClasses, 
      ClassDefinition expectedClassDefinition)
  {
    Assert.AreEqual (expectedDerivedClasses.Count, actualDerivedClasses.Count, 
        string.Format ("Number of derived classes of class definition '{0}' does not match. Expected: {1}, actual: {2}", 
        expectedClassDefinition.ID, expectedDerivedClasses.Count, actualDerivedClasses.Count));

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

    foreach (PropertyDefinition expectedDefinition in expectedDefinitions)
    {
      PropertyDefinition actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
      CheckPropertyDefinition (expectedDefinition, actualDefinition, expectedClassDefinition);
    }
  }

  private void CheckPropertyDefinition (
      PropertyDefinition expectedDefinition, 
      PropertyDefinition actualDefinition,
      ClassDefinition classDefinition)
  {
    Assert.AreEqual (expectedDefinition.PropertyName, actualDefinition.PropertyName, 
        string.Format ("PropertyNames of property definitions (class definition: '{0}') do not match. Expected: {1}, actual: {2}", 
        classDefinition, 
        expectedDefinition.PropertyName, actualDefinition.PropertyName));

    Assert.AreEqual (expectedDefinition.ColumnName, actualDefinition.ColumnName, 
        string.Format ("ColumnName of property definition '{0}' (class definition: '{1}') does not match. Expected: {2}, actual: {3}", 
        expectedDefinition.PropertyName, classDefinition, 
        expectedDefinition.ColumnName, actualDefinition.ColumnName));

    Assert.AreEqual (expectedDefinition.MaxLength, actualDefinition.MaxLength, 
        string.Format ("MaxLength of property definition '{0}' (class definition: '{1}') does not match. Expected: {2}, actual: {3}", 
        expectedDefinition.PropertyName, classDefinition, 
        expectedDefinition.MaxLength, actualDefinition.MaxLength));

    Assert.AreEqual (expectedDefinition.PropertyType, actualDefinition.PropertyType, 
        string.Format ("PropertyType of property definition '{0}' (class definition: '{1}') does not match. Expected: {2}, actual: {3}", 
        expectedDefinition.PropertyName, classDefinition, 
        expectedDefinition.PropertyType, actualDefinition.PropertyType));

    Assert.AreEqual (expectedDefinition.MappingType, actualDefinition.MappingType, 
        string.Format ("MappingType of property definition '{0}' (class definition: '{1}') does not match. Expected: {2}, actual: {3}", 
        expectedDefinition.PropertyName, classDefinition, 
        expectedDefinition.MappingType, actualDefinition.MappingType));
  }      
}
}

using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class RelationDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public RelationDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (
      RelationDefinitionCollection expectedDefinitions, 
      RelationDefinitionCollection actualDefinitions)
    {
      Assert.AreEqual (expectedDefinitions.Count, actualDefinitions.Count, 
        string.Format ("Number of relation definitions does not match. Expected: {0}, actual: {1}", 
        expectedDefinitions.Count, actualDefinitions.Count));

      foreach (RelationDefinition expectedDefinition in expectedDefinitions)
      {
        RelationDefinition actualDefinition = actualDefinitions[expectedDefinition.ID];
        CheckRelationDefinition (expectedDefinition, actualDefinition);
      }
    }

    private void CheckRelationDefinition (
      RelationDefinition expectedDefinition, 
      RelationDefinition actualDefinition)
    {
      Assert.AreEqual (expectedDefinition.ID, actualDefinition.ID, 
        string.Format ("IDs of relation definitions do not match. Expected: {0}, actual: {1}", 
        expectedDefinition.ID, actualDefinition.ID));

      CheckEndPointDefinitions (expectedDefinition, actualDefinition);
    }

    private void CheckEndPointDefinitions (
      RelationDefinition expectedRelationDefinition, 
      RelationDefinition actualRelationDefinition)
    {
      foreach (IRelationEndPointDefinition expectedEndPointDefinition in expectedRelationDefinition.EndPointDefinitions)
      {
        IRelationEndPointDefinition actualEndPointDefinition = actualRelationDefinition.GetEndPointDefinition (
          expectedEndPointDefinition.ClassDefinition.ID, expectedEndPointDefinition.PropertyName); 
      
        CheckEndPointDefinition (expectedEndPointDefinition, actualEndPointDefinition, expectedRelationDefinition);
      }
    }

    private void CheckEndPointDefinition (
      IRelationEndPointDefinition expectedEndPointDefinition, 
      IRelationEndPointDefinition actualEndPointDefinition, 
      RelationDefinition relationDefinition)
    {
      Assert.AreEqual (expectedEndPointDefinition.GetType (), actualEndPointDefinition.GetType (), 
        string.Format ("End point definitions (relation definition: '{0}', property name: '{1}')"
        + " are not of same type. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.GetType (), 
        actualEndPointDefinition.GetType ()));

      Assert.AreEqual (expectedEndPointDefinition.ClassDefinition.ID, actualEndPointDefinition.ClassDefinition.ID, 
        string.Format ("ClassDefinition of end point defintions (relation definition: '{0}', property name: '{1}')"
        + " does not match. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.ClassDefinition.ID, 
        actualEndPointDefinition.ClassDefinition.ID));
    
      Assert.AreEqual (expectedEndPointDefinition.PropertyName, actualEndPointDefinition.PropertyName, 
        string.Format ("PropertyName of end point defintions (relation definition: '{0}', property name: '{1}')"
        + " does not match. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.PropertyName, 
        actualEndPointDefinition.PropertyName));

      Assert.AreEqual (expectedEndPointDefinition.PropertyType, actualEndPointDefinition.PropertyType, 
        string.Format ("PropertyType of end point defintions (relation definition: '{0}', property name: '{1}')"
        + " does not match. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.PropertyType, 
        actualEndPointDefinition.PropertyType));

      Assert.AreEqual (expectedEndPointDefinition.IsMandatory, actualEndPointDefinition.IsMandatory, 
        string.Format ("IsMandatory of end point defintions (relation definition: '{0}', property name: '{1}')"
        + " does not match. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.IsMandatory, 
        actualEndPointDefinition.IsMandatory));

      Assert.AreEqual (expectedEndPointDefinition.Cardinality, actualEndPointDefinition.Cardinality, 
        string.Format ("Cardinality of end point defintions (relation definition: '{0}', property name: '{1}')"
        + " does not match. Expected: {2}, actual: {3}", 
        relationDefinition.ID,  
        expectedEndPointDefinition.PropertyName,
        expectedEndPointDefinition.Cardinality, 
        actualEndPointDefinition.Cardinality));


      if (expectedEndPointDefinition is VirtualRelationEndPointDefinition)
      {
        VirtualRelationEndPointDefinition expectedVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) expectedEndPointDefinition;
        VirtualRelationEndPointDefinition actualVirtualEndPointDefinition = (VirtualRelationEndPointDefinition) actualEndPointDefinition;

        Assert.AreEqual (expectedVirtualEndPointDefinition.SortExpression, actualVirtualEndPointDefinition.SortExpression, 
          string.Format ("SortExpression of end point defintions (relation definition: '{0}', property name: '{1}')"
          + " does not match. Expected: {2}, actual: {3}", 
          relationDefinition.ID,  
          expectedVirtualEndPointDefinition.PropertyName,
          expectedVirtualEndPointDefinition.SortExpression, 
          actualVirtualEndPointDefinition.SortExpression));
      }
    }      
  }
}

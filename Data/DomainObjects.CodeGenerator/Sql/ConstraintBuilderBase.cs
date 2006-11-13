using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Collections;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public abstract class ConstraintBuilderBase
  {
    // types

    // static members and constants

    // member fields

    private StringBuilder _createConstraintStringBuilder;
    private List<string> _entityNamesForDropConstraintScript;
    private Hashtable _constraintNamesUsed;

    // construction and disposing

    public ConstraintBuilderBase ()
    {
      _createConstraintStringBuilder = new StringBuilder ();
      _constraintNamesUsed = new Hashtable ();
      _entityNamesForDropConstraintScript = new List<string> ();
    }

    // methods and properties

    public abstract void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder);
    public abstract void AddToCreateConstraintScript (ClassDefinition classDefinition, StringBuilder createConstraintStringBuilder);
    public abstract string GetConstraint (IRelationEndPointDefinition relationEndPoint, PropertyDefinition propertyDefinition, ClassDefinition oppositeClassDefinition);    
    protected abstract string ConstraintSeparator { get;}

    public string GetAddConstraintScript ()
    {
      return _createConstraintStringBuilder.ToString ();
    }

    public string GetDropConstraintScript ()
    {
      if (_entityNamesForDropConstraintScript.Count == 0)
        return string.Empty;

      StringBuilder dropConstraintStringBuilder = new StringBuilder ();
      AddToDropConstraintScript (_entityNamesForDropConstraintScript, dropConstraintStringBuilder);
      return dropConstraintStringBuilder.ToString ();
    }

    public void AddConstraints (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddConstraint (currentClass);
    }

    public void AddConstraint (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (TableBuilderBase.IsConcreteTable (classDefinition))
      {
        AddToCreateConstraintScript (classDefinition);
        _entityNamesForDropConstraintScript.Add (classDefinition.MyEntityName);
      }
    }

    private void AddToCreateConstraintScript (ClassDefinition classDefinition)
    {
      if (_createConstraintStringBuilder.Length != 0)
        _createConstraintStringBuilder.Append ("\n");
      int length = _createConstraintStringBuilder.Length;

      AddToCreateConstraintScript (classDefinition, _createConstraintStringBuilder);

      if (_createConstraintStringBuilder.Length == length && length > 0)
        _createConstraintStringBuilder.Remove (length - 1, 1);
    }

    protected List<IRelationEndPointDefinition> GetAllRelationEndPoints (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      List<IRelationEndPointDefinition> allRelationEndPointDefinitions = new List<IRelationEndPointDefinition> ();
      if (classDefinition.BaseClass != null)
        allRelationEndPointDefinitions.AddRange (classDefinition.BaseClass.GetRelationEndPointDefinitions ());

      FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (classDefinition, allRelationEndPointDefinitions);

      return allRelationEndPointDefinitions;
    }

    protected string GetConstraints (ClassDefinition tableRootClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableRootClassDefinition", tableRootClassDefinition);

      StringBuilder constraintBuilder = new StringBuilder ();
      foreach (IRelationEndPointDefinition relationEndPoint in GetAllRelationEndPoints (tableRootClassDefinition))
      {
        string constraint = GetConstraint (relationEndPoint);

        if (constraint.Length != 0)
        {
          if (constraintBuilder.Length != 0)
            constraintBuilder.Append (ConstraintSeparator);

          constraintBuilder.Append (constraint);
        }
      }
      return constraintBuilder.ToString ();
    }

    private string GetConstraint (IRelationEndPointDefinition relationEndPoint)
    {
      if (relationEndPoint.IsNull)
        return string.Empty;

      ClassDefinition oppositeClassDefinition = relationEndPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (relationEndPoint.PropertyName);

      if (!HasConstraint (relationEndPoint, oppositeClassDefinition))
        return string.Empty;

      PropertyDefinition propertyDefinition = relationEndPoint.ClassDefinition.GetMandatoryPropertyDefinition (relationEndPoint.PropertyName);

      return GetConstraint (relationEndPoint, propertyDefinition, oppositeClassDefinition);
    }

    private void FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (ClassDefinition classDefinition, List<IRelationEndPointDefinition> allRelationEndPointDefinitions)
    {
      foreach (RelationDefinition relationDefinition in classDefinition.MyRelationDefinitions)
      {
        foreach (IRelationEndPointDefinition relationEndPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (relationEndPointDefinition.ClassDefinition == classDefinition)
            allRelationEndPointDefinitions.Add (relationEndPointDefinition);
        }
      }

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (derivedClass, allRelationEndPointDefinitions);
    }

    private bool HasConstraint (IRelationEndPointDefinition relationEndPoint, ClassDefinition oppositeClassDefinition)
    {
      if (relationEndPoint.IsVirtual)
        return false;

      if (oppositeClassDefinition.StorageProviderID != relationEndPoint.ClassDefinition.StorageProviderID)
        return false;

      if (oppositeClassDefinition.GetEntityName () == null)
        return false;

      return true;
    }
  }
}

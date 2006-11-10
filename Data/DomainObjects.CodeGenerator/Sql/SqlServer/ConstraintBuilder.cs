using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Collections;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer
{
  public class ConstraintBuilder
  {
    // types

    // static members and constants

    // member fields

    private StringBuilder _scriptBuilder;
    private List<string> _entityNamesForDropConstraintScript;
    private Hashtable _constraintNamesUsed;

    // construction and disposing

    public ConstraintBuilder ()
    {
      _scriptBuilder = new StringBuilder ();
      _constraintNamesUsed = new Hashtable ();
      _entityNamesForDropConstraintScript = new List<string> ();
    }

    // methods and properties

    public string GetAddConstraintScript ()
    {
      return _scriptBuilder.ToString ();
    }

    public string GetDropConstraintScript ()
    {
      if (_entityNamesForDropConstraintScript.Count == 0)
        return string.Empty;
      
      return string.Format ("DECLARE @statement nvarchar (4000)\n"
          + "SET @statement = ''\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [{0}].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('{1}')\n"
          + "    ORDER BY t.name, fk.name\n"
          + "exec sp_executesql @statement\n",
          SqlFileBuilder.DefaultSchema,
          string.Join ("', '", _entityNamesForDropConstraintScript.ToArray ()));
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

      if (TableBuilder.IsConcreteTable (classDefinition))
      {
        _entityNamesForDropConstraintScript.Add (classDefinition.MyEntityName);

        string constraints = GetConstraints (classDefinition);
        if (constraints.Length != 0)
        {
          if (_scriptBuilder.Length != 0)
            _scriptBuilder.Append ("\n");

          _scriptBuilder.AppendFormat ("ALTER TABLE [{0}].[{1}] ADD\n{2}\n",
              SqlFileBuilder.DefaultSchema,
              classDefinition.MyEntityName,
              constraints);
        }
      }
    }

    private string GetConstraints (ClassDefinition tableRootClassDefinition)
    {
      StringBuilder constraintBuilder = new StringBuilder ();
      foreach (IRelationEndPointDefinition relationEndPoint in GetAllRelationEndPoints (tableRootClassDefinition))
      {
        string constraint = GetConstraint (relationEndPoint);

        if (constraint.Length != 0)
        {
          if (constraintBuilder.Length != 0)
            constraintBuilder.Append (",\n");

          constraintBuilder.Append (constraint);
        }
      }
      return constraintBuilder.ToString ();
    }

    private string GetConstraint (IRelationEndPointDefinition relationEndPoint)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

      if (relationEndPoint.IsNull)
        return string.Empty;

      ClassDefinition oppositeClassDefinition = relationEndPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (relationEndPoint.PropertyName);

      if (!HasConstraint (relationEndPoint, oppositeClassDefinition))
        return string.Empty;

      PropertyDefinition propertyDefinition = relationEndPoint.ClassDefinition.GetMandatoryPropertyDefinition (relationEndPoint.PropertyName);

      return string.Format ("  CONSTRAINT [FK_{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}].[{3}] ([ID])",
          GetUniqueConstraintName (relationEndPoint),
          propertyDefinition.ColumnName,
          SqlFileBuilder.DefaultSchema,
          oppositeClassDefinition.GetEntityName ());
    }

    private string GetUniqueConstraintName (IRelationEndPointDefinition relationEndPoint)
    {
      int i = 1;

      string constraintName = relationEndPoint.RelationDefinition.ID;
      while (_constraintNamesUsed.ContainsKey (constraintName))
      {
        constraintName = string.Format ("{0}_{1}", relationEndPoint.RelationDefinition.ID, i);
        i++;
      }

      _constraintNamesUsed.Add (constraintName, constraintName);
      return constraintName;
    }

    private List<IRelationEndPointDefinition> GetAllRelationEndPoints (ClassDefinition classDefinition)
    {
      List<IRelationEndPointDefinition> allRelationEndPointDefinitions = new List<IRelationEndPointDefinition> ();
      if (classDefinition.BaseClass != null)
        allRelationEndPointDefinitions.AddRange (classDefinition.BaseClass.GetRelationEndPointDefinitions ());

      FillAllRelationEndPointDefinitionsWithParticularAndDerivedClass (classDefinition, allRelationEndPointDefinitions);

      return allRelationEndPointDefinitions;
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

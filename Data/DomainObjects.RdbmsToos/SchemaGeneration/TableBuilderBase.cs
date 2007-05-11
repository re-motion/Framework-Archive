using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration
{
  public abstract class TableBuilderBase
  {
    // types

    // static members and constants

    public static bool IsConcreteTable (ClassDefinition classDefinition)
    {
      return classDefinition.MyEntityName != null && (classDefinition.BaseClass == null || classDefinition.BaseClass.GetEntityName() == null);
    }

    public static bool HasClassIDColumn (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      RelationDefinition relationDefinition = propertyDefinition.ClassDefinition.GetRelationDefinition (propertyDefinition.PropertyName);
      if (relationDefinition != null)
      {
        IRelationEndPointDefinition oppositeEndPointDefinition = relationDefinition.GetOppositeEndPointDefinition (
            propertyDefinition.ClassDefinition.ID, propertyDefinition.PropertyName);

        if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy
            && propertyDefinition.ClassDefinition.StorageProviderID == oppositeEndPointDefinition.ClassDefinition.StorageProviderID)
        {
          return true;
        }
      }
      return false;
    }

    // member fields

    private StringBuilder _createTableStringBuilder;
    private StringBuilder _dropTableStringBuilder;
    private Dictionary<string, string> _sqlTypeMapping;

    // construction and disposing

    public TableBuilderBase ()
    {
      _createTableStringBuilder = new StringBuilder();
      _dropTableStringBuilder = new StringBuilder();
    }

    // methods and properties

    public abstract void AddToCreateTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder dropTableStringBuilder);
    public abstract string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable);
    protected abstract string ColumnListOfParticularClassFormatString { get; }
    protected abstract string SqlDataTypeObjectID { get; }
    protected abstract string SqlDataTypeSerializedObjectID { get; }
    protected abstract string SqlDataTypeClassID { get; }

    public virtual string GetSqlDataType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.IsObjectID)
      {
        ClassDefinition oppositeClass = propertyDefinition.ClassDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);
        if (oppositeClass.StorageProviderID == propertyDefinition.ClassDefinition.StorageProviderID)
          return SqlDataTypeObjectID;
        else
          return SqlDataTypeSerializedObjectID;
      }

      throw new InvalidOperationException (
          string.Format (
              "Data type '{0}' is not supported.\r\n  Class: {1}, property: {2}",
              propertyDefinition.PropertyType,
              propertyDefinition.ClassDefinition.ID,
              propertyDefinition.PropertyName));
    }

    public string GetCreateTableScript ()
    {
      return _createTableStringBuilder.ToString();
    }

    public string GetDropTableScript ()
    {
      return _dropTableStringBuilder.ToString();
    }

    public void AddTables (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddTable (currentClass);
    }

    public void AddTable (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (IsConcreteTable (classDefinition))
      {
        AddToCreateTableScript (classDefinition);
        AddToDropTableScript (classDefinition);
      }
    }

    private void AddToCreateTableScript (ClassDefinition classDefinition)
    {
      if (_createTableStringBuilder.Length != 0)
        _createTableStringBuilder.Append ("\r\n");

      AddToCreateTableScript (classDefinition, _createTableStringBuilder);
    }

    private void AddToDropTableScript (ClassDefinition classDefinition)
    {
      if (_dropTableStringBuilder.Length != 0)
        _dropTableStringBuilder.Append ("\r\n");

      AddToDropTableScript (classDefinition, _dropTableStringBuilder);
    }

    protected string GetColumnList (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      string columnList = string.Empty;
      ClassDefinition currentClassDefinition = classDefinition;
      while (currentClassDefinition != null)
      {
        columnList = GetColumnListOfParticularClass (currentClassDefinition, false) + columnList;

        currentClassDefinition = currentClassDefinition.BaseClass;
      }

      StringBuilder columnListStringBuilder = new StringBuilder();
      AppendColumnListOfDerivedClasses (classDefinition, columnListStringBuilder);
      columnList += columnListStringBuilder.ToString();
      return columnList;
    }

    private void AppendColumnListOfDerivedClasses (ClassDefinition classDefinition, StringBuilder columnListStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("columnListStringBuilder", columnListStringBuilder);

      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        columnListStringBuilder.Append (GetColumnListOfParticularClass (derivedClassDefinition, true));
        AppendColumnListOfDerivedClasses (derivedClassDefinition, columnListStringBuilder);
      }
    }

    private string GetColumnListOfParticularClass (ClassDefinition classDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      StringBuilder columnListStringBuilder = new StringBuilder();

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        columnListStringBuilder.Append (GetColumn (propertyDefinition, forceNullable));

      return string.Format (ColumnListOfParticularClassFormatString, classDefinition.ID, columnListStringBuilder);
    }
  }
}
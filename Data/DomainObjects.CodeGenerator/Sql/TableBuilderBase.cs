using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public abstract class TableBuilderBase
  {
    // types

    // static members and constants

    public static bool IsConcreteTable (ClassDefinition classDefinition)
    {
      return classDefinition.MyEntityName != null && (classDefinition.BaseClass == null || classDefinition.BaseClass.GetEntityName () == null);
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
      _createTableStringBuilder = new StringBuilder ();
      _dropTableStringBuilder = new StringBuilder ();
    }

    // methods and properties

    public abstract void AddToCreateTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder dropTableStringBuilder);
    public abstract string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable);
    protected abstract string ColumnListOfParticularClassFormatString { get; }

    protected abstract string SqlDataTypeBoolean { get;}
    protected abstract string SqlDataTypeByte { get;}
    protected abstract string SqlDataTypeDate { get;}
    protected abstract string SqlDataTypeDateTime { get;}
    protected abstract string SqlDataTypeDecimal { get;}
    protected abstract string SqlDataTypeDouble { get;}
    protected abstract string SqlDataTypeGuid { get;}
    protected abstract string SqlDataTypeInt16 { get;}
    protected abstract string SqlDataTypeInt32 { get;}
    protected abstract string SqlDataTypeInt64 { get;}
    protected abstract string SqlDataTypeSingle { get;}
    protected abstract string SqlDataTypeString { get;}
    protected abstract string SqlDataTypeStringWithoutMaxLength { get;}
    protected abstract string SqlDataTypeBinary { get;}
    protected abstract string SqlDataTypeObjectID { get;}
    protected abstract string SqlDataTypeSerializedObjectID { get;}
    protected abstract string SqlDataTypeClassID { get;}

    public string GetSqlDataType (PropertyDefinition propertyDefinition)
    {
      EnsureSqlDataTypeMappingPopulated ();

      if (!_sqlTypeMapping.ContainsKey (propertyDefinition.MappingTypeName))
      {
        // must be an enum type
        return GetSqlDataType ("int32");
      }

      if (propertyDefinition.MappingTypeName == TypeInfo.ObjectIDMappingTypeName)
      {
        ClassDefinition oppositeClass = propertyDefinition.ClassDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);
        if (oppositeClass.StorageProviderID != propertyDefinition.ClassDefinition.StorageProviderID)
          return GetSqlDataType ("SerializedObjectID");
      }

      if (propertyDefinition.MappingTypeName == "string")
      {
        if (propertyDefinition.MaxLength.IsNull)
          return GetSqlDataType ("stringWithoutMaxLength");
        else
          return GetSqlDataType (propertyDefinition.MappingTypeName) + " (" + propertyDefinition.MaxLength.ToString () + ")";
      }

      return GetSqlDataType (propertyDefinition.MappingTypeName);
    }

    private string GetSqlDataType (string mappingTypeName)
    {
      string sqlDataType = _sqlTypeMapping[mappingTypeName];
      if (string.IsNullOrEmpty (sqlDataType))
        throw new ArgumentException (string.Format ("The data type '{0}' cannot be mapped to a SQL data type.", mappingTypeName));

      return sqlDataType;
    }
    
    private void EnsureSqlDataTypeMappingPopulated ()
    {
      if (_sqlTypeMapping == null)
      {
        _sqlTypeMapping = new Dictionary<string, string> ();
        _sqlTypeMapping.Add ("boolean", SqlDataTypeBoolean);
        _sqlTypeMapping.Add ("byte", SqlDataTypeByte);
        _sqlTypeMapping.Add ("date", SqlDataTypeDate);
        _sqlTypeMapping.Add ("dateTime", SqlDataTypeDateTime);
        _sqlTypeMapping.Add ("decimal", SqlDataTypeDecimal);
        _sqlTypeMapping.Add ("double", SqlDataTypeDouble);
        _sqlTypeMapping.Add ("guid", SqlDataTypeGuid);
        _sqlTypeMapping.Add ("int16", SqlDataTypeInt16);
        _sqlTypeMapping.Add ("int32", SqlDataTypeInt32);
        _sqlTypeMapping.Add ("int64", SqlDataTypeInt64);
        _sqlTypeMapping.Add ("single", SqlDataTypeSingle);
        _sqlTypeMapping.Add ("string", SqlDataTypeString);
        _sqlTypeMapping.Add ("stringWithoutMaxLength", SqlDataTypeStringWithoutMaxLength);
        _sqlTypeMapping.Add ("binary", SqlDataTypeBinary);
        _sqlTypeMapping.Add (TypeInfo.ObjectIDMappingTypeName, SqlDataTypeObjectID);
        _sqlTypeMapping.Add ("SerializedObjectID", SqlDataTypeSerializedObjectID);
        _sqlTypeMapping.Add ("ClassID", SqlDataTypeClassID);
      }
    }

    public string GetCreateTableScript ()
    {
      return _createTableStringBuilder.ToString ();
    }

    public string GetDropTableScript ()
    {
      return _dropTableStringBuilder.ToString ();
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
        _createTableStringBuilder.Append ("\n");

      AddToCreateTableScript (classDefinition, _createTableStringBuilder);
    }

    private void AddToDropTableScript (ClassDefinition classDefinition)
    {
      if (_dropTableStringBuilder.Length != 0)
        _dropTableStringBuilder.Append ("\n");

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

      StringBuilder columnListStringBuilder = new StringBuilder ();
      AppendColumnListOfDerivedClasses (classDefinition, columnListStringBuilder);
      columnList += columnListStringBuilder.ToString ();
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

      StringBuilder columnListStringBuilder = new StringBuilder ();

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        columnListStringBuilder.Append (GetColumn (propertyDefinition, forceNullable));

      return string.Format (ColumnListOfParticularClassFormatString, classDefinition.ID, columnListStringBuilder);
    }
  }
}

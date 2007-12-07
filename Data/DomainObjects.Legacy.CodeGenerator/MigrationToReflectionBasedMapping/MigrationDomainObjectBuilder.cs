using System;
using System.Collections.Generic;
using System.IO;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.MigrationToReflectionBasedMapping
{
  public class MigrationDomainObjectBuilder : DomainObjectBuilder
  {
    private static readonly string s_instantiableAttribute =
        "  [Instantiable]\r\n";

    private static readonly string s_defaultStorageGroupAttribute =
        "  [DBStorageGroup]\r\n";

    private static readonly string s_tableAttribute =
        "  [DBTable]\r\n";

    private static readonly string s_tableAttributeWithTableName =
        "  [DBTable (\"%tableName%\")]\r\n";

    private static readonly string s_getObjectContent =
        "      return DomainObject.GetObject<%classname%> (%parameterlist%);\r\n";

    private static readonly string s_getObjectContentForTransactionScope =
        "        return %classname%.GetObject (%parameterlist%);\r\n";

    private static readonly string s_newObjectContent =
        "      return DomainObject.NewObject<%classname%> ().With (%parameterlist%);\r\n";

    private static readonly string s_newObjectContentForTransactionScope =
        "        return %classname%.NewObject (%parameterlist%);\r\n";

    private static readonly string s_transactionScopeHeader =
        "      using (%parameterlist%.EnterNonDiscardingScope ())\r\n"
        + "      {\r\n";

    private static readonly string s_transactionScopeFooter =
        "      }\r\n";

    private static readonly string s_stringPropertyAttribute =
        "    [StringProperty (%parameterlist%)]\r\n";

    private static readonly string s_binaryPropertyAttribute =
        "    [BinaryProperty (%parameterlist%)]\r\n";

    private static readonly string s_bidirectionalRelationAttribute =
        "    [DBBidirectionalRelation (%parameterlist%)]\r\n";

    private static readonly string s_mandatoryAttribute =
        "    [Mandatory]\r\n";

    private static readonly string s_columnNameAttribute =
        "    [DBColumn (%parameterlist%)]\r\n";

    private string s_objectListTypeName = "ObjectList<%classname%>";

    private static readonly string s_abstractPropertyGetStatement = "get; ";
    private static readonly string s_abstractPropertySetStatements = "set; ";

    private static readonly string s_nullableAttributeIsNullableParameterWithFalse = "IsNullable = false";
    private static readonly string s_lengthConstrainedAttributeMaximumLengthParameter = "MaximumLength = %parameterlist%";

    private string s_bidirectionalRelationAttributeContainsForeignKeyParameterWithTrue = ", ContainsForeignKey = true";
    private string s_bidirectionalRelationAttributeSortExpressionParameter = ", SortExpression = ";

    private static readonly string s_entityNameTag = "%tableName%";

    private static readonly string s_newObjectParametersWithTransaction = "ClientTransaction clientTransaction";
    private static readonly string s_newObjectTransactionScopeParameters = "clientTransaction";

    private static readonly string s_getObjectParameters = "ObjectID id";
    private static readonly string s_getObjectParametersForContent = "id";
    private static readonly string s_getObjectParametersWithTransaction = "ObjectID id, ClientTransaction clientTransaction";
    private static readonly string s_getObjectParametersWithTransactionForContent = "id";
    private static readonly string s_getObjectTransactionScopeParameters = "clientTransaction";

    private static Dictionary<Type, string> s_typeToCSharpType;

    static MigrationDomainObjectBuilder ()
    {
      s_typeToCSharpType = new Dictionary<Type, string>();
      s_typeToCSharpType.Add (typeof (NaBoolean), "bool?");
      s_typeToCSharpType.Add (typeof (NaByte), "byte?");
      s_typeToCSharpType.Add (typeof (NaDateTime), "DateTime?");
      s_typeToCSharpType.Add (typeof (NaDecimal), "decimal?");
      s_typeToCSharpType.Add (typeof (NaDouble), "double?");
      s_typeToCSharpType.Add (typeof (NaGuid), "Guid?");
      s_typeToCSharpType.Add (typeof (NaSingle), "float?");
      s_typeToCSharpType.Add (typeof (NaInt32), "int?");
      s_typeToCSharpType.Add (typeof (NaInt64), "long?");
      s_typeToCSharpType.Add (typeof (NaInt16), "short?");
    }

    public MigrationDomainObjectBuilder (MappingConfiguration mappingConfiguration, TextWriter writer)
        : base (mappingConfiguration, writer)
    {
    }

    protected override void WriteAttributes (
        XmlBasedClassDefinition classDefinition, TypeName typeName, bool multiLingualResourcesAttribute, bool serializableAttribute)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("typeName", typeName);

      base.WriteAttributes (classDefinition, typeName, multiLingualResourcesAttribute, serializableAttribute);

      if (IsInstantiable (classDefinition))
        WriteInstantiableAttribute();

      if (IsPersistenceRoot (classDefinition))
        WriteStorageGroupAttribute (classDefinition);

      if (IsTable (classDefinition))
        WriteTableAttribute (classDefinition);
    }

    protected override bool IsClassAbstract (XmlBasedClassDefinition classDefinition)
    {
      return true;
    }

    protected override void WriteStaticMembers (XmlBasedClassDefinition classDefinition, TypeName typeName)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("typeName", typeName);

      if (IsInstantiable (classDefinition))
      {
        WriteNewObject (classDefinition, typeName.Name, null, null, null);
        WriteNewObject (classDefinition, typeName.Name, s_newObjectParametersWithTransaction, null, s_newObjectTransactionScopeParameters);
      }

      WriteGetObject (typeName.Name, s_getObjectParameters, s_getObjectParametersForContent, null);
      WriteGetObject (
          typeName.Name, s_getObjectParametersWithTransaction, s_getObjectParametersWithTransactionForContent, s_getObjectTransactionScopeParameters);
    }

    protected override void WriteConstructors (XmlBasedClassDefinition classDefinition, TypeName typeName)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("typeName", typeName);

      WriteConstructor (typeName.Name, null, null);
    }

    protected override void WriteValueProperty (XmlBasedPropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (HasStringPropertyAttribute (propertyDefinition))
        WriteStringPropertyAttribute (propertyDefinition);
      if (HasBinaryPropertyAttribute (propertyDefinition))
        WriteBinaryPropertyAttribute (propertyDefinition);
      if (HasColumnNameAttribute (propertyDefinition))
        WriteColumnNameAttribute (propertyDefinition);

      //TODO ES: do not qualify with the name of the declaring type if it is declared in this type
      WriteAbstractProperty (
          propertyDefinition.PropertyName,
          GetCSharpTypeName (propertyDefinition),
          s_abstractPropertyGetStatement,
          s_abstractPropertySetStatements);
    }

    protected override void WriteRelationProperty (
        XmlBasedClassDefinition classDefinition, TypeName typeName, IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      if (HasBidirectionalRelationAttribute (endPointDefinition))
        WriteBidirectionalRelationAttribute (endPointDefinition);
      if (IsMandatory (endPointDefinition))
        WriteMandatoryAttribute();
      if (HasColumnNameAttribute (endPointDefinition))
        WriteColumnNameAttribute (endPointDefinition);

      if (endPointDefinition.Cardinality == CardinalityType.One)
      {
        WriteAbstractProperty (
            endPointDefinition.PropertyName,
            GetPropertyTypeForRelationPropertyWithCardinalityOne (classDefinition, typeName, endPointDefinition),
            s_abstractPropertyGetStatement,
            s_abstractPropertySetStatements);
      }
      else
      {
        WriteAbstractProperty (
            endPointDefinition.PropertyName,
            GetPropertyTypeForRelationPropertyWithCardinalityMany (classDefinition, typeName, endPointDefinition),
            s_abstractPropertyGetStatement,
            null);
      }
    }

    protected override string GetCSharpTypeName (TypeInfo typeInfo)
    {
      ArgumentUtility.CheckNotNull ("typeInfo", typeInfo);

      string cSharpTypeName;
      if (s_typeToCSharpType.TryGetValue (typeInfo.Type, out cSharpTypeName))
        return cSharpTypeName;

      return base.GetCSharpTypeName (typeInfo);
    }

    private bool IsInstantiable (XmlBasedClassDefinition classDefinition)
    {
      if (classDefinition.GetEntityName() == null)
        return false;

      return true;
    }

    private void WriteInstantiableAttribute ()
    {
      Write (s_instantiableAttribute);
    }

    private bool IsPersistenceRoot (XmlBasedClassDefinition classDefinition)
    {
      if (classDefinition.BaseClass == null)
        return true;

      return false;
    }

    private void WriteStorageGroupAttribute (XmlBasedClassDefinition classDefinition)
    {
      Write (s_defaultStorageGroupAttribute);
    }

    private bool IsTable (XmlBasedClassDefinition classDefinition)
    {
      if (classDefinition.MyEntityName == null)
        return false;

      if (classDefinition.BaseClass == null)
        return true;

      if (classDefinition.BaseClass.GetEntityName() == null)
        return true;

      if (classDefinition.MyEntityName.Equals (classDefinition.BaseClass.GetEntityName(), StringComparison.Ordinal))
        return false;

      return true;
    }

    private void WriteTableAttribute (XmlBasedClassDefinition classDefinition)
    {
      if (classDefinition.ID.Equals (classDefinition.MyEntityName, StringComparison.Ordinal))
        Write (s_tableAttribute);
      else
        Write (ReplaceTag (s_tableAttributeWithTableName, s_entityNameTag, classDefinition.MyEntityName));
    }

    private void BeginTransactionScope (string parameterList)
    {
      string output = s_transactionScopeHeader;
      output = ReplaceTag (output, s_parameterlistTag, parameterList);

      Write (output);
    }

    private void EndTransactionScope ()
    {
      Write (s_transactionScopeFooter);
    }

    private void WriteNewObject (
        XmlBasedClassDefinition classDefinition,
        string className,
        string parameterList,
        string parametersListForNewObjectInvocation,
        string transactionScopeParameterList)
    {
      bool hasInstantiableBaseClass = (classDefinition.BaseClass != null) ? IsInstantiable (classDefinition.BaseClass) : false;
      BeginMethod (
          s_accessibilityPublic,
          s_modifierStatic + (hasInstantiableBaseClass ? s_modifierNew : string.Empty),
          className,
          "NewObject",
          parameterList);

      bool hasTransactionScope = !string.IsNullOrEmpty (transactionScopeParameterList);
      if (hasTransactionScope)
        BeginTransactionScope (transactionScopeParameterList);

      string output = hasTransactionScope ? s_newObjectContentForTransactionScope : s_newObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, parametersListForNewObjectInvocation);
      Write (output);

      if (hasTransactionScope)
        EndTransactionScope();

      EndMethod();
    }

    private void WriteGetObject (
        string className,
        string parameterList,
        string parametersListForGetObjectInvocation,
        string transactionScopeParameterList)
    {
      BeginMethod (s_accessibilityPublic, s_modifierStatic + s_modifierNew, className, "GetObject", parameterList);

      bool hasTransactionScope = !string.IsNullOrEmpty (transactionScopeParameterList);
      if (hasTransactionScope)
        BeginTransactionScope (transactionScopeParameterList);

      string output = hasTransactionScope ? s_getObjectContentForTransactionScope : s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, parametersListForGetObjectInvocation);
      Write (output);

      if (hasTransactionScope)
        EndTransactionScope();

      EndMethod();
    }

    private void WriteConstructor (string className, string parameterList, string parameterListForBaseConstructorCall)
    {
      BeginConstructor (s_accessibilityProtected, className, parameterList, parameterListForBaseConstructorCall);
      EndConstructor();
    }

    private void WriteAbstractProperty (string propertyName, string propertyTypeName, string getTemplate, string setTemplate)
    {
      BeginProperty (s_accessibilityPublic, s_modifierAbstract, propertyName, propertyTypeName, true);

      if (getTemplate != null)
        Write (getTemplate);

      if (setTemplate != null)
        Write (setTemplate);

      EndProperty (true);
    }

    private string GetPropertyTypeForRelationPropertyWithCardinalityOne (
        XmlBasedClassDefinition classDefinition, TypeName typeName, IRelationEndPointDefinition endPointDefinition)
    {
      return GetTypeName (typeName, GetOppositeClassTypeName (classDefinition, endPointDefinition));
    }

    private string GetPropertyTypeForRelationPropertyWithCardinalityMany (
        XmlBasedClassDefinition classDefinition, TypeName typeName, IRelationEndPointDefinition endPointDefinition)
    {
      TypeName propertyTypeName = new TypeName (endPointDefinition.PropertyTypeName);
      if (DomainObjectCollectionBuilder.DefaultBaseTypeName.CompareTo (propertyTypeName) != 0)
        return GetTypeName (typeName, propertyTypeName);

      TypeName oppositeClassTypeName = GetOppositeClassTypeName (classDefinition, endPointDefinition);
      return ReplaceTag (s_objectListTypeName, s_classnameTag, GetTypeName (typeName, oppositeClassTypeName));
    }

    private bool HasStringPropertyAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      if (!propertyDefinition.MappingTypeName.Equals ("string", StringComparison.Ordinal))
        return false;

      if (!propertyDefinition.IsNullable)
        return true;

      if (propertyDefinition.MaxLength.HasValue)
        return true;

      return false;
    }

    private void WriteStringPropertyAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      string output = s_stringPropertyAttribute;
      output = ReplaceTag (output, s_parameterlistTag, GetNullableLengthConstrainedPropertyAttributeParameterList (propertyDefinition));

      Write (output);
    }

    private bool HasBinaryPropertyAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      if (!propertyDefinition.MappingTypeName.Equals ("binary", StringComparison.Ordinal))
        return false;

      if (!propertyDefinition.IsNullable)
        return true;

      if (propertyDefinition.MaxLength.HasValue)
        return true;

      return false;
    }

    private void WriteBinaryPropertyAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      string output = s_binaryPropertyAttribute;
      output = ReplaceTag (output, s_parameterlistTag, GetNullableLengthConstrainedPropertyAttributeParameterList (propertyDefinition));

      Write (output);
    }

    private string GetNullableLengthConstrainedPropertyAttributeParameterList (XmlBasedPropertyDefinition propertyDefinition)
    {
      string parametersList = string.Empty;

      if (!propertyDefinition.IsNullable)
        parametersList += s_nullableAttributeIsNullableParameterWithFalse;

      if (propertyDefinition.MaxLength.HasValue)
      {
        if (parametersList.Length > 0)
          parametersList += ", ";
        parametersList += ReplaceTag (s_lengthConstrainedAttributeMaximumLengthParameter, s_parameterlistTag, propertyDefinition.MaxLength.ToString());
      }

      return parametersList;
    }

    private bool HasBidirectionalRelationAttribute (IRelationEndPointDefinition endPointDefinition)
    {
      if (endPointDefinition.IsNull)
        return false;

      if (endPointDefinition.Cardinality == CardinalityType.Many)
        return true;

      if (endPointDefinition.RelationDefinition.GetOppositeEndPointDefinition (endPointDefinition).IsNull)
        return false;

      return true;
    }

    private void WriteBidirectionalRelationAttribute (IRelationEndPointDefinition endPointDefinition)
    {
      string output = s_bidirectionalRelationAttribute;

      IRelationEndPointDefinition oppositeEndPointDefinition =
          endPointDefinition.RelationDefinition.GetOppositeEndPointDefinition (endPointDefinition);
      string parameterList = "\"" + oppositeEndPointDefinition.PropertyName + "\"";

      if (!endPointDefinition.IsVirtual && oppositeEndPointDefinition.Cardinality == CardinalityType.One)
        parameterList += s_bidirectionalRelationAttributeContainsForeignKeyParameterWithTrue;

      VirtualRelationEndPointDefinition virtualRelationEndPointDefinition = endPointDefinition as VirtualRelationEndPointDefinition;
      if (virtualRelationEndPointDefinition != null && virtualRelationEndPointDefinition.SortExpression != null)
        parameterList += s_bidirectionalRelationAttributeSortExpressionParameter + "\"" + virtualRelationEndPointDefinition.SortExpression + "\"";

      output = ReplaceTag (output, s_parameterlistTag, parameterList);

      Write (output);
    }

    private bool IsMandatory (IRelationEndPointDefinition endPointDefinition)
    {
      return endPointDefinition.IsMandatory;
    }

    private void WriteMandatoryAttribute ()
    {
      Write (s_mandatoryAttribute);
    }

    private bool HasColumnNameAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      if (propertyDefinition.StorageSpecificName.Equals (propertyDefinition.PropertyName, StringComparison.Ordinal))
        return false;

      if (propertyDefinition.MappingTypeName == TypeInfo.ObjectIDMappingTypeName
          && propertyDefinition.StorageSpecificName.Equals (propertyDefinition.PropertyName + "ID", StringComparison.Ordinal))
      {
        return false;
      }

      return true;
    }

    private bool HasColumnNameAttribute (IRelationEndPointDefinition endPointDefinition)
    {
      if (endPointDefinition.IsVirtual || endPointDefinition.IsNull)
        return false;

      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) endPointDefinition;
      return HasColumnNameAttribute ((XmlBasedPropertyDefinition) relationEndPointDefinition.PropertyDefinition);
    }

    private void WriteColumnNameAttribute (XmlBasedPropertyDefinition propertyDefinition)
    {
      string output = s_columnNameAttribute;
      output = ReplaceTag (output, s_parameterlistTag, "\"" + propertyDefinition.StorageSpecificName + "\"");

      Write (output);
    }

    private void WriteColumnNameAttribute (IRelationEndPointDefinition endPointDefinition)
    {
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) endPointDefinition;
      WriteColumnNameAttribute ((XmlBasedPropertyDefinition) relationEndPointDefinition.PropertyDefinition);
    }

    private TypeName GetOppositeClassTypeName (XmlBasedClassDefinition classDefinition, IRelationEndPointDefinition endPointDefinition)
    {
      return new TypeName (((XmlBasedClassDefinition) classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName)).ClassTypeName);
    }
  }
}
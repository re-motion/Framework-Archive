using System;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainObjectBuilder : CodeBuilder
{
  // types

  // static members and constants

  public static readonly string DefaultBaseClass = "BindableDomainObject";

  #region templates

  private static readonly string s_getObjectContent =
      "    return (%classname%) DomainObject.GetObject (%parameterlist%);" + Environment.NewLine;

  private static readonly string s_getObjectParameters = "ObjectID id";
  private static readonly string s_getObjectParametersForContent = "id";

  private static readonly string s_getObjectParametersWithDeleted = "ObjectID id, bool includeDeleted";
  private static readonly string s_getObjectParametersWithDeletedForContent = "id, includeDeleted";

  private static readonly string s_getObjectParametersWithTransaction = "ObjectID id, ClientTransaction clientTransaction";
  private static readonly string s_getObjectParametersWithTransactionForContent = "id, clientTransaction";

  private static readonly string s_getObjectParametersWithTransactionAndDeleted = "ObjectID id, ClientTransaction clientTransaction, bool includeDeleted";
  private static readonly string s_getObjectParametersWithTransactionAndDeletedForContent = "id, clientTransaction, includeDeleted";

  private static readonly string s_valuePropertyGetStatement = 
      "    get { return (%propertytype%) DataContainer[\"%propertyname%\"]; }" + Environment.NewLine;
  private static readonly string s_valuePropertySetStatement = 
      "    set { DataContainer[\"%propertyname%\"] = value; }" + Environment.NewLine;

  private static readonly string s_relationPropertyCardinalityOneGetStatement = 
      "    get { return (%propertytype%) GetRelatedObject (\"%propertyname%\"); }" + Environment.NewLine;
  private static readonly string s_relationPropertyCardinalityOneSetStatement = 
      "    set { SetRelatedObject (\"%propertyname%\", value); }" + Environment.NewLine;

  private static readonly string s_relationPropertyCardinalityManyGetStatement = 
      "    get { return (%propertytype%) GetRelatedObjects (\"%propertyname%\"); }" + Environment.NewLine;

  #endregion

  // member fields

  private ClassDefinition _classDefinition;
  private string _baseClass;

  // construction and disposing

	public DomainObjectBuilder (string filename, ClassDefinition classDefinition, string baseClass)
      : base (filename)
	{
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    _classDefinition = classDefinition;

    _baseClass = baseClass;
	}

  // methods and properties

  public override void Build ()
  {
    Type type = _classDefinition.ClassType;

    OpenFile ();

    BeginNamespace (type.Namespace);

    if (_classDefinition.BaseClass == null)
      BeginClass (type.Name, _baseClass);
    else
      BeginClass (type.Name, _classDefinition.BaseClass.ID);

    // types
    WriteComment ("types");
    WriteLine ();

    //Write nested types (enums)
    foreach (PropertyDefinition propertyDefinition in GetEnumPropertyDefinitionsWithNestedType (type))
      WriteNestedEnum (propertyDefinition.PropertyType.Name);

    // static members and constants
    WriteComment ("static members and constants");
    WriteLine ();

    WriteGetObject (type.Name);
    WriteGetObjectWithDeleted (type.Name);
    WriteGetObjectWithTransaction (type.Name);
    WriteGetObjectWithTransactionAndDeleted (type.Name);

    // member fields
    WriteComment ("member fields");
    WriteLine ();

    // construction and disposing
    WriteComment ("construction and disposing");
    WriteLine ();

    WriteConstructorDefault (type.Name);
    WriteConstructorWithTransaction (type.Name);
    WriteConstructorWithDataContainer (type.Name);

    // methods and properties
    WriteComment ("methods and properties");
    WriteLine ();

    foreach (PropertyDefinition propertyDefinition in _classDefinition.MyPropertyDefinitions)
    {
      if (propertyDefinition.PropertyType == typeof (ObjectID))
        continue;

      WriteValueProperty (propertyDefinition.PropertyName, TypeToCSharpString (propertyDefinition.PropertyType));
    }

    foreach (IRelationEndPointDefinition endPointDefinition in _classDefinition.GetMyRelationEndPointDefinitions ())
    {
      if (endPointDefinition.Cardinality == CardinalityType.One)
        WriteRelationPropertyCardinalityOne (endPointDefinition.PropertyName, _classDefinition.GetOppositeClassDefinition(endPointDefinition.PropertyName).ID);

      if (endPointDefinition.Cardinality == CardinalityType.Many)
        WriteRelationPropertyCardinalityMany (endPointDefinition.PropertyName, endPointDefinition.PropertyType.Name);
    }

    EndClass ();
  
    EndNamespace ();

    CloseFile ();
  }

  protected void WriteGetObject (string className)
  {
    BeginGetObject (className, s_getObjectParameters);

    string output = s_getObjectContent;
    output = ReplaceTag (output, s_classnameTag, className);
    output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersForContent);
    Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithDeleted (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithDeleted);

    string output = s_getObjectContent;
    output = ReplaceTag (output, s_classnameTag, className);
    output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithDeletedForContent);
    Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithTransaction (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithTransaction);

    string output = s_getObjectContent;
    output = ReplaceTag (output, s_classnameTag, className);
    output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionForContent);
    Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithTransactionAndDeleted (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithTransactionAndDeleted);

    string output = s_getObjectContent;
    output = ReplaceTag (output, s_classnameTag, className);
    output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionAndDeletedForContent);
    Write (output);

    EndGetObject ();
  }

  protected void BeginGetObject (string className, string parameters)
  {
    string accessibility = s_accessibilityPublic + " " + s_accessibilityStatic + " " + s_accessibilityNew;
    BeginMethod (accessibility, className, "GetObject", parameters);
  }

  protected void EndGetObject ()
  {
    EndMethod ();
  }

  protected void WriteConstructorDefault (string className)
  {
    BeginConstructor (className);
    EndConstructor ();
  }

  protected void WriteConstructorWithTransaction (string className)
  {
    BeginConstructor (className, "ClientTransaction clientTransaction", "clientTransaction");
    EndConstructor ();
  }

  protected void WriteConstructorWithDataContainer (string className)
  {
    BeginConstructor (s_accessibilityProtected, className, "DataContainer dataContainer", "dataContainer");
    EndConstructor ();
  }


  protected void WriteValueProperty (string propertyName, string propertyType)
  {
    BeginProperty (propertyName, propertyType);

    string getStatement = s_valuePropertyGetStatement;
    getStatement = ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = ReplaceTag (getStatement, s_propertynameTag, propertyName);
    Write (getStatement);

    string setStatement = s_valuePropertySetStatement;
    setStatement = ReplaceTag (setStatement, s_propertynameTag, propertyName);
    Write (setStatement);

    EndProperty ();
  }
  
  protected void WriteRelationPropertyCardinalityOne (string propertyName, string propertyType)
  {
    BeginProperty (propertyName, propertyType);

    string getStatement = s_relationPropertyCardinalityOneGetStatement;
    getStatement = ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = ReplaceTag (getStatement, s_propertynameTag, propertyName);
    Write (getStatement);

    string setStatement = s_relationPropertyCardinalityOneSetStatement;
    setStatement = ReplaceTag (setStatement, s_propertynameTag, propertyName);
    Write (setStatement);

    EndProperty ();
  }
  
  protected void WriteRelationPropertyCardinalityMany (string propertyName, string propertyType)
  {
    BeginProperty (propertyName, propertyType);

    string getStatement = s_relationPropertyCardinalityManyGetStatement;
    getStatement = ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = ReplaceTag (getStatement, s_propertynameTag, propertyName);
    Write (getStatement);

    EndProperty ();
  }
}
}

using System;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
//TODO: implement mapping from CLS Type to C# value types (string, int, double, ...)
public class DomainObjectBuilder : CodeBuilder, IBuilder
{
  // types

  // static members and constants

  private static readonly string s_defaultBaseClass = "BindableDomainObject";

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
      "    set { DataConatainer[\"%propertyname%\"] = value; }" + Environment.NewLine;

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
    Initialize (classDefinition, baseClass);
	}

	public DomainObjectBuilder (TextWriter textWriter, ClassDefinition classDefinition, string baseClass)
      : base (textWriter)
	{
    Initialize (classDefinition, baseClass);
	}

  private void Initialize (ClassDefinition classDefinition, string baseClass)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    _classDefinition = classDefinition;

    if (baseClass != null)
      _baseClass = baseClass;
    else
      _baseClass = s_defaultBaseClass;
  }

  // methods and properties

  public virtual void Build ()
  {
    Type type = _classDefinition.ClassType;

    OpenFile ();

    BeginNamespace (type.Namespace);

    BeginClass (type.Name, _baseClass);

    // types
    WriteComment ("types");
    WriteNewLine ();

    //Write nested types (enums)
    foreach (PropertyDefinition propertyDefinition in BuilderUtility.GetPropertyDefinitionsWithNestedType (type))
      WriteNestedEnum (propertyDefinition.PropertyType.Name);

    // static members and constants
    WriteComment ("static members and constants");
    WriteNewLine ();

    WriteGetObject (type.Name);
    WriteGetObjectWithDeleted (type.Name);
    WriteGetObjectWithTransaction (type.Name);
    WriteGetObjectWithTransactionAndDeleted (type.Name);

    // member fields
    WriteComment ("member fields");
    WriteNewLine ();

    // construction and disposing
    WriteComment ("construction and disposing");
    WriteNewLine ();

    WriteConstructorDefault (type.Name);
    WriteConstructorWithTransaction (type.Name);
    WriteConstructorWithDataContainer (type.Name);

    // methods and properties
    WriteComment ("methods and properties");
    WriteNewLine ();

    foreach (PropertyDefinition propertyDefinition in _classDefinition.MyPropertyDefinitions)
    {
      if (propertyDefinition.PropertyType == typeof (ObjectID))
        continue;

      WriteValueProperty (propertyDefinition.PropertyName, propertyDefinition.PropertyType.Name);
    }

    //TODO: improve this snipped with _classDefinition.GetRelationEndPoints (is implemented in latest RPF build)
    foreach (RelationDefinition relationDefinition in _classDefinition.MyRelationDefinitions)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (endPointDefinition.ClassDefinition != _classDefinition)
          continue;

        if (endPointDefinition.Cardinality == CardinalityType.One)
          WriteRelationPropertyCardinalityOne (endPointDefinition.PropertyName, endPointDefinition.PropertyType.Name);

        if (endPointDefinition.Cardinality == CardinalityType.Many)
          WriteRelationPropertyCardinalityMany (endPointDefinition.PropertyName, endPointDefinition.PropertyType.Name);
      }
    }

    EndClass ();
  
    EndNamespace ();

    CloseFile ();
  }

  protected void WriteGetObject (string className)
  {
    BeginGetObject (className, s_getObjectParameters);

    string output = s_getObjectContent;
    output = BuilderUtility.ReplaceTag (output, s_classnameTag, className);
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, s_getObjectParametersForContent);
    ClassWriter.Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithDeleted (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithDeleted);

    string output = s_getObjectContent;
    output = BuilderUtility.ReplaceTag (output, s_classnameTag, className);
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithDeletedForContent);
    ClassWriter.Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithTransaction (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithTransaction);

    string output = s_getObjectContent;
    output = BuilderUtility.ReplaceTag (output, s_classnameTag, className);
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionForContent);
    ClassWriter.Write (output);

    EndGetObject ();
  }

  protected void WriteGetObjectWithTransactionAndDeleted (string className)
  {
    BeginGetObject (className, s_getObjectParametersWithTransactionAndDeleted);

    string output = s_getObjectContent;
    output = BuilderUtility.ReplaceTag (output, s_classnameTag, className);
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionAndDeletedForContent);
    ClassWriter.Write (output);

    EndGetObject ();
  }

  protected void BeginGetObject (string className, string parameters)
  {
    string accessibility = s_accessibilityPublic + " " + s_accessibilityStatic;
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
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertynameTag, propertyName);
    ClassWriter.Write (getStatement);

    string setStatement = s_valuePropertySetStatement;
    setStatement = BuilderUtility.ReplaceTag (setStatement, s_propertynameTag, propertyName);
    ClassWriter.Write (setStatement);

    EndProperty ();
  }
  
  protected void WriteRelationPropertyCardinalityOne (string propertyName, string propertyType)
  {
    BeginProperty (propertyName, propertyType);

    string getStatement = s_relationPropertyCardinalityOneGetStatement;
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertynameTag, propertyName);
    ClassWriter.Write (getStatement);

    string setStatement = s_relationPropertyCardinalityOneSetStatement;
    setStatement = BuilderUtility.ReplaceTag (setStatement, s_propertynameTag, propertyName);
    ClassWriter.Write (setStatement);

    EndProperty ();
  }
  
  protected void WriteRelationPropertyCardinalityMany (string propertyName, string propertyType)
  {
    BeginProperty (propertyName, propertyType);

    string getStatement = s_relationPropertyCardinalityManyGetStatement;
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertytypeTag, propertyType);
    getStatement = BuilderUtility.ReplaceTag (getStatement, s_propertynameTag, propertyName);
    ClassWriter.Write (getStatement);

    EndProperty ();
  }
}
}

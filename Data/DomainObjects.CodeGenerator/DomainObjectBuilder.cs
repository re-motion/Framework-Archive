using System;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
  
public class DomainObjectBuilder
{
  // types

  private class Builder: CodeFileBuilder
  {
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
    private static readonly string s_relationPropertyCardinalityManySetStatement = "    set { } // marks property %propertyname% as modifiable" + Environment.NewLine;
    #endregion

    private ClassDefinition _classDefinition;

    public Builder (TextWriter writer) : base (writer)
    {
    }

    public void Build (ClassDefinition classDefinition, string baseClass, bool serializableAttribute, bool multiLingualResourcesAttribute)
    {
      _classDefinition = classDefinition;
      Type type = classDefinition.ClassType;

      BeginNamespace (type.Namespace);

      if (serializableAttribute)
        WriteSerializableAttribute ();

      if (multiLingualResourcesAttribute)
        WriteMultiLingualResourcesAttribute (type);

      if (classDefinition.BaseClass == null)
        BeginClass (type.Name, baseClass);
      else
        BeginClass (type.Name, classDefinition.BaseClass.ID);

      // types
      WriteComment ("types");
      WriteLine ();

      //Write nested types (enums)
      foreach (PropertyDefinition propertyDefinition in GetEnumPropertyDefinitionsWithNestedType (type))
        WriteNestedEnum (propertyDefinition.PropertyType, multiLingualResourcesAttribute);

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

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (propertyDefinition.PropertyType == typeof (ObjectID))
          continue;

        WriteValueProperty (propertyDefinition.PropertyName, TypeToCSharpString (propertyDefinition.PropertyType));
      }

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality == CardinalityType.One)
          WriteRelationPropertyCardinalityOne (endPointDefinition.PropertyName, classDefinition.GetOppositeClassDefinition(endPointDefinition.PropertyName).ClassType);

        if (endPointDefinition.Cardinality == CardinalityType.Many)
          WriteRelationPropertyCardinalityMany (endPointDefinition.PropertyName, endPointDefinition.PropertyType);
      }

      EndClass ();
    
      EndNamespace ();

      FinishFile ();
    }


    private void WriteGetObject (string className)
    {
      BeginGetObject (className, s_getObjectParameters);

      string output = s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersForContent);
      Write (output);

      EndGetObject ();
    }

    private void WriteGetObjectWithDeleted (string className)
    {
      BeginGetObject (className, s_getObjectParametersWithDeleted);

      string output = s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithDeletedForContent);
      Write (output);

      EndGetObject ();
    }

    private void WriteGetObjectWithTransaction (string className)
    {
      BeginGetObject (className, s_getObjectParametersWithTransaction);

      string output = s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionForContent);
      Write (output);

      EndGetObject ();
    }

    private void WriteGetObjectWithTransactionAndDeleted (string className)
    {
      BeginGetObject (className, s_getObjectParametersWithTransactionAndDeleted);

      string output = s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, s_getObjectParametersWithTransactionAndDeletedForContent);
      Write (output);

      EndGetObject ();
    }

    private void BeginGetObject (string className, string parameters)
    {
      string accessibility = s_accessibilityPublic + " " + s_accessibilityStatic + " " + s_accessibilityNew;
      BeginMethod (accessibility, className, "GetObject", parameters);
    }

    private void EndGetObject ()
    {
      EndMethod ();
    }

    private void WriteConstructorDefault (string className)
    {
      BeginConstructor (className);
      EndConstructor ();
    }

    private void WriteConstructorWithTransaction (string className)
    {
      BeginConstructor (className, "ClientTransaction clientTransaction", "clientTransaction");
      EndConstructor ();
    }

    private void WriteConstructorWithDataContainer (string className)
    {
      BeginConstructor (s_accessibilityProtected, className, "DataContainer dataContainer", "dataContainer");
      WriteComment ("This infrastructure constructor is necessary for the DomainObjects framework.");
      WriteComment ("Do not remove the constructor or place any code here.");
      EndConstructor ();
    }

    private void WriteValueProperty (string propertyName, string propertyTypeName)
    {
      WriteProperty (propertyName, propertyTypeName, s_valuePropertyGetStatement, s_valuePropertySetStatement);
    }
    
    private void WriteRelationPropertyCardinalityOne (string propertyName, Type propertyType)
    {
      WriteProperty (propertyName, GetTypeName (propertyType), s_relationPropertyCardinalityOneGetStatement, s_relationPropertyCardinalityOneSetStatement);
    }
    
    private void WriteRelationPropertyCardinalityMany (string propertyName, Type propertyType)
    {
      WriteProperty (propertyName, GetTypeName (propertyType), s_relationPropertyCardinalityManyGetStatement, s_relationPropertyCardinalityManySetStatement);
    }

    private void WriteProperty (string propertyName, string propertyTypeName, string getTemplate, string setTemplate)
    {
      BeginProperty (propertyName, propertyTypeName);

      if (getTemplate != null)
      {
        string getStatement = getTemplate;
        getStatement = ReplaceTag (getStatement, s_propertytypeTag, propertyTypeName);
        getStatement = ReplaceTag (getStatement, s_propertynameTag, propertyName);
        Write (getStatement);
      }

      if (setTemplate != null)
      {
        string setStatement = setTemplate;
        setStatement = ReplaceTag (setStatement, s_propertynameTag, propertyName);
        Write (setStatement);
      }

      EndProperty ();
    }

    private string GetTypeName (Type type)
    {
      if (_classDefinition.ClassType.Namespace == type.Namespace || _classDefinition.ClassType.Namespace.StartsWith (type.Namespace))
        return type.Name;
      else
        return type.FullName;
    }
  }

  // static members and constants

  public const string DefaultBaseClass = "BindableDomainObject";

  public static void Build (
      string filename, ClassDefinition classDefinition, 
      string baseClass, bool serializableAttribute, bool multiLingualResourcesAttribute)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

    using (TextWriter writer = new StreamWriter (filename))
    {
      Build (writer, classDefinition, baseClass, serializableAttribute, multiLingualResourcesAttribute);
    }
  }

  public static void Build (
      TextWriter writer, ClassDefinition classDefinition, 
      string baseClass, bool serializableAttribute, bool multiLingualResourcesAttribute)
  {
    ArgumentUtility.CheckNotNull ("stream", writer);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    Builder builder = new Builder (writer);
    builder.Build (classDefinition, baseClass, serializableAttribute, multiLingualResourcesAttribute);
  }

  private DomainObjectBuilder()
  {
  }
}

}

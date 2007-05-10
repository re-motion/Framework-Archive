using System;
using System.IO;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator
{
  public class DomainObjectBuilder : CodeFileBuilder
  {
    // types

    // static members and constants

    // types

    // static members and constants

    public const string DefaultBaseClass = "BindableDomainObject";

    public static void Build (
        MappingConfiguration mappingConfiguration,
        string filename, 
        XmlBasedClassDefinition classDefinition, 
        string baseClass, 
        bool serializableAttribute, 
        bool multiLingualResourcesAttribute)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

      using (TextWriter writer = new StreamWriter (filename))
      {
        Build (mappingConfiguration, writer, classDefinition, baseClass, serializableAttribute, multiLingualResourcesAttribute);
      }
    }

    public static void Build (
        MappingConfiguration mappingConfiguration,
        TextWriter writer, 
        XmlBasedClassDefinition classDefinition, 
        string baseClass, 
        bool serializableAttribute, 
        bool multiLingualResourcesAttribute)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      DomainObjectBuilder builder = new DomainObjectBuilder (mappingConfiguration, writer);
      builder.Build (classDefinition, baseClass, serializableAttribute, multiLingualResourcesAttribute);
    }

    #region templates

    private static readonly string s_getObjectContent =
        "      return (%classname%) DomainObject.GetObject (%parameterlist%);\r\n";

    private static readonly string s_getObjectParameters = "ObjectID id";
    private static readonly string s_getObjectParametersForContent = "id";

    private static readonly string s_getObjectParametersWithTransaction = "ObjectID id, ClientTransaction clientTransaction";
    private static readonly string s_getObjectParametersWithTransactionForContent = "id, clientTransaction";

    private static readonly string s_nestedEnum =
        "    public enum %enumname%\r\n"
        + "    {\r\n"
        + "      DummyEntry = 0\r\n"
        + "    }\r\n"
        + "\r\n";

    private static readonly string s_valuePropertyGetStatement = 
        "      get { return (%propertytype%) DataContainer[\"%propertyname%\"]; }\r\n";
    private static readonly string s_valuePropertySetStatement = 
        "      set { DataContainer[\"%propertyname%\"] = value; }\r\n";

    private static readonly string s_relationPropertyCardinalityOneGetStatement = 
        "      get { return (%propertytype%) GetRelatedObject (\"%propertyname%\"); }\r\n";
    private static readonly string s_relationPropertyCardinalityOneSetStatement = 
        "      set { SetRelatedObject (\"%propertyname%\", value); }\r\n";

    private static readonly string s_relationPropertyCardinalityManyGetStatement = 
        "      get { return (%propertytype%) GetRelatedObjects (\"%propertyname%\"); }\r\n";
    private static readonly string s_relationPropertyCardinalityManySetStatement = 
        "      set { } // marks property %propertyname% as modifiable\r\n";

    #endregion

    // member fields

    private MappingConfiguration _mappingConfiguration;

    // construction and disposing
    
    public DomainObjectBuilder (MappingConfiguration mappingConfiguration, TextWriter writer) : base (writer)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);

      _mappingConfiguration = mappingConfiguration;
    }

    // methods and properties

    public MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    private void Build (XmlBasedClassDefinition classDefinition, string baseClass, bool serializableAttribute, bool multiLingualResourcesAttribute)
    {
      TypeName typeName = new TypeName (classDefinition.ClassTypeName);

      BeginNamespace (typeName.Namespace);

      if (serializableAttribute)
        WriteSerializableAttribute ();

      if (multiLingualResourcesAttribute)
        WriteMultiLingualResourcesAttribute (typeName);

      if (classDefinition.BaseClass == null)
      {
        BeginClass (typeName.Name, baseClass, classDefinition.GetEntityName() == null);
      }
      else
      {
        TypeName baseTypeName = new TypeName (classDefinition.BaseClass.ClassTypeName);
        //TODO ES: decide if typeName must be fully qualified
        BeginClass (typeName.Name, baseTypeName.Name, classDefinition.GetEntityName () == null);
      }

      // types
      WriteComment ("types");
      WriteLine ();

      //Write nested types (enums)
      foreach (TypeName nestedEnumTypeName in GetNestedPropertyTypeNames (typeName))
        WriteNestedEnum (nestedEnumTypeName, multiLingualResourcesAttribute);

      // static members and constants
      WriteComment ("static members and constants");
      WriteLine ();

      WriteGetObject (typeName.Name, s_getObjectParameters, s_getObjectParametersForContent);
      WriteGetObject (typeName.Name, s_getObjectParametersWithTransaction, s_getObjectParametersWithTransactionForContent);

      // member fields
      WriteComment ("member fields");
      WriteLine ();

      // construction and disposing
      WriteComment ("construction and disposing");
      WriteLine ();

      WriteConstructor (typeName.Name, null, null);
      WriteConstructor (typeName.Name, "ClientTransaction clientTransaction", "clientTransaction");
      WriteInfrastructureConstructor (typeName.Name);

      // methods and properties
      WriteComment ("methods and properties");
      WriteLine ();

      foreach (XmlBasedPropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (propertyDefinition.MappingTypeName == TypeInfo.ObjectIDMappingTypeName)
          continue;

        //TODO ES: do not qualify with the name of the declaring type if it is declared in this type
        WriteValueProperty (propertyDefinition.PropertyName, GetCSharpTypeName (propertyDefinition));
      }

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality == CardinalityType.One)
        {
          TypeName propertyTypeName = new TypeName (
              ((XmlBasedClassDefinition) classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName)).ClassTypeName);
          WriteProperty (endPointDefinition.PropertyName, GetTypeName (typeName, propertyTypeName), s_relationPropertyCardinalityOneGetStatement, s_relationPropertyCardinalityOneSetStatement);
        }
        else
        {
          WriteProperty (endPointDefinition.PropertyName, GetTypeName (typeName, new TypeName (endPointDefinition.PropertyTypeName)), s_relationPropertyCardinalityManyGetStatement, s_relationPropertyCardinalityManySetStatement);
        }
      }

      EndClass ();
      EndNamespace ();
      FinishFile ();
    }

    private void WriteGetObject (string className, string parameterList, string parametersListForGetObjectInvocation)
    {
      BeginMethod (s_accessibilityPublic, s_modifierStatic + s_modifierNew, className, "GetObject", parameterList);

      string output = s_getObjectContent;
      output = ReplaceTag (output, s_classnameTag, className);
      output = ReplaceTag (output, s_parameterlistTag, parametersListForGetObjectInvocation);
      Write (output);

      EndMethod ();
    }

    private void WriteConstructor (string className, string parameterList, string parameterListForBaseConstructorCall)
    {
      BeginConstructor (className, parameterList, parameterListForBaseConstructorCall);
      EndConstructor ();
    }

    private void WriteInfrastructureConstructor (string className)
    {
      BeginConstructor (s_accessibilityProtected, className, "DataContainer dataContainer", "dataContainer");
      WriteIndentation (1);
      WriteComment ("This infrastructure constructor is necessary for the DomainObjects framework.");
      WriteIndentation (1);
      WriteComment ("Do not remove the constructor or place any code here.");
      WriteIndentation (1);
      WriteComment ("For any code that should run when a DomainObject is loaded, OnLoaded () should be overridden.");
      EndConstructor ();
    }

    private void WriteValueProperty (string propertyName, string propertyTypeName)
    {
      WriteProperty (propertyName, propertyTypeName, s_valuePropertyGetStatement, s_valuePropertySetStatement);
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

    protected void WriteNestedEnum (TypeName enumTypeName, bool enumDescriptionResourceAttribute)
    {
      if (enumDescriptionResourceAttribute)
      {
        WriteIndentation (1);
        WriteEnumDescriptionResourceAttribute (enumTypeName);
      }
      Write (ReplaceTag (s_nestedEnum, s_enumTag, enumTypeName.Name));
    }

    private string GetTypeName (TypeName typeNameOfClassWritten, TypeName typeName)
    {
      if (typeNameOfClassWritten.Namespace == typeName.Namespace || typeNameOfClassWritten.Namespace.StartsWith (typeName.Namespace))
        return typeName.Name;
      else
        return typeName.FullName;
    }

    public List<TypeName> GetAllDistinctPropertyTypeNames ()
    {
      List<TypeName> propertyTypeNames = new List<TypeName> ();
      foreach (ClassDefinition classDefinition in _mappingConfiguration.ClassDefinitions)
      {
        foreach (XmlBasedPropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          if (propertyDefinition.MappingTypeName.Contains (","))
          {
            TypeName propertyTypeName = new TypeName (propertyDefinition.MappingTypeName);

            if (!IsTypeNameAlreadyInList (propertyTypeNames, propertyTypeName))
              propertyTypeNames.Add (propertyTypeName);
          }
        }
      }
      return propertyTypeNames;
    }

    private bool IsTypeNameAlreadyInList (List<TypeName> typeNames, TypeName typeName)
    {
      foreach (TypeName typeNameInList in typeNames)
      {
        if (typeNameInList.CompareTo(typeName) == 0)
          return true;
      }
      return false;
    }

    public List<TypeName> GetNestedPropertyTypeNames (TypeName typeName)
    {
      List<TypeName> nestedPropertyTypeNames = new List<TypeName> ();

      foreach (TypeName propertyTypeName in GetAllDistinctPropertyTypeNames ())
      {
        if (propertyTypeName.IsNested && propertyTypeName.DeclaringTypeName.CompareTo (typeName) == 0)
          nestedPropertyTypeNames.Add (propertyTypeName);
      }

      return nestedPropertyTypeNames;
    }
  }
}

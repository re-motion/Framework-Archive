using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator
{
public class CodeFileBuilder: FileBuilder
{
  // types

  // static members and constants

  #region CSharp

  private static Hashtable s_TypeToCSharpType;

  static CodeFileBuilder ()
  {
    s_TypeToCSharpType = new Hashtable ();
    s_TypeToCSharpType.Add (typeof(bool), "bool");
    s_TypeToCSharpType.Add (typeof(byte), "byte");
    s_TypeToCSharpType.Add (typeof(decimal), "decimal");
    s_TypeToCSharpType.Add (typeof(double), "double");
    s_TypeToCSharpType.Add (typeof(float), "float");
    s_TypeToCSharpType.Add (typeof(int), "int");
    s_TypeToCSharpType.Add (typeof(long), "long");
    s_TypeToCSharpType.Add (typeof(short), "short");
    s_TypeToCSharpType.Add (typeof(string), "string");
    s_TypeToCSharpType.Add (typeof(byte[]), "byte[]");
  }

  #endregion

  #region Tags

  protected static readonly string s_commentTag = "%comment%";
  protected static readonly string s_namespaceTag = "%namespace%";
  protected static readonly string s_classnameTag = "%classname%";
  protected static readonly string s_baseClassnameTag = "%baseclassname%";
  protected static readonly string s_accessibilityTag = "%accessibility%";
  protected static readonly string s_modifierTag = "%modifier%";
  protected static readonly string s_returntypeTag = "%returntype%";
  protected static readonly string s_methodnameTag = "%methodname%";
  protected static readonly string s_parameterlistTag = "%parameterlist%";
  protected static readonly string s_baseConstructorTag = "%baseconstructor%";
  protected static readonly string s_enumTag = "%enumname%";
  protected static readonly string s_propertytypeTag = "%propertytype%";
  protected static readonly string s_propertynameTag = "%propertyname%";
  //protected static readonly string s_resourcesTag = "%resources%";
  protected static readonly string s_typeNameTag = "%typename%";

  #endregion

  #region Templates

  private static readonly string s_indentation = "  ";

  private static readonly string s_comment =
      "    // %comment%\r\n";
  private static readonly string s_fileHeader = 
      "using System;\r\n"
      + "\r\n" 
      + "using Rubicon.Data.DomainObjects;\r\n"
      + "using Rubicon.Data.DomainObjects.ObjectBinding;\r\n"
      + "using Rubicon.NullableValueTypes;\r\n"
      + "using Rubicon.Globalization;\r\n"
      + "using Rubicon.Utilities;\r\n"
      + "\r\n";
  private static readonly string s_serializableAttribute = 
      "  [Serializable]\r\n";
  private static readonly string s_multilingualResourcesAttribute = 
      "  [MultiLingualResources(\"%namespace%.Globalization.%typename%\")]\r\n";
  private static readonly string s_enumDescriptionResourceAttribute = 
      "  [EnumDescriptionResource(\"%namespace%.Globalization.%typename%\")]\r\n";

  private static readonly string s_namespaceHeader = 
      "namespace %namespace%\r\n"
      + "{\r\n";
  private static readonly string s_namespaceFooter = "}\r\n";

  private static readonly string s_classHeader = 
      "  public %modifier%class %classname% : %baseclassname%\r\n"
      + "  {\r\n";
  private static readonly string s_classFooter = 
      "  }\r\n";

  protected static readonly string s_modifierStatic = "static ";
  protected static readonly string s_modifierNew = "new ";
  protected static readonly string s_modifierAbstract = "abstract ";

  protected static readonly string s_accessibilityPrivate = "private ";
  protected static readonly string s_accessibilityProtected = "protected ";
  protected static readonly string s_accessibilityPublic = "public ";

  protected static readonly string s_returntypeVoid = "void";

  private static readonly string s_methodHeader = 
      "    %accessibility%%modifier%%returntype% %methodname% (%parameterlist%)\r\n"
      + "    {\r\n";
  private static readonly string s_methodFooter =
      "    }\r\n"
      + "\r\n";

  private static readonly string s_constructorHeader = 
      "    %accessibility%%classname% (%parameterlist%)%baseconstructor%\r\n"
      + "    {\r\n";
  private static readonly string s_constructorFooter =
      "    }\r\n"
      + "\r\n";

  private readonly string s_baseConstructorCall = "\r\n      : base (%parameterlist%)";

  private static readonly string s_propertyHeader = 
      "    %accessibility%%propertytype% %propertyname%\r\n"
      + "    {\r\n";
  private static readonly string s_propertyFooter =
      "    }\r\n"
      + "\r\n";

  #endregion

  // member fields

  private bool _fileHeaderWritten = false;
  private string _lastNamespaceWritten = string.Empty;

  // construction and disposing

  protected CodeFileBuilder (TextWriter writer) : base (writer)
	{
  }
  
  // methods and properties

  protected virtual string GetCSharpTypeName (XmlBasedPropertyDefinition propertyDefinition)
  {
    TypeName typeName;
    if (propertyDefinition.MappingTypeName.Contains (","))
    {
      typeName = new TypeName (propertyDefinition.MappingTypeName);
    }
    else
    {
      TypeInfo typeInfo = TypeInfo.GetInstance (propertyDefinition.MappingTypeName, propertyDefinition.IsNullable);

      string cSharpTypeString = (string) s_TypeToCSharpType[typeInfo.Type];
      if (cSharpTypeString != null)
        return cSharpTypeString;
      else
        typeName = new TypeName (typeInfo.Type.AssemblyQualifiedName);
    }

    if (typeName.IsNested)
      return typeName.DeclaringTypeName.Name + "." + typeName.Name;

    return typeName.Name;
  }

  protected override void FinishFile ()
  {
    if (_lastNamespaceWritten != string.Empty)
      EndNamespace ();
    base.FinishFile();
  }

  protected void WriteIndentation (int indentations)
  {
    for (int i = 0; i < indentations; ++i)
      Write (s_indentation);
  }

  protected void WriteComment (string comment)
  {
    string commentLine = s_comment;

    commentLine = ReplaceTag (commentLine, s_commentTag, comment);

    Write (commentLine);
  }

  //TODO ES: remove additional logic with _fileHeaderWritten when ConfigurationLoader is gone and rename to WriteFileHeader
  protected void BeginFile ()
  {
    if (!_fileHeaderWritten)
    {
      Write (s_fileHeader);
      _fileHeaderWritten = true;
    }
  }

  //TODO ES: remove additional logic with _lastNamespaceWritten when ConfigurationLoader is gone
  protected void BeginNamespace (string namespaceName)
  {
    BeginFile ();
    if (_lastNamespaceWritten != namespaceName)
    {
      if (_lastNamespaceWritten != string.Empty)
        EndNamespace ();

      Write (ReplaceTag (s_namespaceHeader, s_namespaceTag, namespaceName));
      _lastNamespaceWritten = namespaceName;
    }
  }

  protected void EndNamespace ()
  {
    Write (s_namespaceFooter);
    _lastNamespaceWritten = string.Empty;
  }

  protected void WriteSerializableAttribute ()
  {
    Write (s_serializableAttribute);
  }

  //TODO: "Ressources" should not be inserted before the type name, but after the assembly name in the namespace
  protected void WriteMultiLingualResourcesAttribute (TypeName typeName)
  {
    string attributeString = s_multilingualResourcesAttribute;
    attributeString = ReplaceTag (attributeString, s_typeNameTag, typeName.Name);
    attributeString = ReplaceTag (attributeString, s_namespaceTag, typeName.Namespace);
    Write (attributeString);
  }

  //TODO: "Ressources" should not be inserted before the type name, but after the assembly name in the namespace
  protected void WriteEnumDescriptionResourceAttribute (TypeName typeName)
  {
    string attributeString = s_enumDescriptionResourceAttribute;
    
    if (typeName.IsNested) 
      attributeString = ReplaceTag (attributeString, s_typeNameTag, typeName.DeclaringTypeName.Name + "+" + typeName.Name);
    else
      attributeString = ReplaceTag (attributeString, s_typeNameTag, typeName.Name);

    attributeString = ReplaceTag (attributeString, s_namespaceTag, typeName.Namespace);
    Write (attributeString);
  }

  protected void BeginClass (string className, string baseClassName, bool abstractClass)
  {
    string classHeader = s_classHeader;
    classHeader = ReplaceTag (classHeader, s_modifierTag, abstractClass ? s_modifierAbstract : string.Empty);
    classHeader = ReplaceTag (classHeader, s_classnameTag, className);
    classHeader = ReplaceTag (classHeader, s_baseClassnameTag, baseClassName);

    Write (classHeader);
  }

  protected void EndClass ()
  {
    Write (s_classFooter);
  }

  protected void BeginMethod (string accessibility, string modifier, string returnType, string methodName, string parameterlist)
  {
    ArgumentUtility.CheckNotNull ("accessibility", accessibility);
    ArgumentUtility.CheckNotNull ("returnType", returnType);
    ArgumentUtility.CheckNotNull ("methodName", methodName);
    ArgumentUtility.CheckNotNull ("parameterlist", parameterlist);

    string constructor = s_methodHeader;
    constructor = ReplaceTag (constructor, s_accessibilityTag, accessibility);
    constructor = ReplaceTag (constructor, s_modifierTag, modifier);
    constructor = ReplaceTag (constructor, s_returntypeTag, returnType);
    constructor = ReplaceTag (constructor, s_methodnameTag, methodName);
    constructor = ReplaceTag (constructor, s_parameterlistTag, parameterlist);

    Write (constructor);
  }

  protected void EndMethod ()
  {
    Write (s_methodFooter);
  }

  protected void BeginConstructor (string className, string parameterlist, string parameterListForBaseConstructorCall)
  {
    BeginConstructor (s_accessibilityPublic, className, parameterlist, parameterListForBaseConstructorCall);
  }

  protected void BeginConstructor (string accessibility, string className, string parameterlist, string parameterListForBaseConstructorCall)
  {
    ArgumentUtility.CheckNotNull ("className", className);

    string constructor = s_constructorHeader;
    constructor = ReplaceTag (constructor, s_accessibilityTag, accessibility);
    constructor = ReplaceTag (constructor, s_classnameTag, className);
    constructor = ReplaceTag (constructor, s_parameterlistTag, parameterlist);

    string baseConstructorCall;
    if (parameterListForBaseConstructorCall != null)
      baseConstructorCall = ReplaceTag (s_baseConstructorCall, s_parameterlistTag, parameterListForBaseConstructorCall);
    else
      baseConstructorCall = string.Empty;

    constructor = ReplaceTag (constructor, s_baseConstructorTag, baseConstructorCall);

    Write (constructor);
  }

  protected void EndConstructor ()
  {
    Write (s_constructorFooter);
  }

  protected void BeginProperty (string propertyName, string propertyType)
  {
    BeginProperty (s_accessibilityPublic, propertyName, propertyType);
  }

  protected void BeginProperty (string accessibility, string propertyName, string propertyType)
  {
    string property = s_propertyHeader;
    property = ReplaceTag (property, s_accessibilityTag, accessibility);
    property = ReplaceTag (property, s_propertynameTag, propertyName);
    property = ReplaceTag (property, s_propertytypeTag, propertyType);

    Write (property);
  }

  protected void EndProperty ()
  {
    Write (s_propertyFooter);
  }
}
}

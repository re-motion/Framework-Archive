using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public abstract class CodeBuilder : IBuilder
{
  // types

  // static members and constants

  #region Tags

  protected readonly string s_commentTag = "%comment%";
  protected readonly string s_namespaceTag = "%namespace%";
  protected readonly string s_classnameTag = "%classname%";
  protected readonly string s_baseClassnameTag = "%baseclassname%";
  protected readonly string s_accessibilityTag = "%accessibility%";
  protected readonly string s_returntypeTag = "%returntype%";
  protected readonly string s_methodnameTag = "%methodname%";
  protected readonly string s_parameterlistTag = "%parameterlist%";
  protected readonly string s_baseConstructorTag = "%baseconstructor%";
  protected readonly string s_enumTag = "%enumname%";
  protected readonly string s_propertytypeTag = "%propertytype%";
  protected readonly string s_propertynameTag = "%propertyname%";

  #endregion

  #region Templates

  private static readonly string s_comment =
      "  // %comment%" + Environment.NewLine;
  private static readonly string s_fileHeader = 
      "using System;" + Environment.NewLine
      + "using Rubicon.Data.DomainObjects;" + Environment.NewLine 
      + Environment.NewLine;

  private static readonly string s_namespaceHeader = 
      "namespace %namespace%" + Environment.NewLine
      + "{" + Environment.NewLine;
  private static readonly string s_namespaceFooter = "}" + Environment.NewLine;

  private static readonly string s_classHeader = 
      "public class %classname% : %baseclassname%" + Environment.NewLine
      + "{" + Environment.NewLine;
  private static readonly string s_classFooter = 
      "}" + Environment.NewLine;

  protected static readonly string s_accessibilityStatic = "static";
  protected static readonly string s_accessibilityNew = "new";
  protected static readonly string s_accessibilityPrivate = "private";
  protected static readonly string s_accessibilityProtected = "protected";
  protected static readonly string s_accessibilityPublic = "public";
  protected static readonly string s_accessibilityDefault = "public";

  protected static readonly string s_returntypeVoid = "void";

  private static readonly string s_methodHeader = 
      "  %accessibility% %returntype% %methodname% (%parameterlist%)" + Environment.NewLine
      + "  {" + Environment.NewLine;
  private static readonly string s_methodFooter =
      "  }" + Environment.NewLine
      + "" + Environment.NewLine;

  private static readonly string s_indexerHeader = 
      "  %accessibility% %returntype% this[%parameterlist%]" + Environment.NewLine
      + "  {" + Environment.NewLine;
  private static readonly string s_indexerFooter =
      "  }" + Environment.NewLine
      + "" + Environment.NewLine;

  private static readonly string s_constructorHeader = 
      "  %accessibility% %classname% (%parameterlist%)%baseconstructor%" + Environment.NewLine
      + "  {" + Environment.NewLine;
  private static readonly string s_constructorFooter =
      "  }" + Environment.NewLine
      + "" + Environment.NewLine;

  private readonly string s_baseConstructorCall = " : base (%parameterlist%)";

  private static readonly string s_enum = 
      "public enum %enumname%" + Environment.NewLine
      + "{" + Environment.NewLine
      + "  DummyEntry = 0" + Environment.NewLine
      + "}" + Environment.NewLine;

  private static readonly string s_nestedEnum = 
      "  public enum %enumname%" + Environment.NewLine
      + "  {" + Environment.NewLine
      + "    DummyEntry = 0" + Environment.NewLine
      + "  }" + Environment.NewLine
      + "" + Environment.NewLine;

  private static readonly string s_propertyHeader = 
      "  %accessibility% %propertytype% %propertyname%" + Environment.NewLine
      + "  {" + Environment.NewLine;
  private static readonly string s_propertyFooter =
      "  }" + Environment.NewLine
      + "" + Environment.NewLine;

  #endregion

  // member fields

  private string _filename = null;
  private TextWriter _classWriter;
  private bool _fileHeaderWritten = false;
  private string _lastNamespaceWritten = string.Empty;

  // construction and disposing

  protected CodeBuilder (string filename)
	{
    ArgumentUtility.CheckNotNull ("filename", filename);

    _filename = filename;
  }
  
  // methods and properties

  protected string FileName
  {
    get { return _filename; }
    set { _filename = value;}
  }

  protected void OpenFile ()
  {
    if (_filename != null)
      _classWriter = new StreamWriter (_filename);
  }

  protected void CloseFile ()
  {
    if (_lastNamespaceWritten != string.Empty)
      EndNamespace ();

    _classWriter.Close ();
  }

  protected void Write (string text)
  {
    _classWriter.Write (text);
  }

  protected void WriteNewLine ()
  {
    _classWriter.Write (Environment.NewLine);
  }

  protected void WriteComment (string comment)
  {
    string commentLine = s_comment;

    commentLine = BuilderUtility.ReplaceTag (commentLine, s_commentTag, comment);

    _classWriter.Write (commentLine);
  }

  protected void BeginFile ()
  {
    if (!_fileHeaderWritten)
    {
      _classWriter.Write (s_fileHeader);
      _fileHeaderWritten = true;
    }
  }

  protected void BeginNamespace (string namespaceName)
  {
    BeginFile ();
    if (_lastNamespaceWritten != namespaceName)
    {
      if (_lastNamespaceWritten != string.Empty)
        EndNamespace ();

      _classWriter.Write (BuilderUtility.ReplaceTag (s_namespaceHeader, s_namespaceTag, namespaceName));
      _lastNamespaceWritten = namespaceName;
    }
  }

  protected void EndNamespace ()
  {
    _classWriter.Write (s_namespaceFooter);
    _lastNamespaceWritten = string.Empty;
  }

  protected void BeginClass (string className, string baseClassName)
  {
    string classHeader = s_classHeader;
    classHeader = BuilderUtility.ReplaceTag (classHeader, s_classnameTag, className);
    classHeader = BuilderUtility.ReplaceTag (classHeader, s_baseClassnameTag, baseClassName);

    _classWriter.Write (classHeader);
  }

  protected void EndClass ()
  {
    _classWriter.Write (s_classFooter);
  }

  protected void BeginMethod (string accessibility, string returnType, string methodName, string parameterlist)
  {
    ArgumentUtility.CheckNotNull ("accessibility", accessibility);
    ArgumentUtility.CheckNotNull ("returnType", returnType);
    ArgumentUtility.CheckNotNull ("methodName", methodName);
    ArgumentUtility.CheckNotNull ("parameterlist", parameterlist);

    string constructor = s_methodHeader;
    constructor = BuilderUtility.ReplaceTag (constructor, s_accessibilityTag, accessibility);
    constructor = BuilderUtility.ReplaceTag (constructor, s_returntypeTag, returnType);
    constructor = BuilderUtility.ReplaceTag (constructor, s_methodnameTag, methodName);
    constructor = BuilderUtility.ReplaceTag (constructor, s_parameterlistTag, parameterlist);

    _classWriter.Write (constructor);
  }

  protected void EndMethod ()
  {
    _classWriter.Write (s_methodFooter);
  }

  protected void BeginIndexer (string accessibility, string returntypename, string parameter)
  {
    ArgumentUtility.CheckNotNull ("accessibility", accessibility);
    ArgumentUtility.CheckNotNull ("returntypename", returntypename);
    ArgumentUtility.CheckNotNull ("parameter", parameter);

    string constructor = s_indexerHeader;
    constructor = BuilderUtility.ReplaceTag (constructor, s_accessibilityTag, accessibility);
    constructor = BuilderUtility.ReplaceTag (constructor, s_returntypeTag, returntypename);
    constructor = BuilderUtility.ReplaceTag (constructor, s_parameterlistTag, parameter);

    _classWriter.Write (constructor);
  }

  protected void EndIndexer ()
  {
    _classWriter.Write (s_indexerFooter);
  }

  protected void BeginConstructor (string className)
  {
    BeginConstructor (s_accessibilityDefault, className, "", null);
  }

  protected void BeginConstructor (string className, string parameterlist)
  {
    BeginConstructor (s_accessibilityDefault, className, parameterlist, null);
  }

  protected void BeginConstructor (string className, string parameterlist, string baseParameterlist)
  {
    BeginConstructor (s_accessibilityDefault, className, parameterlist, baseParameterlist);
  }

  protected void BeginConstructor (string accessibility, string className, string parameterlist, string baseParameterlist)
  {
    ArgumentUtility.CheckNotNull ("className", className);
    ArgumentUtility.CheckNotNull ("parameterlist", parameterlist);

    string constructor = s_constructorHeader;
    constructor = BuilderUtility.ReplaceTag (constructor, s_accessibilityTag, accessibility);
    constructor = BuilderUtility.ReplaceTag (constructor, s_classnameTag, className);
    constructor = BuilderUtility.ReplaceTag (constructor, s_parameterlistTag, parameterlist);
    if (baseParameterlist != null)
    {
      string baseConstructorCall = BuilderUtility.ReplaceTag (s_baseConstructorCall, s_parameterlistTag, baseParameterlist);
      constructor = BuilderUtility.ReplaceTag (constructor, s_baseConstructorTag, baseConstructorCall);
    }
    else
    {
      constructor = BuilderUtility.ReplaceTag (constructor, s_baseConstructorTag, "");
    }

    _classWriter.Write (constructor);
  }

  protected void EndConstructor ()
  {
    _classWriter.Write (s_constructorFooter);
  }

  protected void WriteEnum (string enumName)
  {
    _classWriter.Write (BuilderUtility.ReplaceTag (s_enum, s_enumTag, enumName));
  }

  protected void WriteNestedEnum (string enumName)
  {
    _classWriter.Write (BuilderUtility.ReplaceTag (s_nestedEnum, s_enumTag, enumName));
  }

  protected void BeginProperty (string propertyName, string propertyType)
  {
    BeginProperty (s_accessibilityDefault, propertyName, propertyType);
  }

  protected void BeginProperty (string accessibility, string propertyName, string propertyType)
  {
    string property = s_propertyHeader;
    property = BuilderUtility.ReplaceTag (property, s_accessibilityTag, accessibility);
    property = BuilderUtility.ReplaceTag (property, s_propertynameTag, propertyName);
    property = BuilderUtility.ReplaceTag (property, s_propertytypeTag, propertyType);

    _classWriter.Write (property);
  }

  protected void EndProperty ()
  {
    _classWriter.Write (s_propertyFooter);
  }

  protected PropertyDefinition[] GetEnumPropertyDefinitionsWithNestedType (Type outerType)
  {
    ArrayList nestedTypes = new ArrayList ();
    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (propertyDefinition.PropertyType.IsEnum && propertyDefinition.PropertyType.DeclaringType == outerType)
          nestedTypes.Add (propertyDefinition);
      }
    }
    return (PropertyDefinition[]) nestedTypes.ToArray (typeof(PropertyDefinition));
  }

  #region IBuilder Members

  public abstract void Build ();

  #endregion
}
}

using System;
using System.Collections;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainObjectCollectionBuilder : CodeFileBuilder
{
  // types

  // static members and constants

  public static readonly string DefaultBaseClass = "DomainObjectCollection";

  #region templates

  private static readonly string s_getObjectHeader = 
      "  public static new %classname% GetObject (%parameterlist%)" + Environment.NewLine
      + "  {" + Environment.NewLine;
  private static readonly string s_getObjectContent =
      "    return (%classname%) DomainObject.GetObject (%parameterlist%);" + Environment.NewLine;
  private static readonly string s_getObjectFooter =
      "  }" + Environment.NewLine
      + Environment.NewLine;

  private static readonly string s_indexerGetStatement = 
      "    get { return (%returntype%) base[%parameterlist%]; }" + Environment.NewLine;
  private static readonly string s_indexerSetStatement = 
      "    set { base[%parameterlist%] = value; }" + Environment.NewLine;

  #endregion

  // member fields

  private Type _type;
  private string _namespacename;
  private string _classname;
  private string _requiredItemTypeName;
  private string _baseClass;

  // construction and disposing

	public DomainObjectCollectionBuilder (string filename, Type type, string requiredItemTypeName, string baseClass)
      : base (filename)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("requiredTypename", requiredItemTypeName);
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    _type = type;
    _namespacename = type.Namespace;
    _classname = type.Name;
    _requiredItemTypeName = requiredItemTypeName;

    _baseClass = baseClass;
	}

  // methods and properties

  public override void Build ()
  {
    OpenFile ();

    BeginNamespace (_namespacename);

    BeginClass (_classname, _baseClass);

    // types
    WriteComment ("types");
    WriteLine ();

    //Write nested types (enums)
    foreach (PropertyDefinition propertyDefinition in GetEnumPropertyDefinitionsWithNestedType (_type))
      WriteNestedEnum (propertyDefinition.PropertyType.Name);

    // static members and constants
    WriteComment ("static members and constants");
    WriteLine ();

    // member fields
    WriteComment ("member fields");
    WriteLine ();

    // construction and disposing
    WriteComment ("construction and disposing");
    WriteLine ();

    WriteConstructor (_classname, _requiredItemTypeName);

    // methods and properties
    WriteComment ("methods and properties");
    WriteLine ();

    WriteIndexer (_classname, _requiredItemTypeName, "int index", "index");
    WriteIndexer (_classname, _requiredItemTypeName, "ObjectID id", "id", true);

    EndClass ();
  
    EndNamespace ();

    CloseFile ();
  }

  protected void WriteConstructor (string className, string requiredTypename)
  {
    BeginConstructor (className, "", "typeof (" + requiredTypename + ")");
    EndConstructor ();
  }
 
  protected void WriteIndexer (string classname, string requiredTypename, string parameter, string baseParameter)
  {
    WriteIndexer (classname, requiredTypename, parameter, baseParameter, false);
  }

  protected void WriteIndexer (string classname, string requiredTypename, string parameter, string baseParameter, bool noSetter)
  {
    BeginIndexer (s_accessibilityDefault + " " + s_accessibilityNew, requiredTypename, parameter);
    WriteIndexerGetStatement (requiredTypename, baseParameter);
    if (!noSetter)
      WriteIndexerSetStatement (baseParameter);
    EndIndexer ();
  }

  protected void WriteIndexerGetStatement (string requiredTypename, string baseParameter)
  {
    string output = s_indexerGetStatement;
    output = ReplaceTag (output, s_returntypeTag, requiredTypename);
    output = ReplaceTag (output, s_parameterlistTag, baseParameter);

    Write (output);
  }

  protected void WriteIndexerSetStatement (string baseParameter)
  {
    string output = s_indexerSetStatement;
    output = ReplaceTag (output, s_parameterlistTag, baseParameter);

    Write (output);
  }
}
}

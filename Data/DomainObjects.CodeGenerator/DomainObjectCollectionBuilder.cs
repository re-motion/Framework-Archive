using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainObjectCollectionBuilder : CodeBuilder, IBuilder
{
  // types

  // static members and constants

  private static readonly string s_defaultBaseClass = "DomainObjectCollection";

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

    _type = type;
    _namespacename = type.Namespace;
    _classname = type.Name;
    _requiredItemTypeName = requiredItemTypeName;

    if (baseClass != null)
      _baseClass = baseClass;
    else
      _baseClass = s_defaultBaseClass;
	}

  // methods and properties

  public virtual void Build ()
  {
    //TODO: implement this on the DomainModelBuilder
    if (_classname == s_defaultBaseClass)
      return;

    OpenFile ();

    BeginNamespace (_namespacename);

    BeginClass (_classname, _baseClass);

    // types
    WriteComment ("types");
    WriteNewLine ();

    //Write nested types (enums)
    foreach (PropertyDefinition propertyDefinition in BuilderUtility.GetPropertyDefinitionsWithNestedType (_type))
      WriteNestedEnum (propertyDefinition.PropertyType.Name);

    // static members and constants
    WriteComment ("static members and constants");
    WriteNewLine ();

    // member fields
    WriteComment ("member fields");
    WriteNewLine ();

    // construction and disposing
    WriteComment ("construction and disposing");
    WriteNewLine ();

    WriteConstructor (_classname, _requiredItemTypeName);

    // methods and properties
    WriteComment ("methods and properties");
    WriteNewLine ();

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
    BeginIndexer (s_accessibilityDefault, requiredTypename, parameter);
    WriteIndexerGetStatement (requiredTypename, baseParameter);
    if (!noSetter)
      WriteIndexerSetStatement (baseParameter);
    EndIndexer ();
  }

  protected void WriteIndexerGetStatement (string requiredTypename, string baseParameter)
  {
    string output = s_indexerGetStatement;
    output = BuilderUtility.ReplaceTag (output, s_returntypeTag, requiredTypename);
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, baseParameter);

    ClassWriter.Write (output);
  }

  protected void WriteIndexerSetStatement (string baseParameter)
  {
    string output = s_indexerSetStatement;
    output = BuilderUtility.ReplaceTag (output, s_parameterlistTag, baseParameter);

    ClassWriter.Write (output);
  }
}
}

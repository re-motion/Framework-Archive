using System;
using System.IO;
using System.Collections;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;


namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainObjectCollectionBuilder
{
  // types

  private class Builder: CodeFileBuilder
  {
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

    public Builder (TextWriter writer) : base (writer)
    {
    }

    public void Build (Type type, string requiredItemTypeName, string baseClass, bool serializableAttribute)
    {
      string classname = type.Name;
      string namespacename = type.Namespace;
//      _type = type;
//      _namespacename = type.Namespace;
//      _classname = type.Name;
//      _requiredItemTypeName = requiredItemTypeName;
//
//      _baseClass = baseClass;
//      _serializableAttribute = serializableAttribute;

      BeginNamespace (namespacename);

      if (serializableAttribute)
        WriteSerializableAttribute ();

      BeginClass (classname, baseClass);

      // types
      WriteComment ("types");
      WriteLine ();

      //Write nested types (enums)
      foreach (PropertyDefinition propertyDefinition in GetEnumPropertyDefinitionsWithNestedType (type))
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

      WriteConstructor (classname, requiredItemTypeName);

      // methods and properties
      WriteComment ("methods and properties");
      WriteLine ();

      WriteIndexer (classname, requiredItemTypeName, "int index", "index");
      WriteIndexer (classname, requiredItemTypeName, "ObjectID id", "id", true);

      EndClass ();
    
      EndNamespace ();

      FinishFile ();
    }


    private void WriteConstructor (string className, string requiredTypename)
    {
      BeginConstructor (className, "", "typeof (" + requiredTypename + ")");
      EndConstructor ();
    }
   
    private void WriteIndexer (string classname, string requiredTypename, string parameter, string baseParameter)
    {
      WriteIndexer (classname, requiredTypename, parameter, baseParameter, false);
    }

    private void WriteIndexer (string classname, string requiredTypename, string parameter, string baseParameter, bool noSetter)
    {
      BeginIndexer (s_accessibilityDefault + " " + s_accessibilityNew, requiredTypename, parameter);
      WriteIndexerGetStatement (requiredTypename, baseParameter);
      if (!noSetter)
        WriteIndexerSetStatement (baseParameter);
      EndIndexer ();
    }

    private void WriteIndexerGetStatement (string requiredTypename, string baseParameter)
    {
      string output = s_indexerGetStatement;
      output = ReplaceTag (output, s_returntypeTag, requiredTypename);
      output = ReplaceTag (output, s_parameterlistTag, baseParameter);

      Write (output);
    }

    private void WriteIndexerSetStatement (string baseParameter)
    {
      string output = s_indexerSetStatement;
      output = ReplaceTag (output, s_parameterlistTag, baseParameter);

      Write (output);
    }

  }
  // static members and constants

  public const string DefaultBaseClass = "DomainObjectCollection";

  public static void Build (string filename, Type type, string requiredItemTypeName, string baseClass, bool serializableAttribute)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

    using (TextWriter writer = new StreamWriter (filename))
    {
      Build (writer, type, requiredItemTypeName, baseClass, serializableAttribute);
    }
  }

  public static void Build (TextWriter writer, Type type, string requiredItemTypeName, string baseClass, bool serializableAttribute)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("requiredTypename", requiredItemTypeName);
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    Builder builder = new Builder (writer);
    builder.Build (type, requiredItemTypeName, baseClass, serializableAttribute);
  }

  private DomainObjectCollectionBuilder()
  {
  }
}

}

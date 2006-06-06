using System;
using System.IO;
using System.Collections;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;


namespace Rubicon.Data.DomainObjects.CodeGenerator
{
  public class DomainObjectCollectionBuilder : CodeFileBuilder
  {
    // types

    // static members and constants

    public static readonly string DefaultBaseClass = typeof (DomainObjectCollection).Name;
    public static readonly TypeName DefaultBaseTypeName = new TypeName (
        typeof (DomainObjectCollection).FullName, 
        typeof (DomainObjectCollection).Assembly.GetName ().Name);

    #region templates

    private static readonly string s_indexer =
        "  %accessibility%%modifier%%returntype% this[%parameterlist%]\r\n"
        + "  {\r\n"
        + "    get { return (%returntype%) base[%baseparameterlist%]; }\r\n"
        + "%setter%  }\r\n"
        + "\r\n";

    private static readonly string s_indexerSetStatement =
        "    set { base[%baseparameterlist%] = value; }\r\n";

    #endregion

    public static void Build (string filename, TypeName typeName, string requiredItemTypeName, string baseClass, bool serializableAttribute)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

      using (TextWriter writer = new StreamWriter (filename))
      {
        Build (writer, typeName, requiredItemTypeName, baseClass, serializableAttribute);
      }
    }

    public static void Build (TextWriter writer, TypeName typeName, string requiredItemTypeName, string baseClass, bool serializableAttribute)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("requiredTypename", requiredItemTypeName);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      DomainObjectCollectionBuilder builder = new DomainObjectCollectionBuilder (writer);
      builder.Build (typeName, requiredItemTypeName, baseClass, serializableAttribute);
    }

    // member fields

    // construction and disposing

    private DomainObjectCollectionBuilder (TextWriter writer)
      : base (writer)
    {
    }

    // methods and properties

    private void Build (TypeName typeName, string requiredItemTypeName, string baseClass, bool serializableAttribute)
    {
      BeginNamespace (typeName.Namespace);

      if (serializableAttribute)
        WriteSerializableAttribute ();

      BeginClass (typeName.Name, baseClass, false);
      WriteComment ("types");
      WriteLine ();
      WriteComment ("static members and constants");
      WriteLine ();
      WriteComment ("member fields");
      WriteLine ();
      WriteComment ("construction and disposing");
      WriteLine ();

      WriteConstructor (typeName.Name, requiredItemTypeName);

      WriteComment ("methods and properties");
      WriteLine ();

      WriteIndexer (typeName.Name, requiredItemTypeName, "int index", "index", false);
      WriteIndexer (typeName.Name, requiredItemTypeName, "ObjectID id", "id", true);

      EndClass ();
      EndNamespace ();
      FinishFile ();
    }

    private void WriteConstructor (string className, string requiredItemTypeName)
    {
      BeginConstructor (className, "", "typeof (" + requiredItemTypeName + ")");
      EndConstructor ();
    }

    private void WriteIndexer (string classname, string requiredItemTypeName, string parameter, string baseParameter, bool noSetter)
    {
      string indexer = s_indexer;
      indexer = ReplaceTag (indexer, s_accessibilityTag, s_accessibilityPublic);
      indexer = ReplaceTag (indexer, s_modifierTag, s_modifierNew);
      indexer = ReplaceTag (indexer, s_returntypeTag, requiredItemTypeName);
      indexer = ReplaceTag (indexer, s_parameterlistTag, parameter);
      indexer = ReplaceTag (indexer, "%setter%", noSetter ? string.Empty : s_indexerSetStatement);
      indexer = ReplaceTag (indexer, "%baseparameterlist%", baseParameter);
      Write (indexer);
    }
  }
}

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using Rubicon.Data.DomainObjects.ConfigurationLoader;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class StubsBuilder : CodeBuilder
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _mappingDocument;
  private ConfigurationNamespaceManager _namespaceManager;
  private string[] _uris;
  private Regex _typeRegex = new Regex (@"^(?<namespacename>\w+(\.\w+)*)\.(?<typename>\w+)(\+(?<membername>\w*))?, (?<assemblyname>(\w+\.)*\w+)$");

  // construction and disposing

  public StubsBuilder (string stubsfile, string mappingFile) : base (stubsfile)
  {
    _mappingDocument = CreateXmlDocuemt (mappingFile);

    //TODO: improve this with XMLNamespaceManager
    PrefixNamespace[] prefixNamespaces = new PrefixNamespace[1] { PrefixNamespace.MappingNamespace } ;

    _namespaceManager = new ConfigurationNamespaceManager (_mappingDocument, prefixNamespaces);
    _uris = new string[1] { PrefixNamespace.MappingNamespace.Uri } ;
  }

  // methods and properties

  private string[] GetClassStrings ()
  {
    XmlNodeList classTypeNodes = _mappingDocument.SelectNodes (
        _namespaceManager.FormatXPath("{0}:mapping/{0}:classes/{0}:class/{0}:type", _uris), 
        _namespaceManager);

    ArrayList classTypes = new ArrayList ();
    foreach (XmlNode classTypeNode in classTypeNodes)
    {
      if (IsTypeString (classTypeNode.InnerText.Trim ()))
        classTypes.Add (classTypeNode.InnerText.Trim ());
    }
    return (string[]) classTypes.ToArray (typeof (string));
  }

  private string[] GetEnumStrings ()
  {
    XmlNodeList propertyTypeNodes = _mappingDocument.SelectNodes (
        _namespaceManager.FormatXPath("{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:property/{0}:type", _uris), 
        _namespaceManager);

    ArrayList enumTypes = new ArrayList ();
    foreach (XmlNode enumTypeNode in propertyTypeNodes)
    {
      if (GetTypeName (enumTypeNode.InnerText.Trim ()) != string.Empty)
        enumTypes.Add (enumTypeNode.InnerText.Trim ());
    }

    return (string[]) enumTypes.ToArray (typeof (string));
  }

  private string[] GetCollectionStrings ()
  {
    XmlNodeList collectionTypeNodes = _mappingDocument.SelectNodes (
        _namespaceManager.FormatXPath("{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:relationProperty/{0}:collectionType", _uris), 
        _namespaceManager);

    ArrayList collectionTypes = new ArrayList ();
    foreach (XmlNode collectionTypeNode in collectionTypeNodes)
    {
      string typeString = collectionTypeNode.InnerText.Trim ();
      if (IsTypeString (typeString) 
          && GetTypeName(typeString) != DomainObjectCollectionBuilder.DefaultBaseClass 
          && !collectionTypes.Contains (typeString))
      {
        collectionTypes.Add (typeString);
      }
    }

    return (string[]) collectionTypes.ToArray (typeof (string));
  }

  private XmlDocument CreateXmlDocuemt (string fileName)
  {
    XmlDocument mappingDocument;
    XmlTextReader mappingFileReader = null;

    try
    {
      mappingFileReader = new XmlTextReader (fileName);
      mappingDocument = new XmlDocument (new NameTable());
      mappingDocument.Load (mappingFileReader);
      return mappingDocument;
    }
    finally
    {
      if (mappingFileReader != null)
        mappingFileReader.Close();
    }
  }

  private bool IsTypeString (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return (typeMatch.Groups["typename"].Value != string.Empty);
  }
  
  public string GetNamespaceName (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["namespacename"].Value;
  }

  private string GetTypeName (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["typename"].Value;
  }

  private string GetMemberName (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["membername"].Value;
  }

  private string GetAssemblyName (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["assemblyname"].Value;
  }

  private string[] GetNestedTypes (string[] typeStrings, string parentTypeString)
  {
    ArrayList nestedTypeNames = new ArrayList ();

    string parentNamespaceName = GetNamespaceName (parentTypeString);
    string parentClassName = GetTypeName (parentTypeString);

    foreach (string typeString in typeStrings)
    {
      if (parentNamespaceName == GetNamespaceName (typeString)
          && parentClassName == GetTypeName (typeString))
      {
        nestedTypeNames.Add (GetMemberName (typeString));
      }
    }
    return (string[]) nestedTypeNames.ToArray (typeof (string));
  }

  public override void Build()
  {
    string[] classTypeNames = GetClassStrings ();
    string[] enumTypeNames = GetEnumStrings ();
    string[] collectionTypeNames = GetCollectionStrings ();

    OpenFile ();

    foreach (string classTypeName in classTypeNames)
    {
      BeginNamespace (GetNamespaceName (classTypeName));

      WriteComment (classTypeName);
      BeginClass (GetTypeName (classTypeName), "DomainObject");

      foreach (string enumTypeName in GetNestedTypes (enumTypeNames, classTypeName))
        WriteNestedEnum (enumTypeName);

      EndClass ();
    }

    foreach (string enumTypeName in enumTypeNames)
    {
      if (GetMemberName (enumTypeName) == "")
      {
        BeginNamespace (GetNamespaceName (enumTypeName));

        WriteComment (enumTypeName);
        WriteEnum (GetTypeName (enumTypeName));
      }
    }

    foreach (string collectionTypeName in collectionTypeNames)
    {
      BeginNamespace (GetNamespaceName (collectionTypeName));

      WriteComment (collectionTypeName);
      BeginClass (GetTypeName (collectionTypeName), "DomainObjectCollection");

      foreach (string enumTypeName in GetNestedTypes (enumTypeNames, collectionTypeName))
        WriteNestedEnum (enumTypeName);

      EndClass ();
    }
    CloseFile ();
  }
}
}

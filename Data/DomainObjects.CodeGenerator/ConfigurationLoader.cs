using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using Rubicon.Data.DomainObjects.ConfigurationLoader;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class StubsBuilder : CodeBuilder, IBuilder
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _mappingDocument;
  private ConfigurationNamespaceManager _namespaceManager;
  private string[] _uris;
  private Regex _typeRegex = new Regex (@"^(?<namespacename>\w+(\.\w+)*)\.(?<classname>\w+)(\+(?<membername>\w*))?, (?<assemblyname>(\w+\.)*\w+)$");

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
      if (GetClassname (enumTypeNode.InnerText.Trim ()) != string.Empty)
        enumTypes.Add (enumTypeNode.InnerText.Trim ());
    }

    return (string[]) enumTypes.ToArray (typeof (string));
  }

  // TODO: each type should be returned only once!
  // TODO: eliminate DomainObjectCollections
  private string[] GetCollectionStrings ()
  {
    XmlNodeList collectionTypeNodes = _mappingDocument.SelectNodes (
        _namespaceManager.FormatXPath("{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:relationProperty/{0}:collectionType", _uris), 
        _namespaceManager);

    ArrayList collectionTypes = new ArrayList ();
    foreach (XmlNode collectionTypeNode in collectionTypeNodes)
    {
      if (IsTypeString (collectionTypeNode.InnerText.Trim ()))
        collectionTypes.Add (collectionTypeNode.InnerText.Trim ());
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

    return (typeMatch.Groups["classname"].Value != string.Empty);
  }
  
  public string GetNamespacename (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["namespacename"].Value;
  }

  //TODO: rename to GetTypeName
  private string GetClassname (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["classname"].Value;
  }

  private string GetMembername (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["membername"].Value;
  }

  private string GetAssemblyname (string typeString)
  {
    Match typeMatch = _typeRegex.Match (typeString);

    return typeMatch.Groups["assemblyname"].Value;
  }

  private string[] GetNestedTypes (string[] typeStrings, string parentTypeString)
  {
    ArrayList nestedTypeNames = new ArrayList ();

    string parentNamespaceName = GetNamespacename (parentTypeString);
    string parentClassName = GetClassname (parentTypeString);

    foreach (string typeString in typeStrings)
    {
      if (parentNamespaceName == GetNamespacename (typeString)
          && parentClassName == GetClassname (typeString))
      {
        nestedTypeNames.Add (GetMembername (typeString));
      }
    }
    return (string[]) nestedTypeNames.ToArray (typeof (string));
  }

  #region IBuilder Members

  public virtual void Build()
  {
    string[] classTypeNames = GetClassStrings ();
    string[] enumTypeNames = GetEnumStrings ();
    string[] collectionTypeNames = GetCollectionStrings ();

    OpenFile ();

    foreach (string classTypeName in classTypeNames)
    {
      BeginNamespace (GetNamespacename (classTypeName));

      WriteComment (classTypeName);
      BeginClass (GetClassname (classTypeName), "DomainObject");

      BeginConstructor (GetClassname (classTypeName));
      EndConstructor ();

      foreach (string enumTypeName in GetNestedTypes (enumTypeNames, classTypeName))
        WriteNestedEnum (enumTypeName);

      EndClass ();
    }

    foreach (string enumTypeName in enumTypeNames)
    {
      if (GetMembername (enumTypeName) == "")
      {
        BeginNamespace (GetNamespacename (enumTypeName));

        WriteComment (enumTypeName);
        WriteEnum (GetClassname (enumTypeName));
      }
    }

    foreach (string collectionTypeName in collectionTypeNames)
    {
      BeginNamespace (GetNamespacename (collectionTypeName));

      WriteComment (collectionTypeName);
      BeginClass (GetClassname (collectionTypeName), "DomainObjectCollection");

      BeginConstructor (GetClassname (collectionTypeName));
      EndConstructor ();

      foreach (string enumTypeName in GetNestedTypes (enumTypeNames, collectionTypeName))
        WriteNestedEnum (enumTypeName);

      EndClass ();
    }
    CloseFile ();
  }

  #endregion
}
}

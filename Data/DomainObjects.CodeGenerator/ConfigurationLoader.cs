using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{

/// <summary>
///   Provides the configuration for other builders. 
/// </summary>
/// <remarks>
///   Diverts all assembly requests to a stub assembly created for the current configuration. Call <see cref="Unload"/> 
///   after using the configuration, or use the C# <c>using</c> statement.
/// </remarks>
public class ConfigurationLoader: IDisposable
{
  // types
  
  private class StubBuilder: CodeFileBuilder
  {
    private XmlDocument _mappingDocument;
    private XmlNamespaceManager _namespaceManager;
    private Regex _typeRegex = new Regex (@"^(?<namespacename>\w+(\.\w+)*)\.(?<typename>\w+)(\+(?<membername>\w*))?, (?<assemblyname>(\w+\.)*\w+)$");

    public StubBuilder (TextWriter writer): base (writer)
    {
    }

    public void Build (string mappingPath)
    {
      _mappingDocument = CreateXmlDocuemt (Path.Combine (mappingPath, MappingLoader.DefaultConfigurationFile));

      PrefixNamespace[] prefixNamespaces = new PrefixNamespace[1] { PrefixNamespace.MappingNamespace } ;

      _namespaceManager = new XmlNamespaceManager (_mappingDocument.NameTable);
      _namespaceManager.AddNamespace ("m", PrefixNamespace.MappingNamespace.Uri);

      string[] classTypeNames = GetClassStrings ();
      string[] enumTypeNames = GetEnumStrings ();
      string[] collectionTypeNames = GetCollectionStrings ();

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
      FinishFile ();
    }

    private string[] GetClassStrings ()
    {
      XmlNodeList classTypeNodes = _mappingDocument.SelectNodes ("m:mapping/m:classes/m:class/m:type", _namespaceManager);

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
      XmlNodeList propertyTypeNodes = _mappingDocument.SelectNodes ("m:mapping/m:classes/m:class/m:properties/m:property/m:type", _namespaceManager);

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
          "m:mapping/m:classes/m:class/m:properties/m:relationProperty/m:collectionType", _namespaceManager);

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
  }

  private Assembly _stubAssembly;

  public ConfigurationLoader (string xmlFilePath, string xmlSchemaFilePath)
  {
    ArgumentUtility.CheckNotNull ("xmlFilePath", xmlFilePath);
    ArgumentUtility.CheckNotNull ("xmlSchemaFilePath", xmlSchemaFilePath);

    MemoryStream stream = new MemoryStream ();
    TextWriter writer = new StreamWriter (stream);

    StubBuilder stubBuilder = new StubBuilder (writer);
    stubBuilder.Build (xmlFilePath);

    stream.Seek (0, SeekOrigin.Begin);
    StreamReader reader = new StreamReader (stream);

    CSharpCodeProvider provider = new CSharpCodeProvider();
    ICodeCompiler compiler = provider.CreateCompiler ();
    string[] referencedAssemblies = new string[] {
        typeof (Rubicon.Utilities.StringUtility).Assembly.Location, // Rubicon.Core assembly
        typeof (Rubicon.Data.DomainObjects.ClientTransaction).Assembly.Location, // Rubicon.Data.DomainObjects assembly
        typeof (Rubicon.Data.DomainObjects.ObjectBinding.DomainObjectClass).Assembly.Location // Rubicon.Data.DomainObjects.ObjectBinding assembly
    };
    CompilerParameters options = new CompilerParameters (referencedAssemblies);
    options.GenerateInMemory = true;
    CompilerResults results = compiler.CompileAssemblyFromSource (options, reader.ReadToEnd());
    if (results.Errors.Count > 0)
    {
      ArrayList lines = new ArrayList(); // ArrayList<string>
      stream.Position = 0;
      string line;
      while ((line = reader.ReadLine ()) != null)
        lines.Add (line);

      StringBuilder msg = new StringBuilder("compiler errors:", results.Errors.Count * 300);
      foreach (CompilerError error in results.Errors)
      {
        msg.AppendFormat ("\nline {0},{1}: {2}", error.Line, error.Column, lines[error.Line - 1]);
        msg.AppendFormat ("\n{0}: {1}", error.ErrorNumber, error.ErrorText);
      }
      throw new ApplicationException (msg.ToString()); 
    }
    _stubAssembly = results.CompiledAssembly;

    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler (this.CurrentDomain_AssemblyResolve);

    MappingConfiguration.SetCurrent (new MappingConfiguration (
        Path.Combine (xmlFilePath, MappingLoader.DefaultConfigurationFile), 
        Path.Combine (xmlSchemaFilePath, MappingLoader.DefaultSchemaFile),
        true));

    StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (
        Path.Combine (xmlFilePath, StorageProviderConfigurationLoader.DefaultConfigurationFile), 
        Path.Combine (xmlSchemaFilePath, StorageProviderConfigurationLoader.DefaultSchemaFile)));
  }

  private Assembly CurrentDomain_AssemblyResolve (object sender, ResolveEventArgs args)
  {
    return _stubAssembly;
  }

  void IDisposable.Dispose()
  {
    Unload();
  }

  public void Unload()
  {
    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler (this.CurrentDomain_AssemblyResolve);
  }
}

}

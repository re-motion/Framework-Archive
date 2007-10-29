using System;
using System.Configuration;
using System.IO;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
  public class QueryFileElement : ConfigurationElement, INamedConfigurationElement
  {
    private static string GetRootedPath (string path)
    {
      if (Path.IsPathRooted (path))
        return Path.GetFullPath (path);
      else
        return Path.GetFullPath (Path.Combine (ReflectionUtility.GetExecutingAssemblyPath (), path));
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _queryFileNameProperty;
    private readonly ConfigurationProperty _queryFileFilenameProperty;

    public QueryFileElement ()
    {
      _queryFileNameProperty = new ConfigurationProperty (
          "name",
          typeof (string),
          null,
          ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

      _queryFileFilenameProperty = new ConfigurationProperty (
          "filename",
          typeof (string),
          null,
          ConfigurationPropertyOptions.IsRequired);

      _properties.Add (_queryFileNameProperty);
      _properties.Add (_queryFileFilenameProperty);
    }

    public QueryFileElement (string name, string fileName) : this()
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fileName", fileName);

      Name = name;
      FileName = fileName;
    }

    public string Name
    {
      get { return (string) this[_queryFileNameProperty]; }
      protected set { this[_queryFileNameProperty] = value; }
    }

    public string FileName
    {
      get { return (string) this[_queryFileFilenameProperty]; }
      protected set { this[_queryFileFilenameProperty] = value; }
    }

    public string RootedFileName
    {
      get { return GetRootedPath (FileName); }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }
  }
}
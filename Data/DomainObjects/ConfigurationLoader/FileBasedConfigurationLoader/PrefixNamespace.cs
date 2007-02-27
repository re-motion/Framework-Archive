using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader
{
  public class PrefixNamespace
  {
    // types

    // static members and constants

    public static readonly PrefixNamespace QueryConfigurationNamespace = new PrefixNamespace (
        "q", "http://www.rubicon-it.com/Data/DomainObjects/Queries/1.0");

    // member fields

    private string _prefix;
    private string _uri;

    // construction and disposing

    public PrefixNamespace (string prefix, string uri)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("prefix", prefix);
      ArgumentUtility.CheckNotNullOrEmpty ("uri", uri);

      _prefix = prefix;
      _uri = uri;
    }

    // methods and properties

    public string Prefix
    {
      get { return _prefix; }
    }

    public string Uri
    {
      get { return _uri; }
    }
  }
}

using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IIdentity"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullIdentity : IIdentity
  {
    public string Name
    {
      get { return string.Empty; }
    }

    public string AuthenticationType
    {
      get { return string.Empty; }
    }

    public bool IsAuthenticated
    {
      get { return false; }
    }
  }
}
using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IUserProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullUserProvider : IUserProvider
  {
    private NullPrincipal _principal = new NullPrincipal();

    public NullUserProvider()
    {
    }

    public IPrincipal GetUser()
    {
      return _principal;
    }

    bool INullableObject.IsNull
    {
      get { return true; }
    }
  }
}
using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface IUserProvider : INullableObject
  {
    IPrincipal GetUser();
  }
}
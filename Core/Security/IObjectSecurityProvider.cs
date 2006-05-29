using System;
namespace Rubicon.Security
{
  public interface IObjectSecurityProvider : ISecurityProvider
  {
    bool HasAccessOnGetAccessor (ISecurableObject securableObject);
    bool HasAccessOnSetAccessor (ISecurableObject securableObject);
  }
}

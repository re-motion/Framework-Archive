using System;
namespace Rubicon.Security
{
  public interface IObjectSecurityProvider : ISecurityProvider
  {
    bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName);
    bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName);
  }
}

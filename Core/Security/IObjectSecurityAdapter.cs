using System;
namespace Rubicon.Security
{
  [Obsolete ("Use IObjectSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IObjectSecurityProvider : IObjectSecurityAdapter, ISecurityProviderObsolete
  {
  }

  public interface IObjectSecurityAdapter : ISecurityAdapter
  {
    bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName);
    bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName);
  }
}

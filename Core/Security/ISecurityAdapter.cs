using System;
namespace Rubicon.Security
{
  [Obsolete ("Use ISecurityAdapter instead. (Version: 1.7.41)")]
  public interface ISecurityProviderObsolete : ISecurityAdapter
  {
  }

//marker interface, verwendet in typeparam von secadaapterregistry.
  public interface ISecurityAdapter
  {
  }
}

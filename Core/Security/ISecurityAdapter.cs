using System;
namespace Rubicon.Security
{
  [Obsolete ("Use ISecurityAdapter instead. (Version: 1.7.41)")]
  public interface ISecurityProvider : ISecurityAdapter
  {
  }

  public interface ISecurityAdapter
  {
  }
}

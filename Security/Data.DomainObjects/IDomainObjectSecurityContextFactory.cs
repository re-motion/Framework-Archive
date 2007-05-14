using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Data.DomainObjects
{
  public interface IDomainObjectSecurityContextFactory : ISecurityContextFactory
  {
    bool IsDiscarded { get; }
    bool IsNew { get; }
    bool IsDeleted { get; }
  }
}
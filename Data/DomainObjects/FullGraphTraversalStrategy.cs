using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects
{
  public sealed class FullGraphTraversalStrategy : IGraphTraversalStrategy
  {
    public static readonly  FullGraphTraversalStrategy Instance = new FullGraphTraversalStrategy();

    private FullGraphTraversalStrategy ()
    {
    }

    public bool IncludeObject (DomainObject domainObject)
    {
      return true;
    }

    public bool FollowLink (DomainObject currentObject, PropertyAccessor linkProperty)
    {
      return true;
    }
  }
}
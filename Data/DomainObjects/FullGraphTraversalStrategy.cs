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

    public bool ShouldProcessObject (DomainObject domainObject)
    {
      return true;
    }

    public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
    {
      return true;
    }
  }
}
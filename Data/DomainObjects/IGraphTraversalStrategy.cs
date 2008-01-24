using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Defines a strategy to use when traversing a <see cref="DomainObject"/> graph using a <see cref="DomainObjectGraphTraverser"/>.
  /// </summary>
  public interface IGraphTraversalStrategy
  {
    /// <summary>
    /// Determines whether to process the given object in the result set when traversing a <see cref="DomainObject"/> graph.
    /// </summary>
    /// <param name="domainObject">The domain object to decide about.</param>
    /// <returns>True if the object should be processed; otherwise, false.</returns>
    /// <remarks>The question of processing has no effect on the question whether the object's links should be followed.</remarks>
    bool ShouldProcessObject (DomainObject domainObject);
    /// <summary>
    /// Determines whether to follow a relation link when traversing a <see cref="DomainObject"/> graph.
    /// </summary>
    /// <param name="currentObject">The current domain object defining the relation link.</param>
    /// <param name="linkProperty">The link property. Note that when the property's <see cref="PropertyAccessor.GetValue{T}"/> methods are
    /// accessed, this can cause the related objects to be loaded from the database.</param>
    /// <returns>True if the traverser should follow the link; otherwise, if traversal should stop at the <paramref name="currentObject"/>.</returns>
    bool ShouldFollowLink (DomainObject currentObject, PropertyAccessor linkProperty);
  }
}
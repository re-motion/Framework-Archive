using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Allows a mixin applied to a <see cref="DomainObject"/> to react on events related to the <see cref="DomainObject"/> instance.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implement this interface on a mixin applied to a <see cref="DomainObject"/> to be informed about when the <see cref="DomainObject"/> instance
  /// is retrieved via <see cref="DomainObject.NewObject{T}"/> or <see cref="DomainObject.GetObject{T}(ObjectID)"/>.
  /// </para>
  /// <para>
  /// The hook methods defined on
  /// this interface are called by the <see cref="DomainObjects"/> infrastructure at points of time when it is safe to access the domain object's
  /// ID and properties. Use them instead of <see cref="Mixin{TThis}.OnInitialized"/> to execute mixin initialization code that must access
  /// the domain object's properties.
  /// </para>
  /// </remarks>
  public interface IDomainObjectMixin
  {
    /// <summary>
    /// Called when the mixin's target domain object has been created.
    /// </summary>
    void OnDomainObjectCreated ();
    
    /// <summary>
    /// Called when the mixin's target domain object has been loaded.
    /// </summary>
    void OnDomainObjectLoaded ();
  }
}
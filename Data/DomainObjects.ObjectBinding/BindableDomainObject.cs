using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Provides a base class for bindable <see cref="DomainObject"/> classes.
  /// </summary>
  /// <remarks>
  /// Deriving from this base class is equivalent to deriving from the <see cref="DomainObject"/> class and applying the
  /// <see cref="BindableDomainObjectAttribute"/> to the derived class.
  /// </remarks>
  [BindableDomainObject]
  public abstract class BindableDomainObject : DomainObject
  {
    /// <summary>
    /// Provides a possibility to override the display name of the bindable domain object.
    /// </summary>
    /// <value>The display name.</value>
    /// <remarks>Override this property to replace the default display name provided by the <see cref="BindableObjectClass"/> with a custom one.
    /// </remarks>
    [OverrideMixinMember]
    [StorageClassNone]
    public virtual string DisplayName
    {
      get { return Mixin.Get<BindableDomainObjectMixin> (this).BaseDisplayName; }
    }
  }
}

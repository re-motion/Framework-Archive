using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class BindableDomainObjectWithOverriddenDisplayName : BindableSampleDomainObject
  {
    public new static BindableDomainObjectWithOverriddenDisplayName NewObject ()
    {
      return DomainObject.NewObject<BindableDomainObjectWithOverriddenDisplayName> ().With ();
    }

    public static new BindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BindableDomainObjectWithOverriddenDisplayName> (id);
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}

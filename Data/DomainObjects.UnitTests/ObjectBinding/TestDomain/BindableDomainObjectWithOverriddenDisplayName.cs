using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class BindableDomainObjectWithOverriddenDisplayName : BindableDomainObject
  {
    public static BindableDomainObjectWithOverriddenDisplayName NewObject ()
    {
      return DomainObject.NewObject<BindableDomainObjectWithOverriddenDisplayName> ().With ();
    }

    public static BindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BindableDomainObjectWithOverriddenDisplayName> (id);
    }

    [Override]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}

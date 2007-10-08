using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [BindableDomainObject]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class BindableSampleDomainObject : DomainObject
  {
    public static BindableSampleDomainObject NewObject ()
    {
      return DomainObject.NewObject<BindableSampleDomainObject> ().With ();
    }

    public new static BindableSampleDomainObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BindableSampleDomainObject> (id);
    }

    public abstract string Name { get; set; }
  }
}

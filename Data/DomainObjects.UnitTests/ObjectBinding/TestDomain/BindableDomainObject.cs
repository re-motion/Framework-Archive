using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [BindableDomainObject]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class BindableDomainObject : DomainObject
  {
    public static BindableDomainObject NewObject ()
    {
      return DomainObject.NewObject<BindableDomainObject> ().With ();
    }

    public static BindableDomainObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BindableDomainObject> (id);
    }

    public abstract string Name { get; set; }
  }
}

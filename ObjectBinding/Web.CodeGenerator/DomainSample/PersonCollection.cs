using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace DomainSample
{
public class PersonCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PersonCollection () : base (typeof (Person))
  {
  }

  // methods and properties

  public new Person this[int index]
  {
    get { return (Person) base[index]; }
    set { base[index] = value; }
  }

  public new Person this[ObjectID id]
  {
    get { return (Person) base[id]; }
  }

}
}

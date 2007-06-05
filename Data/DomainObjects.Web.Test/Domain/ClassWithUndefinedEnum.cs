using System;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable ("TableWithUndefinedEnum")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassWithUndefinedEnum: BindableDomainObject
  {
    public static ClassWithUndefinedEnum NewObject ()
    {
      return DomainObject.NewObject<ClassWithUndefinedEnum> ().With ();
    }

    public new static ClassWithUndefinedEnum GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClassWithUndefinedEnum> (id);
    }

    protected ClassWithUndefinedEnum()
    {
    }

    public abstract UndefinedEnum UndefinedEnum { get; set; }
  }
}
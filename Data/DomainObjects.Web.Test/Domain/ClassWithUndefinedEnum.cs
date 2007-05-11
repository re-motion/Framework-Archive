using System;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable ("TableWithUndefinedEnum")]
  [Instantiable]
  [RpaTest]
  public abstract class ClassWithUndefinedEnum: BindableDomainObject
  {
    public static ClassWithUndefinedEnum NewObject ()
    {
      return DomainObject.NewObject<ClassWithUndefinedEnum> ().With ();
    }

    protected ClassWithUndefinedEnum()
    {
    }

    protected ClassWithUndefinedEnum (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract UndefinedEnum UndefinedEnum { get; set; }
  }
}
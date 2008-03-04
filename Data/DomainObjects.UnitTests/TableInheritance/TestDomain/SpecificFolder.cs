using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_SpecificFolder")]
  [Instantiable]
  public abstract class SpecificFolder : Folder
  {
    public static SpecificFolder NewObject()
    {
      return NewObject<SpecificFolder> ().With ();
    }

    public static SpecificFolder GetObject (ObjectID id)
    {
      return DomainObject.GetObject<SpecificFolder> (id);
    }

    protected SpecificFolder ()
    {
    }
  }
}
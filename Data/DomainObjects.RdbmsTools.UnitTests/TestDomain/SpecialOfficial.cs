using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class SpecialOfficial : Official
  {
    public static new SpecialOfficial NewObject ()
    {
      return NewObject<SpecialOfficial>().With();
    }

    protected SpecialOfficial()
    {
    }

    protected SpecialOfficial (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Speciality { get; set;}
  }
}

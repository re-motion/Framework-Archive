using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinAddingUnidirectionalRelation1 : DomainObjectMixin<DomainObject>
  {
    public Computer Computer
    {
      get { return Properties[typeof (MixinAddingUnidirectionalRelation1), "Computer"].GetValue<Computer>(); }
      set { Properties[typeof (MixinAddingUnidirectionalRelation1), "Computer"].SetValue (value); }
    }
  }
}
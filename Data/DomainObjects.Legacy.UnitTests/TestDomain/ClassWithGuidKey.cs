using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithGuidKey : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithGuidKey GetObject (ObjectID id)
    {
      return (ClassWithGuidKey) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithGuidKey ()
    {
    }

    protected ClassWithGuidKey (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public ClassWithValidRelations ClassWithValidRelationsOptional
    {
      get
      {
        return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsOptional");
      }
      set
      {
        SetRelatedObject ("ClassWithValidRelationsOptional", value);
      }
    }

    public ClassWithValidRelations ClassWithValidRelationsNonOptional
    {
      get
      {
        return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsNonOptional");
      }
      set
      {
        SetRelatedObject ("ClassWithValidRelationsNonOptional", value);
      }
    }

    public ClassWithInvalidRelation ClassWithInvalidRelation
    {
      get
      {
        return (ClassWithInvalidRelation) GetRelatedObject ("ClassWithInvalidRelation");
      }
      set
      {
        SetRelatedObject ("ClassWithInvalidRelation", value);
      }
    }

    public ClassWithRelatedClassIDColumnAndNoInheritance ClassWithRelatedClassIDColumnAndNoInheritance
    {
      get
      {
        return (ClassWithRelatedClassIDColumnAndNoInheritance) GetRelatedObject ("ClassWithRelatedClassIDColumnAndNoInheritance");
      }
      set
      {
        SetRelatedObject ("ClassWithRelatedClassIDColumnAndNoInheritance", value);
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidProperties : DomainObject
  {
    protected ClassWithInvalidProperties ()
    {
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException (); }
    }

    [StringProperty]
    private ClassWithInvalidProperties PropertyWithStringAttribute
    {
      get { throw new NotImplementedException (); }
    }

    [BinaryProperty]
    private ClassWithInvalidProperties PropertyWithBinaryAttribute
    {
      get { throw new NotImplementedException (); }
    }
  }
}
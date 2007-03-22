using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  // Assists the DomainObject factory methods in creating domain objects.
  interface IDomainObjectCreator
  {
    DomainObject CreateWithCurrentTransaction (Type type);
    DomainObject CreateWithTransaction (Type type, ClientTransaction clientTransaction);
    DomainObject CreateWithDataContainer (DataContainer dataContainer, ObjectID objectID);
  }
}

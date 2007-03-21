using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects
{
  interface IDomainObjectCreator
  {
    DomainObject CreateWithCurrentTransaction (Type type);
    DomainObject CreateWithTransaction (Type type, ClientTransaction clientTransaction);
    DomainObject CreateWithDataContainer (DataContainer dataContainer, ObjectID objectID);
  }
}

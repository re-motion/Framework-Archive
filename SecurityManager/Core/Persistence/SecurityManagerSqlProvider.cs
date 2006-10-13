using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.SecurityManager.Persistence
{
  public class SecurityManagerSqlProvider : SqlProvider
  {
    // constants

    // types

    // static members

    // member fields

    private RevisionStorageProviderExtension _revisionExtension;

    // construction and disposing

    public SecurityManagerSqlProvider (RdbmsProviderDefinition definition) 
      : base (definition)
    {
      _revisionExtension = new RevisionStorageProviderExtension ();
    }

    // methods and properties

    public override void Save (DataContainerCollection dataContainers)
    {
      _revisionExtension.Saving (Connection, Transaction , dataContainers);
      base.Save (dataContainers);
    }
  }
}
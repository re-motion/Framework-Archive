using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public class OrganizationalStructureObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public OrganizationalStructureObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected OrganizationalStructureObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}

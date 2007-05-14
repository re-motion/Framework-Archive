using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.Data;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [PermanentGuid ("8DBA42FE-ECD9-4b10-8F79-48E7A1119414")]
  public class OrganizationalStructureObject : BaseSecurityManagerObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    // types

    // static members and constants

    // member fields

    private DomainObjectSecurityStrategy _securityStrategy;

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

    protected virtual string GetOwningClient ()
    {
      return null;
    }

    protected virtual string GetOwner ()
    {
      return null;
    }

    protected virtual string GetOwningGroup ()
    {
      return null;
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum> ();
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum> ();
    }

    SecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      using (new SecurityFreeSection ())
      {
        return new SecurityContext (GetType (), GetOwner (), GetOwningGroup (), GetOwningClient (), GetStates (), GetAbstractRoles ());
      }
    }

    bool IDomainObjectSecurityContextFactory.IsDiscarded
    {
      get { return IsDiscarded; }
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State == StateType.New; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State == StateType.Deleted; }
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, this);

      return _securityStrategy;
    }
  }
}

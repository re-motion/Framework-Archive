using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Tenant")]
  [PermanentGuid ("BD8FB1A4-E300-4663-AB1E-D6BD7B106619")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Tenant : OrganizationalStructureObject
  {
    // constants

    // types

    public enum Methods
    {
      Search
    }

    // static members

    private static readonly string s_currentKey = typeof (Tenant).AssemblyQualifiedName + "_Current";

    public static Tenant Current
    {
      get { return (Tenant) CallContext.GetData (s_currentKey); }
      set { CallContext.SetData (s_currentKey, value); }
    }

    internal static Tenant NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<Tenant> ().With ();
      }
    }

    public static new Tenant GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<Tenant> (id);
      }
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Tenant.FindAll");
      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    public static Tenant FindByUnqiueIdentifier (string uniqueIdentifier, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Tenant.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection tenants = clientTransaction.QueryManager.GetCollection (query);
      if (tenants.Count == 0)
        return null;

      return (Tenant) tenants[0];
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    // member fields

    // construction and disposing

    protected Tenant ()
    {
      UniqueIdentifier = Guid.NewGuid ().ToString ();
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    public abstract bool IsAbstract { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract Tenant Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<Tenant> Children { get; }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override string GetOwningTenant ()
    {
      return UniqueIdentifier;
    }

    // TODO: UnitTests
    public List<Tenant> GetPossibleParentTenants (ObjectID tenantID)
    {
      List<Tenant> clients = new List<Tenant> ();

      foreach (Tenant tenant in FindAll (ClientTransaction))
      {
        if ((!Children.Contains (tenant.ID)) && (tenant.ID != this.ID))
          clients.Add (tenant);
      }
      return clients;
    }

    public ObjectList<Tenant> GetHierachy ()
    {
      ObjectList<Tenant> tenants = new ObjectList<Tenant> ();
      tenants.Add (this);
      foreach (Tenant tenant in Children)
        tenants.Combine (tenant.GetHierachy ());

      return tenants;
    }
  }
}

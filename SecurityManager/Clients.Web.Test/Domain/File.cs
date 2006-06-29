using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Data;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("BAA77408-32E6-4979-9914-8A12B71808F2")]
  public class File : BaseSecurableObject
  {
    // types

    // static members and constants

    public static new File GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (File) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public File (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected File (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    [PermanentGuid ("4B073E2B-C56D-419c-8358-808FDEF669EF")]
    public Confidentiality Confidentiality
    {
      get { return (Confidentiality) DataContainer["Confidentiality"]; }
      set { DataContainer["Confidentiality"] = value; }
    }

    public User Owner
    {
      get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.User) GetRelatedObject ("Owner"); }
      set { SetRelatedObject ("Owner", value); }
    }

    public User Clerk
    {
      get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.User) GetRelatedObject ("Clerk"); }
      set { SetRelatedObject ("Clerk", value); }
    }

    public Group Group
    {
      get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.Group) GetRelatedObject ("Group"); }
      set { SetRelatedObject ("Group", value); }
    }

    public DomainObjectCollection Files
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("Files"); }
      set { } // marks property Files as modifiable
    }

    public override User GetOwner ()
    {
      return Clerk;
    }

    public override Group GetOwnerGroup ()
    {
      if (Clerk == null)
        return null;
      return Clerk.Group;
    }

    public override Client GetOwnerClient ()
    {
      return Client;
    }
  }
}

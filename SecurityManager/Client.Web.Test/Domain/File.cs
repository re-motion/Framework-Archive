using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Client.Web.Test.Domain
{
public class File : BindableDomainObject
{
  // types

  // static members and constants

  public static new File GetObject (ObjectID id)
  {
    return (File) DomainObject.GetObject (id);
  }

  public static new File GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (File) DomainObject.GetObject (id, clientTransaction);
  }

  // member fields

  // construction and disposing

  public File ()
  {
  }

  public File (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected File (DataContainer dataContainer) : base (dataContainer)
  {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
  }

  // methods and properties

  public string Name
  {
    get { return (string) DataContainer["Name"]; }
    set { DataContainer["Name"] = value; }
  }

  public Confidentiality Confidentiality
  {
    get { return (Confidentiality) DataContainer["Confidentiality"]; }
    set { DataContainer["Confidentiality"] = value; }
  }

  public Rubicon.SecurityManager.Domain.OrganizationalStructure.User Owner
  {
    get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.User) GetRelatedObject ("Owner"); }
    set { SetRelatedObject ("Owner", value); }
  }

  public Rubicon.SecurityManager.Domain.OrganizationalStructure.User Clerk
  {
    get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.User) GetRelatedObject ("Clerk"); }
    set { SetRelatedObject ("Clerk", value); }
  }

  public Rubicon.SecurityManager.Domain.OrganizationalStructure.Group Group
  {
    get { return (Rubicon.SecurityManager.Domain.OrganizationalStructure.Group) GetRelatedObject ("Group"); }
    set { SetRelatedObject ("Group", value); }
  }

  public Rubicon.Data.DomainObjects.DomainObjectCollection Files
  {
    get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("Files"); }
    set { } // marks property Files as modifiable
  }

}
}

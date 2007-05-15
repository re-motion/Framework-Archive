using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("DF0A8DB4-943C-4bd1-8B3B-276C8AA16BDB")]
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class FileItem : BaseSecurableObject
  {
    public static FileItem NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<FileItem> ().With ();
      }
    }
    
    public static new FileItem GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (FileItem) DomainObject.GetObject (id, clientTransaction);
    }

    protected FileItem ()
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract File File { get; set; }

    public override User GetOwner ()
    {
      if (File == null)
        return null;
      return File.GetOwner();
    }

    public override Group GetOwnerGroup ()
    {
      if (File == null)
        return null;
      return File.GetOwnerGroup ();
    }

    public override Client GetOwnerClient ()
    {
      if (File == null)
        return null;
      return File.GetOwnerClient ();
    }
  }
}

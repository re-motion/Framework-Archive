using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Client.Web.Test.Domain
{
public class FileItem : BindableDomainObject
{
  // types

  // static members and constants

  public static new FileItem GetObject (ObjectID id)
  {
    return (FileItem) DomainObject.GetObject (id);
  }

  public static new FileItem GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (FileItem) DomainObject.GetObject (id, clientTransaction);
  }

  // member fields

  // construction and disposing

  public FileItem ()
  {
  }

  public FileItem (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected FileItem (DataContainer dataContainer) : base (dataContainer)
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

  public File File
  {
    get { return (File) GetRelatedObject ("File"); }
    set { SetRelatedObject ("File", value); }
  }

}
}

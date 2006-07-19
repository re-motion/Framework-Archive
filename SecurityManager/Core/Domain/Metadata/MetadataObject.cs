using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using System.Globalization;
using System.Threading;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public abstract class MetadataObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static MetadataObject Find (ClientTransaction transaction, string metadataID)
    {
      FindMetadataObjectQueryBuilder queryBuilder = new FindMetadataObjectQueryBuilder ();
      DomainObjectCollection metadataObjects = transaction.QueryManager.GetCollection (queryBuilder.CreateQuery (metadataID));
      if (metadataObjects.Count == 0)
        return null;

      return (MetadataObject) metadataObjects[0];
    }

    public static new MetadataObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (MetadataObject) DomainObject.GetObject (id, clientTransaction);
    }

    public static new MetadataObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (MetadataObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public MetadataObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected MetadataObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public int Index
    {
      get { return (int) DataContainer["Index"]; }
      set { DataContainer["Index"] = value; }
    }

    public virtual Guid MetadataItemID
    {
      get { return (Guid) DataContainer["MetadataItemID"]; }
      set { DataContainer["MetadataItemID"] = value; }
    }

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public DomainObjectCollection LocalizedNames
    {
      get { return GetRelatedObjects ("LocalizedNames"); }
    }

    //TODO: Rewrite with Test
    public override string DisplayName
    {
      get
      {
        LocalizedName localizedName = GetLocalizedName (CultureInfo.CurrentUICulture.Name);
        if (localizedName != null)
          return localizedName.Text;

        localizedName = GetLocalizedName (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        if (localizedName != null)
          return localizedName.Text;

        localizedName = GetLocalizedName (CultureInfo.InvariantCulture.Name);
        if (localizedName != null)
          return localizedName.Text;

        return Name;
      }
    }

    public LocalizedName GetLocalizedName (Culture culture)
    {
      return GetLocalizedName (culture.CultureName);
    }

    public LocalizedName GetLocalizedName (string cultureName)
    {
      foreach (LocalizedName localizedName in LocalizedNames)
      {
        if (localizedName.Culture.CultureName == cultureName)
          return localizedName;
      }

      return null;
    }
  }
}

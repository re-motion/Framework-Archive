using System;
using System.Collections.Generic;
using System.Globalization;
using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public abstract class MetadataObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static MetadataObject Find (string metadataID, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      
      FindMetadataObjectQueryBuilder queryBuilder = new FindMetadataObjectQueryBuilder ();
      DomainObjectCollection metadataObjects = clientTransaction.QueryManager.GetCollection (queryBuilder.CreateQuery (metadataID));
      if (metadataObjects.Count == 0)
        return null;

      return (MetadataObject) metadataObjects[0];
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

    public override string DisplayName
    {
      get
      {
        foreach (CultureInfo cultureInfo in GetCultureHierachy (CultureInfo.CurrentUICulture))
        {
          LocalizedName localizedName = GetLocalizedName (cultureInfo.Name);
          if (localizedName != null)
            return localizedName.Text;
        }

        return Name;
      }
    }

    public LocalizedName GetLocalizedName (Culture culture)
    {
      ArgumentUtility.CheckNotNull ("culture", culture);

      return GetLocalizedName (culture.CultureName);
    }

    public LocalizedName GetLocalizedName (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);

      foreach (LocalizedName localizedName in LocalizedNames)
      {
        if (localizedName.Culture.CultureName.Equals (cultureName, StringComparison.Ordinal))
          return localizedName;
      }

      return null;
    }

    private List<CultureInfo> GetCultureHierachy (CultureInfo cultureInfo)
    {
      List<CultureInfo> cultureHierarchy = new List<CultureInfo> ();

      cultureHierarchy.Add (cultureInfo);
      while (cultureInfo != cultureInfo.Parent) // Invariant culture is its own parent
      {
        cultureInfo = cultureInfo.Parent;
        cultureHierarchy.Add (cultureInfo);
      }

      return cultureHierarchy;
    }
  }
}

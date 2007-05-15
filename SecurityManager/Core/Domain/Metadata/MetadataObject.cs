using System;
using System.Collections.Generic;
using System.Globalization;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [SecurityManagerStorageGroup]
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

    protected MetadataObject ()
    {
    }

    // methods and properties

    public abstract int Index { get; set; }

    public abstract Guid MetadataItemID { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 200)]
    public abstract string Name { get; set; }

    [IsReadOnly]
    [DBBidirectionalRelation ("MetadataObject")]
    public abstract ObjectList<LocalizedName> LocalizedNames { get; }

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

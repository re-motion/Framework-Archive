using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class LocalizedName : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static new LocalizedName GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (LocalizedName) DomainObject.GetObject (id, clientTransaction);
    }

    public static new LocalizedName GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (LocalizedName) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public LocalizedName (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public LocalizedName (ClientTransaction clientTransaction, string text, Culture culture, MetadataObject metadataObject)
      : base (clientTransaction)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("culture", culture);
      ArgumentUtility.CheckNotNull ("metadataObject", metadataObject);

      DataContainer["Text"] = text;
      SetRelatedObject ("Culture", culture);
      SetRelatedObject ("MetadataObject", metadataObject);
    }

    protected LocalizedName (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string Text
    {
      get { return (string) DataContainer["Text"]; }
    }

    public Culture Culture
    {
      get { return (Culture) GetRelatedObject ("Culture"); }
    }

    public MetadataObject MetadataObject
    {
      get { return (MetadataObject) GetRelatedObject ("MetadataObject"); }
    }
  }
}

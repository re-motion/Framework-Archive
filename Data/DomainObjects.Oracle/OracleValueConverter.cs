using System;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Oracle
{
  public class OracleValueConverter: ValueConverter
  {
    public override object GetDBValue (ObjectID id, string storageProviderID)
    {
      return ConvertDBValue (base.GetDBValue (id, storageProviderID));
    }

    public override object GetDBValue (object value)
    {
      return ConvertDBValue (base.GetDBValue (value));
    }

    private object ConvertDBValue (object value)
    {
      if (value is Guid)
        return ((Guid)value).ToByteArray();
      else if (value is bool)
        return (bool) value ? 1 : 0;
      return value;
    }

    public override ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
    {
      if (dataValue is byte[])
        return base.GetObjectID (classDefinition, new Guid ((byte[]) dataValue));
      else
        return base.GetObjectID (classDefinition, dataValue);
    }

    public override object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
    {
      if (dataValue != null)
      {
        if (propertyDefinition.PropertyType == typeof (Guid))
        {
          byte[] binaryGuid = ArgumentUtility.CheckType<byte[]> ("dataValue", dataValue);
          dataValue = new Guid (binaryGuid);
        }
        else if (propertyDefinition.PropertyType == typeof (bool))
        {
          Int16 boolAsInt = ArgumentUtility.CheckNotNullAndValueType<Int16> ("dataValue", dataValue);
          dataValue = (boolAsInt != 0);
        }
      }
      return base.GetValue (classDefinition, propertyDefinition, dataValue);
    }
  }
}

using System;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  [Serializable]
  public abstract class PropertyDefinition: ISerializable, IObjectReference
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinition _classDefinition;
    private string _propertyName;
    private string _storageSpecificName;
    private int? _maxLength;
    private bool _isPersistent;

    // Note: _mappingClassID is used only during the deserialization process. 
    // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
    private string _mappingClassID;

    // construction and disposing

    protected PropertyDefinition (string propertyName, string columnName, int? maxLength, bool isPersistent)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      _propertyName = propertyName;
      _storageSpecificName = columnName;
      _maxLength = maxLength;
      _isPersistent = isPersistent;
    }

    // methods and properties

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    [Obsolete ("Use StorageSpecificName instead. (Version 1.7.42)")]
    public string ColumnName
    {
      get { return StorageSpecificName; }
    }

    public string StorageSpecificName
    {
      get
      {
        if (!_isPersistent)
          throw new InvalidOperationException ("Cannot access property 'StorageSpecificName' for non-persistent property definitions.");
        return _storageSpecificName;
      }
    }

    public abstract string MappingTypeName { get; }

    public abstract Type PropertyType { get; }

    public abstract bool IsPropertyTypeResolved
    {
      get; }

    public abstract bool IsNullable
    {
      get; }

    public int? MaxLength
    {
      get { return _maxLength; }
    }

    public abstract object DefaultValue { get; }

    public bool IsPersistent
    {
      get { return _isPersistent; }
    }

    /// <summary>
    /// IsPartOfMappingConfiguration is used only during the deserialization process. 
    /// It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
    /// </summary>
    protected bool IsPartOfMappingConfiguration
    {
      get { return _mappingClassID != null; }
    }

    public void SetClassDefinition (ClassDefinition classDefinition)
    {
      _classDefinition = classDefinition;
    }

    #region ISerializable Members

    protected PropertyDefinition (SerializationInfo info, StreamingContext context)
    {
      _propertyName = info.GetString ("PropertyName");
      bool ispartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

      if (ispartOfMappingConfiguration)
      {
        // Note: If this object was part of MappingConfiguration.Current during the serialization process,
        // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
        // Therefore only the information needed in IObjectReference.GetRealObject is deserialized here.
        _mappingClassID = info.GetString ("MappingClassID");
      }
      else
      {
        _classDefinition = (ClassDefinition) info.GetValue ("ClassDefinition", typeof (ClassDefinition));
        _storageSpecificName = info.GetString ("StorageSpecificName");

        _maxLength = (int?) info.GetValue ("MaxLength", typeof (int?));
        _isPersistent = info.GetBoolean ("IsPersistent");
      }
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("PropertyName", _propertyName);

      bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
      info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

      if (isPartOfMappingConfiguration)
      {
        // Note: If this object is part of MappingConfiguration.Current during the serialization process,
        // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
        // Therefore only the information needed in IObjectReference.GetRealObject is serialized here.
        info.AddValue ("MappingClassID", _classDefinition.ID);
      }
      else
      {
        GetObjectData (info, context);
      }
    }

    protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("ClassDefinition", _classDefinition);
      info.AddValue ("StorageSpecificName", _storageSpecificName);
      info.AddValue ("MaxLength", _maxLength);
      info.AddValue ("IsPersistent", _isPersistent);
    }

    #endregion

    #region IObjectReference Members

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      // Note: A PropertyDefinition must know its ClassDefinition to correctly deserialize itself and a 
      // ClassDefinition knows its PropertyDefintions. For bi-directional relationships
      // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
      // Therefore the member _classDefinition cannot be used here, because it could point to the wrong instance. 
      if (_mappingClassID != null)
        return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_mappingClassID)[_propertyName];
      else
        return this;
    }

    #endregion
  }
}
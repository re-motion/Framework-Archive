// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}")]
  public class PropertyDefinition
  {
    private readonly string _propertyName;
    private readonly ClassDefinition _classDefinition;
    private readonly int? _maxLength;
    private readonly StorageClass _storageClass;
    private IStoragePropertyDefinition _storagePropertyDefinition;
    private readonly IPropertyInformation _propertyInfo;
    private readonly Type _propertyType;
    private readonly bool _isNullable;
    private readonly bool _isObjectID;

    public PropertyDefinition (
        ClassDefinition classDefinition, 
        IPropertyInformation propertyInfo, 
        string propertyName, 
        Type propertyType, 
        bool isObjectID,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      _classDefinition = classDefinition;
      _propertyInfo = propertyInfo;
      _propertyType = propertyType;
      _propertyName = propertyName;
      _isObjectID = isObjectID;
      _isNullable = isNullable;
      _maxLength = maxLength;
      _storageClass = storageClass;
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public IStoragePropertyDefinition StoragePropertyDefinition
    {
      get
      {
        if (StorageClass != StorageClass.Persistent)
          throw new InvalidOperationException ("Cannot access property 'storagePropertyDefinition' for non-persistent property definitions.");

        return _storagePropertyDefinition;
      }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public object DefaultValue
    {
      get
      {
        if (_isNullable)
          return null;

        if (_propertyType.IsArray)
          return Array.CreateInstance (_propertyType.GetElementType (), 0);

        if (_propertyType == typeof (string))
          return string.Empty;

        if (_propertyType.IsEnum)
          return EnumUtility.GetEnumMetadata (_propertyType).OrderedValues[0];

        if (ExtensibleEnumUtility.IsExtensibleEnumType (_propertyType))
          return ExtensibleEnumUtility.GetDefinition (_propertyType).GetValueInfos ().First ().Value;

        return Activator.CreateInstance (_propertyType, new object[0]);
      }
    }

    public bool IsNullable
    {
      get { return _isNullable; }
    }

    public bool IsObjectID
    {
      get { return _isObjectID; }
    }

    public int? MaxLength
    {
      get { return _maxLength; }
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }

    public void SetStorageProperty (IStoragePropertyDefinition storagePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("storagePropertyDefinition", storagePropertyDefinition);

      _storagePropertyDefinition = storagePropertyDefinition;
    }

    public override string ToString ()
    {
      return GetType ().FullName + ": " + _propertyName;
    }
  }
}

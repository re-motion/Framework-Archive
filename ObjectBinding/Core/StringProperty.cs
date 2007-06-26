using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  //TODO: doc and check existing doc
  public class StringProperty : PropertyBase, IBusinessObjectStringProperty
  {
    private readonly int? _maxLength;

    public StringProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isRequired, int? maxLength)
        : base (propertyInfo, itemType, isList, isRequired)
    {
      _maxLength = maxLength;
    }

    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    public int? MaxLength
    {
      get { return _maxLength; }
    }
  }
}
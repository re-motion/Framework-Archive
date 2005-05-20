using System;
using System.Text;
using System.Collections;
using System.Xml.Serialization;
using System.Globalization;

namespace Rubicon.ObjectBinding
{

/// <summary> The abstract default implementation of the <see cref="IBusinessObject"/> interface. </summary>
public abstract class BusinessObject: IBusinessObject
{
  /// <summary> 
  ///   Gets the string representation of the value accessed through the specified <see cref="IBusinessObjectProperty"/> 
  ///   from the the passed <see cref="IBusinessObject"/>.
  /// </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> whose property value will be returned. </param>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="format"> The format string passed to the value's <b>ToString</b> method. </param>
  /// <returns> 
  ///   The string representation of the property value for the <paramref name="property"/> parameter.  
  /// <list type="table">
  ///   <listheader>
  ///     <term> Property and Property Value </term>
  ///     <description> Return Value </description>
  ///   </listheader>
  ///   <item>
  ///     <term> The value is an empty list or <see langword="null"/> </term>
  ///     <description> <see cref="String.Empty"/> is returned. </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a list and the first list item implements the <see cref="IBusinessObjectWithIdentity"/> 
  ///       interface. 
  ///     </term>
  ///     <description> 
  ///       The first item's <see cref="IBusinessObjectWithIdentity.DisplayName"/> is returned. If there is more than 
  ///       one item in the list, the total number of list items is appended to the returned string. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> The value is a scalar and implements the <see cref="IBusinessObjectWithIdentity"/> interface. </term>
  ///     <description> The value's <see cref="IBusinessObjectWithIdentity.DisplayName"/> is returned.  </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a list and the <paramref name="property"/> parameter implements the 
  ///       <see cref="IBusinessObjectBooleanProperty"/> interface. 
  ///     </term>
  ///     <description> 
  ///       <see cref="IBusinessObjectBooleanProperty.GetDisplayName"/> is evaluated for the first item and the 
  ///       resulting string is returned. If there is more than one item in the list, the total number of list items 
  ///       is appended to the returned string. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a scalar and the <paramref name="property"/> parameter implements the
  ///       <see cref="IBusinessObjectBooleanProperty"/> interface. 
  ///     </term>
  ///     <description> 
  ///       <see cref="IBusinessObjectBooleanProperty.GetDisplayName"/> is evaluated for the value and the resulting 
  ///       string is returned.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a list and the <paramref name="property"/> parameter implements the 
  ///       <see cref="IBusinessObjectEnumerationProperty"/> interface.
  ///     </term>
  ///     <description> 
  ///       The first item's <see cref="IEnumerationValueInfo.DisplayName"/> is returned. If there is more than one 
  ///       item in the list, the total number of list items is appended to the returned string. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a scalar and the <paramref name="property"/> parameter implements the 
  ///       <see cref="IBusinessObjectEnumerationProperty"/> interface. 
  ///     </term>
  ///     <description> The value's <see cref="IEnumerationValueInfo.DisplayName"/> is returned. </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a list, the first item implements the <see cref="IFormattable"/> interface,
  ///       and a format string is provided using the <paramref name="format"/> parameter. 
  ///     </term>
  ///     <description> 
  ///       The first item is formatted using the format string and is then returned. If there is more than one item in 
  ///       the list, the total number of list items is appended to the returned string. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> 
  ///       The value is a scalar, implements the <see cref="IFormattable"/> interface, 
  ///       and a format string is provided using the <paramref name="format"/> parameter. 
  ///     </term>
  ///     <description> The value is formatted using the format string and is then returned. </description>
  ///   </item>
  ///   <item>
  ///     <term> The value is a list. </term>
  ///     <description> 
  ///       The first item's <b>ToString</b> method is executed and the resulting string is returned. If there is more 
  ///       than one item in the list, the total number of list items is appended to the returned string. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term> The value is a scalar. </term>
  ///     <description> The value's <b>ToString</b> method is executed and the resulting string is returned.  </description>
  ///   </item>
  /// </list>
  /// </returns>
  /// <remarks> 
  ///     Uses the <paramref name="obj"/>'s <see cref="IBusinessObject.GetProperty"/> method for accessing the value.
  /// </remarks>
  /// <exception cref="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of <paramref name="obj"/>'s class. 
  /// </exception>
  public static string GetPropertyString (IBusinessObject obj, IBusinessObjectProperty property, string format)
  {
    int count = 0;

    if (property.IsList)
    {
      // parse "lines=<n>" from format string, where n = no. of lines (integer > 0) or "all" (-1 internally)
      int lines = 1;
      if (format != null && format.StartsWith ("lines="))
      {
        string strLines = format.Substring ("lines=".Length);
        if (strLines == "all")
        {
          lines = -1;
        }
        else if (strLines.Length > 0)
        {
          double dblLines;
          if (double.TryParse (strLines, NumberStyles.Integer, CultureInfo.InvariantCulture, out dblLines) && dblLines > 0)
            lines = (int) dblLines;
        }
      }

      IList list = (IList) obj.GetProperty (property);
      if (list == null)
        return string.Empty;

      count = list.Count;
      StringBuilder sb = new StringBuilder (count * 40);
      for (int i = 0; 
            i < count && (lines == -1 || i < lines); 
            ++i)
      {
        if (i > 0)
          sb.Append ("\r\n");
        sb.Append (GetStringValue (list[i], property, format));
      }

      if (lines != -1 && count > lines)
        sb.Append (" ... [" + count.ToString() + "]");

      return sb.ToString();
    }
    else
    {
      return GetStringValue (obj.GetProperty (property), property, format);
    }
  }

  private static string GetStringValue (object value, IBusinessObjectProperty property, string format)
  {
    if (value == null)
      return string.Empty;

    string strValue;
    IBusinessObjectWithIdentity businessObject = value as IBusinessObjectWithIdentity;
    if (businessObject != null)
    {
      strValue = businessObject.DisplayName;
    }
    else if (property is IBusinessObjectBooleanProperty)
    {
      if (value is bool)
        strValue = ((IBusinessObjectBooleanProperty) property).GetDisplayName ((bool)value);
      else
        strValue = string.Empty;
    }
    else if (property is IBusinessObjectEnumerationProperty)
    {
      IBusinessObjectEnumerationProperty enumProperty = (IBusinessObjectEnumerationProperty) property;
      IEnumerationValueInfo enumValueInfo = enumProperty.GetValueInfoByValue (value);
      strValue = enumValueInfo.DisplayName;
    }
    else if (format != null && value is IFormattable)
    {
      strValue = ((IFormattable) value).ToString (format, null);
    }
    else
    {
      strValue = value.ToString();
    }

    return strValue;
  }

  /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
  /// <param name="propertyIdentifier"> 
  ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
  ///   business object.
  /// </param>
  /// <returns> Returns the <see cref="IBusinessObjectProperty"/> or <see langword="null"/>. </returns>
  /// <remarks> 
  ///   <para>
  ///     It is not specified wheter an exception is thrown or <see langword="null"/> is returned if the 
  ///     <see cref="IBusinessObjectProperty"/> could not be found.
  ///   </para><para>
  ///     The default implementation uses the <see cref="BusinessObjectClass"/>'s 
  ///     <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for finding the 
  ///     <see cref="IBusinessObjectProperty"/>.
  ///   </para>
  /// </remarks>
  public virtual IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  /// <overloads> Gets the value accessed through the specified property. </overloads>
  /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  public abstract object GetProperty (IBusinessObjectProperty property);
  
  /// <summary>
  ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  public object GetProperty (string propertyIdentifier)
  {
    return GetProperty (GetBusinessObjectProperty (propertyIdentifier));
  }

  /// <overloads> Sets the value accessed through the specified property. </overloads>
  /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  public abstract void SetProperty (IBusinessObjectProperty property, object value);

  /// <summary>
  ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  public void SetProperty (string propertyIdentifier, object value)
  {
    SetProperty (GetBusinessObjectProperty (propertyIdentifier), value);
  }

  /// <overloads> Gets or sets the value accessed through the specified property. </overloads>
  /// <summary> Gets or sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  public object this [IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  /// <summary> 
  ///   Gets or Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  public object this [string propertyIdentifier]
  {
    get { return GetProperty (propertyIdentifier); }
    set { SetProperty (propertyIdentifier, value); }
  }

  /// <overloads> Gets the string representation of the value accessed through the specified property.  </overloads>
  /// <summary> 
  ///   Gets the string representation of the value accessed through the specified 
  ///   <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  /// <summary> 
  ///   Gets the formatted string representation of the value accessed through the specified 
  ///   <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  public virtual string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return GetPropertyString (this, property, format);
  }

  /// <summary> 
  ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
  ///   identified by the passed <paramref name="propertyIdentifier"/>.
  /// </summary>
  public string GetPropertyString (string propertyIdentifier)
  {
    return GetPropertyString (GetBusinessObjectProperty (propertyIdentifier));
  }

  /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
  [XmlIgnore] // Not to be serialized.
  public abstract IBusinessObjectClass BusinessObjectClass { get; }
}

}

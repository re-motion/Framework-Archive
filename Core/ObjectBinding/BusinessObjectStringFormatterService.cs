using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public class BusinessObjectStringFormatterService
  {
    /// <summary> 
    ///   Gets the string representation of the value accessed through the specified <see cref="IBusinessObjectProperty"/> 
    ///   from the the passed <see cref="IBusinessObject"/>.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose property value will be returned. </param>
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
    ///     Uses the <paramref name="businessObject"/>'s <see cref="IBusinessObject.GetProperty"/> method for accessing the value.
    /// </remarks>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of <paramref name="businessObject"/>'s class. 
    /// </exception>
    public string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format)
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

        IList list = (IList) businessObject.GetProperty (property);
        if (list == null)
          return string.Empty;

        count = list.Count;
        StringBuilder sb = new StringBuilder (count*40);
        for (int i = 0;
             i < count && (lines == -1 || i < lines);
             ++i)
        {
          if (i > 0)
            sb.Append ("\r\n");
          sb.Append (GetStringValue (businessObject, list[i], property, format));
        }

        if (lines != -1 && count > lines)
          sb.Append (" ... [" + count.ToString() + "]");

        return sb.ToString();
      }
      else
      {
        return GetStringValue (businessObject, businessObject.GetProperty (property), property, format);
      }
    }

    private string GetStringValue (IBusinessObject businessObject, object value, IBusinessObjectProperty property, string format)
    {
      if (property is IBusinessObjectBooleanProperty)
        return GetStringValueForBooleanProperty (value, (IBusinessObjectBooleanProperty) property);
      if (property is IBusinessObjectDateTimeProperty)
        return GetStringValueForDateTimeProperty (value, (IBusinessObjectDateTimeProperty) property, format);
      if (property is IBusinessObjectEnumerationProperty)
        return GetStringValueForEnumerationProperty (value, (IBusinessObjectEnumerationProperty) property, businessObject);
      if (property is IBusinessObjectNumericProperty)
        return GetStringValueForNumericProperty (value, (IBusinessObjectNumericProperty) property, format);
      if (property is IBusinessObjectReferenceProperty)
        return GetStringValueForReferenceProperty (value, (IBusinessObjectReferenceProperty) property);
      if (property is IBusinessObjectStringProperty)
        return GetStringValueForStringProperty (value, (IBusinessObjectStringProperty) property);

      return (value != null) ? value.ToString() : string.Empty;
    }

    private string GetStringValueForDateTimeProperty (object value, IBusinessObjectDateTimeProperty property, string format)
    {
      if (value == null)
        return string.Empty;
      return ((IFormattable) value).ToString (format, null);
    }

    private string GetStringValueForBooleanProperty (object value, IBusinessObjectBooleanProperty property)
    {
      if (value is bool)
        return property.GetDisplayName ((bool) value);
      else
        return string.Empty;
    }

    private string GetStringValueForEnumerationProperty (object value, IBusinessObjectEnumerationProperty property, IBusinessObject businessObject)
    {
      IEnumerationValueInfo enumValueInfo = property.GetValueInfoByValue (value, businessObject);
      if (enumValueInfo == null)
        return string.Empty;
      return enumValueInfo.DisplayName;
    }

    private string GetStringValueForNumericProperty (object value, IBusinessObjectNumericProperty property, string format)
    {
      if (value == null)
        return string.Empty;
      return ((IFormattable) value).ToString (format, null);
    }

    private string GetStringValueForReferenceProperty (object value, IBusinessObjectReferenceProperty property)
    {
      if (value == null)
        return string.Empty;
      return ((IBusinessObject) value).DisplayName;
    }

    private string GetStringValueForStringProperty (object value, IBusinessObjectStringProperty property)
    {
      return StringUtility.NullToEmpty ((string) value);
    }
  }
}
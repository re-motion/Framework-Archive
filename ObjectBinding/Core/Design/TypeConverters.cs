using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Remotion.ObjectBinding.Design
{

/// <summary>
/// Converts the name of an IBusinessObjectDataSource to an object reference. Used by the GUI designer.
/// </summary>
[Obsolete ("Only required when configuring a component in the VS.NET ASP.NET Designer.")]
public class BusinessObjectDataSourceObjectConverter: StringObjectConverter
{
  public override bool IsConvertibleObject (IComponent objectToConvert)
  {
    return objectToConvert is IBusinessObjectDataSource;
  }
}

/// <summary>
/// Converts the name of an object to an object reference. Used by the GUI designer.
/// </summary>
[Obsolete ("Only required when configuring a component in the VS.NET ASP.NET Designer.")]
public abstract class StringObjectConverter: TypeConverter
{
  public abstract bool IsConvertibleObject (IComponent objectToConvert);

  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    return sourceType == typeof (string);
  }

  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value == null)
      return string.Empty;

    if (value is string)
      return value;

    throw this.GetConvertFromException (value);
  }

  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return true;
  }

  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  public override StandardValuesCollection GetStandardValues (ITypeDescriptorContext context)
  {
    object[] result = null;
    if (context != null)
    {
      ArrayList standardValues = new ArrayList();
      IContainer container = context.Container;
      if (container != null)
      {
        ComponentCollection components = container.Components;
        foreach (object compObj in components)
        {
          IComponent component = compObj as IComponent;
          if (component == null || ! IsConvertibleObject (component) || System.Runtime.InteropServices.Marshal.IsComObject (component))
            continue;

          PropertyDescriptor modifiers = TypeDescriptor.GetProperties (component)["Modifiers"];
          if (modifiers != null)
          {
            MemberAttributes attributes = (MemberAttributes) modifiers.GetValue (component);
            if ((attributes & MemberAttributes.AccessMask) == MemberAttributes.Private)
              continue;
          }

          if (component.Site == null || component.Site.Name == null)
            continue;

          IGetComponentBindingExpression getBindingExpression = component as IGetComponentBindingExpression;
          if (getBindingExpression != null)
            standardValues.Add (getBindingExpression.BindingExpression);
          else
            standardValues.Add (component.Site.Name);
        }
      }
      result = standardValues.ToArray ();
      Array.Sort (result);
    }
    return new StandardValuesCollection (result);
  }
}


}

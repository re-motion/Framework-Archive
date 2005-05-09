using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Design
{
public class BusinessObjectDataSourceControlConverter : StringConverter
{
  public BusinessObjectDataSourceControlConverter()
  {
  }

  private object[] GetDataSourceControls (IBusinessObjectBoundWebControl control, IContainer container)
  {
    ComponentCollection components = container.Components;
    ArrayList dataSources = new ArrayList();

    for (int i = 0; i < components.Count; i++)
    {
      IComponent component = (IComponent) components[i];
      IBusinessObjectDataSourceControl dataSource = component as IBusinessObjectDataSourceControl;
      if (   dataSource != null 
          && ! StringUtility.IsNullOrEmpty (dataSource.ID)
          && dataSource != control)
      {
        dataSources.Add (dataSource.ID);
      }
    }

    dataSources.Sort(Comparer.Default);
    return dataSources.ToArray();
  }

  public override TypeConverter.StandardValuesCollection GetStandardValues (ITypeDescriptorContext context)
  {
    if ((context != null) && (context.Container != null))
    {
      object[] dataSources = GetDataSourceControls (
          (IBusinessObjectBoundWebControl) context.Instance, 
          context.Container);
      if (dataSources != null)
        return new TypeConverter.StandardValuesCollection (dataSources);
    }
    return null;
  }

  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return true;
  }
}
}

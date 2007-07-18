using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Design
{
  public class BindableObjectDataSourceDesigner: BocDataSourceDesigner
  {
    public override string GetDesignTimeHtml()
    {
      BindableObjectDataSourceControl dataSourceControl = (BindableObjectDataSourceControl) Component;

      //Exception designTimeException = dataSourceControl.GetDesignTimeException();
      //if (designTimeException != null)
      //  return CreateErrorDesignTimeHtml (designTimeException.Message, designTimeException.InnerException);
      return CreatePlaceHolderDesignTimeHtml();
    }

    public override void Initialize (System.ComponentModel.IComponent component)
    {
      base.Initialize (component);

      IPropertyValueUIService propertyValueUIService = (IPropertyValueUIService) component.Site.GetService (typeof (IPropertyValueUIService));
      propertyValueUIService.AddPropertyValueUIHandler (PropertyValueUIHandler);
    }

    private void PropertyValueUIHandler (ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList)
    {
      if (propDesc.DisplayName == "TypeName")
      {
        string value = propDesc.GetValue (Component) as string;
        if (string.IsNullOrEmpty (value))
          return;
        if (TypeUtility.GetDesignModeType (value, Component.Site, false) != null)
          return;

        Image image = new Bitmap (9, 9);
        using (Graphics g = Graphics.FromImage (image))
        {
          g.FillEllipse (Brushes.Red, 0, 0, 9, 9);
        }
        valueUIItemList.Add (new PropertyValueUIItem (image, delegate { }, "Error"));
      }
    }
  }
}
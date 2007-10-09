using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.Design.BindableObject;
using Rubicon.Mixins.Context;
using TypeUtility=Rubicon.Utilities.TypeUtility;

namespace Rubicon.ObjectBinding.Web.UI.Design
{
  public class BindableObjectDataSourceDesigner : BocDataSourceDesigner
  {
    public BindableObjectDataSourceDesigner ()
    {
    }

    public override string GetDesignTimeHtml ()
    {
      return CreatePlaceHolderDesignTimeHtml();
    }

    public override void Initialize (System.ComponentModel.IComponent component)
    {
      base.Initialize (component);

      BindableObjectTypeFinder typeFinder = new BindableObjectTypeFinder (component.Site);
      ApplicationContext mixinConfiguration = typeFinder.GetApplicationContext (false);
      MixinConfiguration.SetActiveContext (mixinConfiguration);

      IPropertyValueUIService propertyValueUIService = (IPropertyValueUIService) component.Site.GetService (typeof (IPropertyValueUIService));
      propertyValueUIService.AddPropertyValueUIHandler (PropertyValueUIHandler);
    }

    private void PropertyValueUIHandler (ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList)
    {
      if (propDesc.DisplayName == "TypeName")
      {
        string value = propDesc.GetValue (Component) as string;
        if (!IsValidTypeName (value))
        {
          Image image = new Bitmap (8, 8);
          using (Graphics g = Graphics.FromImage (image))
          {
            g.FillEllipse (Brushes.Red, 0, 0, 8, 8);
          }
          valueUIItemList.Add (
              new PropertyValueUIItem (
                  image, delegate { }, string.Format ("Could not load type '{0}'.", TypeUtility.ParseAbbreviatedTypeName (value))));
        }
      }
    }

    private bool IsValidTypeName (string value)
    {
      if (string.IsNullOrEmpty (value))
        return true;
      if (TypeUtility.GetDesignModeType (value, Component.Site, false) == null)
        return false;
      return true;
    }
  }
}
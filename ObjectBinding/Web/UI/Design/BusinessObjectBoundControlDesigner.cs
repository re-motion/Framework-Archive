using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI.Design;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

/// <summary>
/// Provides a desinger for the DataSource property of BusinessObject-bound controls.
/// </summary>
[Obsolete ("Place a IBusinessObjectDataSourceControl onto the page instead and link it to the BusinessObjectBoundWebControl.")]
public class BusinessObjectBoundControlDesigner: ControlDesigner
{
  private IBusinessObjectBoundControl _boundControl;

  public override void Initialize (IComponent component)
  {
    _boundControl = (IBusinessObjectBoundControl) component;
    base.Initialize (component);
  }

  /// <summary>
  /// Modifies the type information of the DataSource property for design time.
  /// </summary>
  /// <param name="properties"></param>
  protected override void PreFilterProperties (IDictionary properties)
  {
    base.PreFilterProperties (properties);

    // find the original DataSource property definition
    PropertyDescriptor prop = (PropertyDescriptor) properties["DataSource"];
    System.ComponentModel.AttributeCollection attrColl = prop.Attributes;

    // add attribute [TypeConverter (typeof (BusinessObjectDataSourceObjectConverter))]
    Attribute[] attrs = new Attribute [attrColl.Count + 1];
    attrColl.CopyTo (attrs, 0);
    attrs[attrColl.Count] = new TypeConverterAttribute (typeof (BusinessObjectDataSourceObjectConverter));

    // replace the old property definition, register this type for string conversions using its DataSource string property
    prop = TypeDescriptor.CreateProperty (this.GetType(), "DataSource", typeof (string), attrs);
    properties["DataSource"] = prop;
  }

  /// <summary>
  /// Converts the DataSource property to and from data-binding string values at design time.
  /// </summary>
  public string DataSource
  {
    get 
    { 
      string result = string.Empty;;
      System.Web.UI.DataBinding dataBinding = DataBindings["DataSource"];
      if (dataBinding != null)
        result = dataBinding.Expression;
      return result;
    }
    set 
    { 
      if (value == null || value == string.Empty)
      {
        DataBindings.Remove ("DataSource");
      }
      else
      {
        System.Web.UI.DataBinding dataBinding = DataBindings["DataSource"];
        if (dataBinding == null)
          dataBinding = new System.Web.UI.DataBinding ("DataSource", typeof (IBusinessObjectDataSource), value);
        else
          dataBinding.Expression = value;
        DataBindings.Add (dataBinding);
        this.OnBindingsCollectionChanged ("DataSource");
      }
    }
  }

  public override bool DesignTimeHtmlRequiresLoadComplete
  {
    get { return true; }
  }

}

}

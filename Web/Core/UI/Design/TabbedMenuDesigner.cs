using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI.Design;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{
public class TabStripMenuDesigner: WebControlDesigner, IServiceProvider
{
  private DesignerVerbCollection _verbs = null;

	public TabStripMenuDesigner()
	{
    _verbs = new DesignerVerbCollection();
    _verbs.Add (new DesignerVerb ("Edit Menu Tabs", new EventHandler(OnVerbEditFixedColumns)));
  }

  private void OnVerbEditFixedColumns (object sender, EventArgs e) 
  {
    TabStripMenu tabStripMenu = Component as TabStripMenu;
    if (tabStripMenu == null)
      throw new InvalidOperationException ("Cannot use TabStripMenuDesigner for objects other than TabStripMenu.");

    PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties (tabStripMenu);
    PropertyDescriptor propertyDescriptor = propertyDescriptors["Tabs"];

    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    TabStripMainMenuItemCollectionEditor editor = null;
    //Does not work because EditorAttribute is applied on property
    //editor = (TabStripMainMenuItemCollectionEditor) TypeDescriptor.GetEditor (value, typeof(UITypeEditor));
    editor = new TabStripMainMenuItemCollectionEditor (typeof (WebTabCollection));
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }
}

}

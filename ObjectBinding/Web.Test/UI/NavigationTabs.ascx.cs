using System;
using System.Reflection;
using System.Web.UI.WebControls;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;

using Rubicon.Web.Configuration;

namespace OBWTest.UI
{
	/// <summary>
	///		Summary description for NavigationTabs.
	/// </summary>
	public class NavigationTabs : System.Web.UI.UserControl
	{
    protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue WaiConformanceLevelField;
    protected Rubicon.Web.UI.Controls.TabbedMenu TabbedMenu;

		private void Page_Load(object sender, System.EventArgs e)
		{
      Type itemType = typeof (WcagConfiguration);
      PropertyInfo propertyInfo = itemType.GetProperty ("ConformanceLevel");
			EnumerationProperty property = 
          new EnumerationProperty(new PropertyBase.Parameters(BindableObjectProvider.Current, propertyInfo, propertyInfo.PropertyType, null, false,
          false, false));

      WaiConformanceLevelField.Property = property;
      WaiConformanceLevelField.LoadUnboundValue (WebConfiguration.Current.Wcag.ConformanceLevel, IsPostBack);
		}

    private void WaiConformanceLevelField_SelectionChanged(object sender, System.EventArgs e)
    {
      WebConfiguration.Current.Wcag.ConformanceLevel = (WaiConformanceLevel) WaiConformanceLevelField.Value;
      WaiConformanceLevelField.IsDirty = false;
    }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.WaiConformanceLevelField.SelectionChanged += new System.EventHandler(this.WaiConformanceLevelField_SelectionChanged);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
	}
}

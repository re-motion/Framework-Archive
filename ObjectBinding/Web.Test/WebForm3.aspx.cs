using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.ObjectBinding.Reflection;

namespace OBWTest
{
	/// <summary>
	/// Summary description for WebForm3.
	/// </summary>
	public class WebForm3: System.Web.UI.Page
	{
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
    protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel1;
    protected System.Web.UI.WebControls.Button SaveButton;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue HeightField;
    protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel2;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
    protected System.Web.UI.WebControls.RadioButtonList RadioButtonList1;
    protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource1;
  
		private void Page_Load (object sender, System.EventArgs e)
		{
      Person p = new Person();
      p.FirstName = "Hugo";
      p.LastName = "Meier";
      p.DateOfBirth = new DateTime (1973, 10, 21);
      p.Height = 170;
			reflectionBusinessObjectDataSource1.BusinessObject = p;

      this.DataBind();
      reflectionBusinessObjectDataSource1.LoadValues();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
      this.reflectionBusinessObjectDataSource1 = new Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource();
      this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
      // 
      // reflectionBusinessObjectDataSource1
      // 
      this.reflectionBusinessObjectDataSource1.BusinessObject = null;
      this.reflectionBusinessObjectDataSource1.EditMode = true;
      this.reflectionBusinessObjectDataSource1.TypeName = "OBWTest.Person, OBWTest";
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void SaveButton_Click (object sender, System.EventArgs e)
    {
      Page.Validate();
      if (IsValid)
      {
        reflectionBusinessObjectDataSource1.SaveValues();
        string s = ((Person)reflectionBusinessObjectDataSource1.BusinessObject).FirstName;
      }
    }
	}
}

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

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

using Rubicon.Data.DomainObjects;

using Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test
{
	public class NewObjectPage : System.Web.UI.Page
	{
    protected Rubicon.Web.UI.Controls.HtmlHeadContents Htmlheadcontents1;

    protected ControlWithAllDataTypes ControlWithAllDataTypesControl;

		private void Page_Load(object sender, System.EventArgs e)
		{
      ClassWithAllDataTypes objectWithAllDataTypes = null;

      if (!IsPostBack)
      {
        objectWithAllDataTypes = new ClassWithAllDataTypes ();

        Session["CurrentObjectID"] = objectWithAllDataTypes.ObjectID;
      }
      else
      {
        ClientTransaction.SetCurrent ((ClientTransaction) Session["ClientTransaction"]);
      }

      objectWithAllDataTypes = ClassWithAllDataTypes.GetObject ((ObjectID) Session["CurrentObjectID"]);

      ControlWithAllDataTypesControl.ObjectWithAllDataTypes = objectWithAllDataTypes;
		}


    private void SaveButton_Click(object sender, EventArgs e)
    {
      if (ControlWithAllDataTypesControl.Validate ())
      {
        ControlWithAllDataTypesControl.Save ();
      
        ClientTransaction.Current.Commit ();
      }
    }

    protected override void OnUnload(EventArgs e)
    {
      Session["ClientTransaction"] = ClientTransaction.Current;
      base.OnUnload (e);
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
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

  }
}

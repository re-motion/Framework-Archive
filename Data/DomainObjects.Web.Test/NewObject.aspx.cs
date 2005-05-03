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

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Classes;
using Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test
{
	public class NewObjectPage : BasePage
	{
    protected Rubicon.Web.UI.Controls.HtmlHeadContents Htmlheadcontents1;

    protected ControlWithAllDataTypes ControlWithAllDataTypesControl;

		private void Page_Load(object sender, System.EventArgs e)
		{
      ClassWithAllDataTypes objectWithAllDataTypes = null;

      if (!IsPostBack)
      {
        objectWithAllDataTypes = new ClassWithAllDataTypes ();

        ClassWithAllDataTypes objectWithAllDataTypes2 = CreateTestObjectWithAllDataTypes ();

        ClassForRelationTest objectForRelationTest1 = new ClassForRelationTest ();
        objectForRelationTest1.Name = "ObjectForRelationTest1";
        objectForRelationTest1.ClassWithAllDataTypesMandatory = objectWithAllDataTypes;
        objectWithAllDataTypes2.ClassForRelationTestMandatory = objectForRelationTest1;

        ClassForRelationTest objectForRelationTest2 = new ClassForRelationTest ();
        objectForRelationTest2.Name = "ObjectForRelationTest2";
        objectWithAllDataTypes.ClassForRelationTestMandatory = objectForRelationTest2;
        objectForRelationTest2.ClassWithAllDataTypesMandatory = objectWithAllDataTypes2;

        Session["CurrentObjectID"] = objectWithAllDataTypes.ObjectID;
      }
      else
      {
        ClientTransaction.SetCurrent ((ClientTransaction) Session["ClientTransaction"]);
      }

      objectWithAllDataTypes = ClassWithAllDataTypes.GetObject ((ObjectID) Session["CurrentObjectID"]);

      ControlWithAllDataTypesControl.ObjectWithAllDataTypes = objectWithAllDataTypes;
		}

    private ClassWithAllDataTypes CreateTestObjectWithAllDataTypes ()
    {
      ClassWithAllDataTypes test = new ClassWithAllDataTypes ();

      test.ByteProperty = 23;
      test.DateProperty = DateTime.Now;
      test.DateTimeProperty = DateTime.Now;
      test.DecimalProperty = decimal.Parse ("23.2");
      test.DoubleProperty = 23.2;
      test.GuidProperty = new Guid ("{00000008-0000-0000-0000-000000000009}");
      test.Int16Property = 2;
      test.Int32Property = 4;
      test.Int64Property = 8;
      test.SingleProperty = Single.Parse ("9.8");
      test.StringProperty = "aasdf";

      return test;
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

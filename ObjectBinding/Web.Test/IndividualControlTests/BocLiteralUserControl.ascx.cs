using System;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

using OBRTest;

namespace OBWTest.IndividualControlTests
{

  public partial class BocLiteralUserControl : BaseUserControl
  {
    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers ();

      this.CVTestSetNullButton.Click += new System.EventHandler (this.CVTestSetNullButton_Click);
      this.CVTestSetNewValueButton.Click += new System.EventHandler (this.CVTestSetNewValueButton_Click);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Person person = (Person) CurrentObject.BusinessObject;

      UnboundCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty ("CVString");
      UnboundCVField.LoadUnboundValue (person.CVString, IsPostBack);
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    private void CVTestSetNullButton_Click (object sender, System.EventArgs e)
    {
      CVField.Value = null;
    }

    private void CVTestSetNewValueButton_Click (object sender, System.EventArgs e)
    {
      CVField.Value = "Foo<br/>Bar";
    }
  }

}

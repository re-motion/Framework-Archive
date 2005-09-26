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

using Rubicon.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

using Rubicon.Data.DomainObjects.Web.Test.WxeFunctions;
using Rubicon.Data.DomainObjects.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Test
{
public class WxeTestPage : WxePage
{
  private static ObjectID s_objectWithAllDataTypesID = new ObjectID ("ClassWithAllDataTypes", new Guid ("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D"));

  protected System.Web.UI.WebControls.Button WxeTransactedFunctionCreateNewButton;
  protected System.Web.UI.WebControls.Label ResultLabel;
  protected System.Web.UI.WebControls.Button WxeTransactedFunctionNoneButton;
  protected System.Web.UI.WebControls.Button WxeTransactedFunctionCreateNewAutoCommitButton;
  protected System.Web.UI.WebControls.Button WxeTransactedFunctionCreateNewNoAutoCommitButton;
  protected System.Web.UI.WebControls.Button WxeTransactedFunctionNoneAutoCommitButton;
  protected System.Web.UI.WebControls.Button WxeTransactedFunctionNoneNoAutoCommitButton;

  protected WxeTestPageFunction CurrentWxeTestPageFunction
  {
    get { return (WxeTestPageFunction) CurrentFunction; }
  }

  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

	private void Page_Load(object sender, System.EventArgs e)
	{
    ResultLabel.Visible = false;
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
    this.WxeTransactedFunctionCreateNewButton.Click += new System.EventHandler(this.WxeTransactedFunctionCreateNewButton_Click);
    this.WxeTransactedFunctionNoneButton.Click += new System.EventHandler(this.WxeTransactedFunctionNoneButton_Click);
    this.WxeTransactedFunctionCreateNewAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionCreateNewAutoCommitButton_Click);
    this.WxeTransactedFunctionCreateNewNoAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionCreateNewNoAutoCommitButton_Click);
    this.WxeTransactedFunctionNoneAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionNoneAutoCommitButton_Click);
    this.WxeTransactedFunctionNoneNoAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionNoneNoAutoCommitButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void WxeTransactedFunctionCreateNewButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();

      ExecuteFunction (new CreateRootTestTransactedFunction (ClientTransaction.Current));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      ShowResultText ("Test WxeTransactedFunction (CreateNew) executed successfully");
    }
  }

  private void WxeTransactedFunctionNoneButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();

      ExecuteFunction (new CreateNoneTestTransactedFunction (ClientTransaction.Current));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      ShowResultText ("Test WxeTransactedFunction (None) executed successfully");
    }
  }

  private void WxeTransactedFunctionCreateNewAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();
      SetInt32Property (5, new ClientTransaction ());

      ExecuteFunction (new AutoCommitTestTransactedFunction (
          Rubicon.Data.DomainObjects.Web.ExecutionEngine.TransactionMode.CreateRoot, s_objectWithAllDataTypesID));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (new ClientTransaction()) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not properly commit or set the property value.");

      ShowResultText ("Test WxeTransactedFunction (AutoCommit = true) executed successfully");
    }
  }

  private void WxeTransactedFunctionCreateNewNoAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();
      SetInt32Property (5, new ClientTransaction ());

      ExecuteFunction (new NoAutoCommitTestTransactedFunction (
          Rubicon.Data.DomainObjects.Web.ExecutionEngine.TransactionMode.CreateRoot, s_objectWithAllDataTypesID));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did set and commit the property value.");

      ShowResultText ("Test WxeTransactedFunction (AutoCommit = false) executed successfully");
    }
  }

  private void WxeTransactedFunctionNoneAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      SetInt32Property (5, new ClientTransaction ());
      ClientTransaction.SetCurrent (null);

      RememberCurrentClientTransaction ();

      ExecuteFunction (new AutoCommitTestTransactedFunction (
          Rubicon.Data.DomainObjects.Web.ExecutionEngine.TransactionMode.None, s_objectWithAllDataTypesID));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (ClientTransaction.Current) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not set property value.");

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (None, AutoCommit = true) executed successfully");
    }
  }

  private void WxeTransactedFunctionNoneNoAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      SetInt32Property (5, new ClientTransaction ());
      ClientTransaction.SetCurrent (null);

      RememberCurrentClientTransaction ();

      ExecuteFunction (new NoAutoCommitTestTransactedFunction (
          Rubicon.Data.DomainObjects.Web.ExecutionEngine.TransactionMode.None, s_objectWithAllDataTypesID));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (ClientTransaction.Current) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not set the property value.");

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (None, AutoCommit = false) executed successfully");
    }
  }

  private void SetInt32Property (int value, ClientTransaction clientTransaction)
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (s_objectWithAllDataTypesID, clientTransaction);

    objectWithAllDataTypes.Int32Property = value;

    clientTransaction.Commit ();
  }

  private int GetInt32Property (ClientTransaction clientTransaction)
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (s_objectWithAllDataTypesID, clientTransaction);

    return objectWithAllDataTypes.Int32Property;
  }

  private void RememberCurrentClientTransaction ()
  {
    CurrentWxeTestPageFunction.CurrentClientTransaction = ClientTransaction.Current;
  }

  private void CheckCurrentClientTransactionRestored ()
  {
    if (CurrentWxeTestPageFunction.CurrentClientTransaction != ClientTransaction.Current)
      throw new TestFailureException ("ClientTransaction.Current was not properly restored to the state before the WxeTransactedFunction was called.");
  }

  private void ShowResultText (string text)
  {
    ResultLabel.Visible = true;
    ResultLabel.Text = text;
  }

}
}

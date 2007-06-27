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

using Rubicon.Data.DomainObjects.Web.Legacy.Test.WxeFunctions;
using Rubicon.Data.DomainObjects.Web.Legacy.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Legacy.Test
{
public class WxeTestPage : WxePage
{
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
    this.WxeTransactedFunctionNoneAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionNoneAutoCommitButton_Click);
    this.WxeTransactedFunctionCreateNewNoAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionCreateNewNoAutoCommitButton_Click);
    this.WxeTransactedFunctionNoneNoAutoCommitButton.Click += new System.EventHandler(this.WxeTransactedFunctionNoneNoAutoCommitButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void WxeTransactedFunctionCreateNewButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();

      ExecuteFunction (new CreateRootTestTransactedFunction (ClientTransactionScope.CurrentTransaction));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      ShowResultText ("Test WxeTransactedFunction (CreateNew) executed successfully.");
    }
  }

  private void WxeTransactedFunctionNoneButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();

      ExecuteFunction (new CreateNoneTestTransactedFunction (ClientTransactionScope.CurrentTransaction));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      ShowResultText ("Test WxeTransactedFunction (None) executed successfully.");
    }
  }

  private void WxeTransactedFunctionCreateNewAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();
      SetInt32Property (5, new ClientTransaction ());

      ExecuteFunction (new AutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (new ClientTransaction()) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not properly commit or set the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = true) executed successfully.");
    }
  }

  private void WxeTransactedFunctionCreateNewNoAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      RememberCurrentClientTransaction ();
      SetInt32Property (5, new ClientTransaction ());

      ExecuteFunction (new NoAutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did set and commit the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = false) executed successfully.");
    }
  }

  private void WxeTransactedFunctionNoneAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      SetInt32Property (5, new ClientTransaction ());

      new ClientTransactionScope (null);

      RememberCurrentClientTransaction();

      ExecuteFunction (new AutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (ClientTransactionScope.CurrentTransaction) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not set property value.");

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = true) executed successfully.");
    }
  }

  private void WxeTransactedFunctionNoneNoAutoCommitButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
    {
      SetInt32Property (5, new ClientTransaction ());

      new ClientTransactionScope (null);

      RememberCurrentClientTransaction ();

      ExecuteFunction (new NoAutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1));
    }
    else
    {
      CheckCurrentClientTransactionRestored ();

      if (GetInt32Property (ClientTransactionScope.CurrentTransaction) != 10)
        throw new TestFailureException ("The WxeTransactedFunction wrongly did not set the property value.");

      if (GetInt32Property (new ClientTransaction ()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = false) executed successfully.");
    }
  }

  private void SetInt32Property (int value, ClientTransaction clientTransaction)
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1, clientTransaction);

    objectWithAllDataTypes.Int32Property = value;

    clientTransaction.Commit ();
  }

  private int GetInt32Property (ClientTransaction clientTransaction)
  {
    ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1, clientTransaction);

    return objectWithAllDataTypes.Int32Property;
  }

  private void RememberCurrentClientTransaction ()
  {
    CurrentWxeTestPageFunction.CurrentClientTransaction = ClientTransactionScope.CurrentTransaction;
  }

  private void CheckCurrentClientTransactionRestored ()
  {
    if (CurrentWxeTestPageFunction.CurrentClientTransaction != ClientTransactionScope.CurrentTransaction)
      throw new TestFailureException ("ClientTransactionScope.CurrentTransaction was not properly restored to the state before the WxeTransactedFunction was called.");
  }

  private void ShowResultText (string text)
  {
    ResultLabel.Visible = true;
    ResultLabel.Text = text;
  }

}
}

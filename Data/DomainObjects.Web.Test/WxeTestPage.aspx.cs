using System;
using System.Web.UI.WebControls;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Data.DomainObjects.Web.Test.WxeFunctions;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Data.DomainObjects.Web.Test
{
  public class WxeTestPage: WxePage
  {
    protected Button WxeTransactedFunctionCreateNewButton;
    protected Label ResultLabel;
    protected Button WxeTransactedFunctionNoneButton;
    protected Button WxeTransactedFunctionCreateNewAutoCommitButton;
    protected Button WxeTransactedFunctionCreateNewNoAutoCommitButton;
    protected Button WxeTransactedFunctionNoneAutoCommitButton;
    protected Button WxeTransactedFunctionNoneNoAutoCommitButton;

    protected WxeTestPageFunction CurrentWxeTestPageFunction
    {
      get { return (WxeTestPageFunction) CurrentFunction; }
    }

    protected HtmlHeadContents HtmlHeadContents;

    private void Page_Load (object sender, EventArgs e)
    {
      ResultLabel.Visible = false;
    }

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.WxeTransactedFunctionCreateNewButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewButton_Click);
      this.WxeTransactedFunctionNoneButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneButton_Click);
      this.WxeTransactedFunctionCreateNewAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewAutoCommitButton_Click);
      this.WxeTransactedFunctionNoneAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneAutoCommitButton_Click);
      this.WxeTransactedFunctionCreateNewNoAutoCommitButton.Click +=
          new System.EventHandler (this.WxeTransactedFunctionCreateNewNoAutoCommitButton_Click);
      this.WxeTransactedFunctionNoneNoAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionNoneNoAutoCommitButton_Click);
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion

    private void WxeTransactedFunctionCreateNewButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        RememberCurrentClientTransaction();

        ExecuteFunction (new CreateRootTestTransactedFunction (ClientTransaction.Current));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        ShowResultText ("Test WxeTransactedFunction (CreateNew) executed successfully.");
      }
    }

    private void WxeTransactedFunctionNoneButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        RememberCurrentClientTransaction();

        ExecuteFunction (new CreateNoneTestTransactedFunction (ClientTransaction.Current));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        ShowResultText ("Test WxeTransactedFunction (None) executed successfully.");
      }
    }

    private void WxeTransactedFunctionCreateNewAutoCommitButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, new ClientTransaction());

        ExecuteFunction (new AutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (new ClientTransaction()) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not properly commit or set the property value.");

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = true) executed successfully.");
      }
    }

    private void WxeTransactedFunctionCreateNewNoAutoCommitButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, new ClientTransaction());

        ExecuteFunction (new NoAutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (new ClientTransaction()) != 5)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did set and commit the property value.");

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = false) executed successfully.");
      }
    }

    private void WxeTransactedFunctionNoneAutoCommitButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        SetInt32Property (5, new ClientTransaction());
        ClientTransaction.SetCurrent (null);

        RememberCurrentClientTransaction();

        ExecuteFunction (new AutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.Current) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not set property value.");

        if (GetInt32Property (new ClientTransaction()) != 5)
          throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = true) executed successfully.");
      }
    }

    private void WxeTransactedFunctionNoneNoAutoCommitButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        SetInt32Property (5, new ClientTransaction());
        ClientTransaction.SetCurrent (null);

        RememberCurrentClientTransaction();

        ExecuteFunction (new NoAutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1));
      }
      else
      {
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.Current) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not set the property value.");

        if (GetInt32Property (new ClientTransaction()) != 5)
          throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = false) executed successfully.");
      }
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1, clientTransaction);

      objectWithAllDataTypes.Int32Property = value;

      clientTransaction.Commit();
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1, clientTransaction);

      return objectWithAllDataTypes.Int32Property;
    }

    private void RememberCurrentClientTransaction()
    {
      CurrentWxeTestPageFunction.CurrentClientTransaction = ClientTransaction.Current;
    }

    private void CheckCurrentClientTransactionRestored()
    {
      if (CurrentWxeTestPageFunction.CurrentClientTransaction != ClientTransaction.Current)
        throw new TestFailureException (
            "ClientTransaction.Current was not properly restored to the state before the WxeTransactedFunction was called.");
    }

    private void ShowResultText (string text)
    {
      ResultLabel.Visible = true;
      ResultLabel.Text = text;
    }
  }
}
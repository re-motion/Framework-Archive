using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

[WebMultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class IndividualControlTestForm : TestBasePage
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected System.Web.UI.WebControls.PlaceHolder UserControlPlaceHolder;
  protected Rubicon.Web.UI.Controls.WebButton SaveButton;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;
  protected System.Web.UI.WebControls.Literal Stack;

  private IDataEditControl _dataEditControl;
  protected Rubicon.Web.UI.Controls.WebButton SaveAndRestartButton;
  protected Rubicon.Web.UI.Controls.WebButton CancelButton;
  private bool _isCurrentObjectSaved = false;

  protected override void RegisterEventHandlers()
  {
    base.RegisterEventHandlers ();
    
    Init += new EventHandler (IndividualControlTestForm_Init);
    Load += new EventHandler (IndividualControlTestForm_Load);
    PreRender += new EventHandler (IndividualControlTestForm_PreRender);
    Unload += new EventHandler (IndividualControlTestForm_Unload);

    SaveButton.Click += new EventHandler (SaveButton_Click);
    SaveAndRestartButton.Click += new EventHandler (SaveAndRestartButton_Click);
    CancelButton.Click += new EventHandler (CancelButton_Click);
  }

  private void IndividualControlTestForm_Init (object sender, EventArgs e)
  {
    this.EnableAbort = NaBooleanEnum.True;
    this.EnableOutOfSequencePostBacks = NaBooleanEnum.True;
    this.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
  }

  private void IndividualControlTestForm_Load (object sender, EventArgs e)
  {
    LoadUserControl();
    PopulateDataSources();
    LoadValues (IsPostBack);
  }

  private void IndividualControlTestForm_PreRender(object sender, EventArgs e)
  {
    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    sb.Append ("<b>Stack:</b><br>");
    for (WxeStep step = CurrentStep; step != null; step = step.ParentStep)
      sb.AppendFormat ("{0}<br>", step.ToString());      
    Stack.Text = sb.ToString();
  }

  private void IndividualControlTestForm_Unload(object sender, EventArgs e)
  {
    if (! _isCurrentObjectSaved)
    {
      SaveValues (true);
    }
  }

  private void SaveButton_Click(object sender, EventArgs e)
  {
    bool isValid = ValidateDataSources();
    if (isValid)
    {
      SaveValues (false);
      _isCurrentObjectSaved = true;
    }
  }

  private void SaveAndRestartButton_Click(object sender, EventArgs e)
  {
    bool isValid = ValidateDataSources();
    if (isValid)
    {
      SaveValues (false);
      _isCurrentObjectSaved = true;
      ExecuteNextStep();
    }
  }

  private void CancelButton_Click(object sender, EventArgs e)
  {
    throw new WxeUserCancelException();
  }

  private void LoadUserControl()
  {
    _dataEditControl = (IDataEditControl) LoadControl (CurrentFunction.UserControl);
    if (_dataEditControl == null)
      throw new InvalidOperationException (string.Format ("IDataEditControl '{0}' could not be loaded.", CurrentFunction.UserControl));
    UserControlPlaceHolder.Controls.Add ((Control) _dataEditControl);
  }

  private void PopulateDataSources()
  {
    CurrentObject.BusinessObject = CurrentFunction.Person;
    _dataEditControl.BusinessObject = CurrentFunction.Person;
  }

  private void LoadValues (bool interim)
  {
    CurrentObject.LoadValues (interim);
    _dataEditControl.LoadValues (interim);
  }

  private void SaveValues (bool interim)
  {
    _dataEditControl.SaveValues (interim);
    CurrentObject.SaveValues (interim);
  }

  private bool ValidateDataSources ()
  {
    PrepareValidation();
    
    bool isValid = _dataEditControl.Validate();
    isValid &= CurrentObject.Validate();
    
    return isValid;
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

  }
  #endregion
}

}

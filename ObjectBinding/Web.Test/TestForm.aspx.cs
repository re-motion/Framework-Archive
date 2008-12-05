// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

public class TestForm : Page
{
  protected Button PostBackButton;
  protected LazyContainer LazyContainer;
  protected BocTextValue TextField;
  protected TextBox TextBox1;
  protected RequiredFieldValidator RequiredFieldValidator1;
  protected HtmlTable FormGrid;
  protected FormGridManager fgm;
  protected BocTextValue field;
  protected HtmlHeadContents HtmlHeadContents;

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
    this.PostBackButton.Click += new System.EventHandler(this.PostBackButton_Click);

  }
	#endregion

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);

    if (! IsPostBack)
      TextField.Text = "Foo Bar";
  
    bool ensure = true;
    if (ensure)
      LazyContainer.Ensure();
  }

  public override void Validate()
  {
    base.Validate ();
  }

  private void PostBackButton_Click(object sender, EventArgs e)
  {
  
  }


}

}

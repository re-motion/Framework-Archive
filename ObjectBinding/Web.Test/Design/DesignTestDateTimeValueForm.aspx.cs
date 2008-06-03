/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest.Design
{
public class DesignTestDateTimeValueForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocDateTimeValue BocDateTimeVale1;
  protected BocDateTimeValue BocDateTimeVale2;
  protected BocDateTimeValue BocDateTimeVale3;
  protected BocDateTimeValue BocDateTimeVale4;
  protected BocDateTimeValue BocDateTimeVale17;
  protected BocDateTimeValue BocDateTimeVale18;
  protected BocDateTimeValue BocDateTimeVale5;
  protected BocDateTimeValue BocDateTimeVale6;
  protected BocDateTimeValue BocDateTimeVale8;
  protected BocDateTimeValue BocDateTimeVale19;
  protected BocDateTimeValue BocDateTimeVale24;
  protected BocDateTimeValue BocDateTimeVale25;
  protected BocDateTimeValue BocDateTimeVale26;
  protected BocDateTimeValue BocDateTimeVale27;
  protected BocDateTimeValue BocDateTimeVale28;
  protected BocDateTimeValue BocDateTimeVale29;
  protected BocDateTimeValue BocDateTimeVale36;
  protected BocDateTimeValue BocDateTimeVale37;
  protected BocDateTimeValue BocDateTimeVale38;
  protected BocDateTimeValue BocDateTimeVale39;
  protected BocDateTimeValue BocDateTimeVale40;
  protected BocDateTimeValue BocDateTimeVale41;
  protected BocDateTimeValue BocDateTimeVale42;
  protected BocDateTimeValue BocDateTimeVale43;
  protected BocDateTimeValue BocDateTimeVale44;
  protected BocDateTimeValue BocDateTimeVale45;
  protected BocDateTimeValue BocDateTimeVale46;
  protected BocDateTimeValue BocDateTimeVale47;
  protected BocDateTimeValue BocDateTimeVale7;
  protected BocDateTimeValue BocDateTimeVale48;
  protected BocDateTimeValue BocDateTimeVale49;
  protected BocDateTimeValue BocDateTimeVale51;
  protected BocDateTimeValue BocDateTimeVale50;
  protected BocDateTimeValue BocDateTimeVale52;
  protected BocDateTimeValue BocDateTimeVale9;
  protected BocDateTimeValue BocDateTimeVale10;
  protected BocDateTimeValue BocDateTimeVale11;
  protected BocDateTimeValue BocDateTimeVale12;
  protected BocDateTimeValue BocDateTimeVale22;
  protected BocDateTimeValue BocDateTimeVale23;
  protected BocDateTimeValue BocDateTimeVale13;
  protected BocDateTimeValue BocDateTimeVale14;
  protected BocDateTimeValue BocDateTimeVale15;
  protected BocDateTimeValue BocDateTimeVale16;
  protected BocDateTimeValue BocDateTimeVale20;
  protected BocDateTimeValue BocDateTimeVale21;
  protected BocDateTimeValue BocDateTimeVale30;
  protected BocDateTimeValue BocDateTimeVale31;
  protected BocDateTimeValue BocDateTimeVale32;
  protected BocDateTimeValue BocDateTimeVale33;
  protected BocDateTimeValue BocDateTimeVale34;
  protected BocDateTimeValue BocDateTimeVale35;
  protected HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner = person.Partner;
    
    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.EnableAbort = false;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}

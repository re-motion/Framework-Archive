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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;


namespace OBWTest
{

public class WebForm1 : Page
{
  protected BocTextValue FirstNameField;
  protected SmartLabel BocPropertyLabel1;
  protected Button SaveButton;
  protected BocTextValue LastNameField;
  protected SmartLabel BocPropertyLabel2;
  protected BocTextValue DateOfBirthField;
  protected SmartLabel BocPropertyLabel3;
  protected BocTextValueValidator BocTextValueValidator1;
  protected BocTextValue HeightField;
  protected SmartLabel BocPropertyLabel4;
  protected BocTextValueValidator BocTextValueValidator2;
  protected Label Label1;
  protected BocEnumValue GenderField;
  protected SmartLabel BocPropertyLabel5;
  protected BocEnumValue MarriageStatusField;
  protected SmartLabel SmartLabel1;
  protected BocTextValue PartnerFirstNameField;
  protected Label Label2;
  protected BindableObjectDataSourceControl CurrentObjectDataSource;
  protected BusinessObjectReferenceDataSourceControl PartnerDataSource;
  protected SmartLabel SmartLabel2;

	private void Page_Load(object sender, EventArgs e)
	{
    XmlReflectionBusinessObjectStorageProvider.Current.Reset ();
    Guid personID = new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
    Person person = Person.GetObject (personID);
    Person partner;
    if (person == null)
    {
      person = Person.CreateObject (personID);
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime (1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;

      partner = person.Partner = Person.CreateObject();
      partner.FirstName = "Sepp";
      partner.LastName = "Forcher";

      person.SaveObject();
      partner.SaveObject();
    }
    else
    {
      partner = person.Partner;
    }

    CurrentObjectDataSource.BusinessObject = (IBusinessObject) person;

    this.DataBind();
    CurrentObjectDataSource.LoadValues (IsPostBack);
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
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click (object sender, EventArgs e)
  {
    if (Page.IsValid)
    {
      CurrentObjectDataSource.SaveValues (false);
      Person person = (Person) CurrentObjectDataSource.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
  }

}

}

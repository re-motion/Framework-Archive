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
using Remotion.ObjectBinding.Sample;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest.IndividualControlTests
{

[Serializable]
public class TestFunction: WxeFunction
{
  private Person _person;

  public TestFunction ()
    : base (new NoneTransactionMode ())
  {
    
  }

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public string UserControl
  {
    get 
    {
      return (string) Variables["UserControl"]; 
    }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("UserControl", value);
      Variables["UserControl"] = value; 
    }
  }

  public Person Person
  {
    get
    {
      return _person;
    }
  }

  // steps
  private void Step1()
  {
    if (StringUtility.IsNullOrEmpty (UserControl))
      UserControl = "BocBooleanValueUserControl.ascx";

    ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
  }

  private void Step2()
  {
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();

    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
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
    }
    else
    {
      partner = person.Partner;
    }

    Job[] jobs = new Job[2];
    
    jobs[0] = Job.CreateObject (Guid.NewGuid());
    jobs[0].Title = "Programmer";
    jobs[0].StartDate = new DateTime (2000, 1, 1);
    jobs[0].EndDate = new DateTime (2004, 12, 31);

    jobs[1] = Job.CreateObject (Guid.NewGuid());
    jobs[1].Title = "CEO";
    jobs[1].StartDate = new DateTime (2005, 1, 1);

    if (person.Children.Length == 0)
    {
      Person[] children = new Person[2];
      
      children[0] = Person.CreateObject (Guid.NewGuid());
      children[0].FirstName = "Jack";
      children[0].LastName = "Doe";
      children[0].DateOfBirth = new DateTime (1990, 4, 15);
      children[0].Height = 160;
      children[0].MarriageStatus = MarriageStatus.Single;
      children[0].Jobs = jobs;

      children[1] = Person.CreateObject (Guid.NewGuid());
      children[1].FirstName = "Max";
      children[1].LastName = "Doe";
      children[1].DateOfBirth = new DateTime (1991, 4, 15);
      children[1].Height = 155;
      children[1].MarriageStatus = MarriageStatus.Single;

      person.Children = children;
    }

    if (person.Jobs.Length == 0)
    {
      person.Jobs = jobs;
    }
  
    _person = person;
  }

  private WxeStep Step3 = new WxePageStep ("IndividualControlTests/Form.aspx");

  private void Step4()
  {
    _person.SaveObject();
    if (_person.Children != null)
    {
      foreach (Person child in _person.Children)
        child.SaveObject();
    }
    if (_person.Jobs != null)
    {
      foreach (Job job in _person.Jobs)
        job.SaveObject();
    }
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }
}

}

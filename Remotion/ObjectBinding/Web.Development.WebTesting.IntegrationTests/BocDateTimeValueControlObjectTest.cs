﻿using System;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocDateTimeValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByID ("body_DataEditControl_DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByIndex (2);
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().First();
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetDateTimeValue().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC date time value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByDisplayName ("DateOfBirth");
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByDomainProperty ("DateOfBirth");
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByDomainProperty ("DateOfBirth", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocDateTimeValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DateOfBirthField_Normal"));
    }

    [Test]
    public void TestGetDateTime ()
    {
      var home = Start();

      var bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_ReadOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Disabled");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_NoAutoPostBack");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_DateOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime (new TimeSpan(13, 37, 42));
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime(2008, 4, 4, 13, 37, 42)));
    }

    [Test]
    public void TestSetDateTime ()
    {
      var home = Start();

      var initDateTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime(1988, 3, 20, 4, 2, 0);
      var withSeconds = new DateTime (1988, 3, 20, 4, 2, 17);
      
      var bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (dateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (dateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetDateTime (dateTime); // no auto post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (dateTime, Behavior.WaitFor (WaitFor.Nothing)); // same value, does not trigger post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (initDateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (initDateTime));
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (dateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_DateOnly");
      bocDateTimeValue.SetDateTime (dateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("DateOnlyCurrentValueLabel").Text), Is.EqualTo (dateTime.Date));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetDateTime (withSeconds);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (withSeconds));
    }

    [Test]
    public void TestSetDate ()
    {
      var home = Start();

      var initDateTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime(1988, 3, 20, 4, 2, 0);
      var setDateTime = new DateTime (1988, 3, 20, 12, 0, 0);
      var withSeconds = new DateTime (1988, 3, 20, 4, 2, 17);
      var setWithSeconds = new DateTime (1988, 3, 20, 12, 0, 0);
      
      var bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (dateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetDate (dateTime); // no auto post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (dateTime, Behavior.WaitFor (WaitFor.Nothing)); // same value, does not trigger post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (initDateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (initDateTime));
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setDateTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_DateOnly");
      bocDateTimeValue.SetDate (dateTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("DateOnlyCurrentValueLabel").Text), Is.EqualTo (setDateTime.Date));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetDate (withSeconds);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (setWithSeconds));
    }

    [Test]
    public void TestSetTime ()
    {
      var home = Start();

      var initTime = new TimeSpan (12, 0, 0);
      var setInitTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var time = new TimeSpan(4, 2, 0);
      var setTime = new DateTime (2008, 4, 4, 4, 2, 0);
      var withSeconds = new TimeSpan (4, 2, 17);
      var setWithSeconds = new DateTime (2008, 4, 4, 4, 2, 17);
      
      var bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (time);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetTime (time); // no auto post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setInitTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (time, Behavior.WaitFor (WaitFor.Nothing)); // same value, does not trigger post back
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setInitTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (initTime);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setInitTime));
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setTime));

      bocDateTimeValue = home.GetDateTimeValue().ByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime (withSeconds);
      Assert.That (DateTime.Parse(home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (setWithSeconds));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocDateTimeValue");
    }
  }
}
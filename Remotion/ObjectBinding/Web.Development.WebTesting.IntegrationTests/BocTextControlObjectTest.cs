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
  public class BocTextControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocText = home.GetText().ByID ("body_DataEditControl_LastNameField_Normal");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocText = home.GetText().ByIndex (2);
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocText = home.GetText().ByLocalID ("LastNameField_Normal");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocText = home.GetText().First();
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetText().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC text.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocText = home.GetText().ByDisplayName ("LastName");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocText = home.GetText().ByDomainProperty ("LastName");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocText = home.GetText().ByDomainProperty ("LastName", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocText = home.GetText().ByLocalID ("LastNameField_Normal");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetText().ByLocalID ("LastNameField_ReadOnly");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetText().ByLocalID ("LastNameField_Disabled");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetText().ByLocalID ("LastNameField_NoAutoPostBack");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));
    }

    [Test]
    [Ignore ("Ignored until FillWith problem has been diagnosed.")]
    // Todo RM-6297: enable test as soon as FillWith problem has been diagnosed.
    public void TestFillWith ()
    {
      var home = Start();

      var bocText = home.GetText().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));

      bocText = home.GetText().ByLocalID ("LastNameField_NoAutoPostBack");
      bocText.FillWith ("Blubba"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.GetText().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba", WaitFor.Nothing); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.GetText().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Doe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocText");
    }
  }
}
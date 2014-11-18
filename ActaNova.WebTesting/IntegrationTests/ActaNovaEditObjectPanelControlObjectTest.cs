﻿using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaEditObjectPanelControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var personErzeugen = home.MainMenu.Select ("Neu", "Person").ExpectMainPage();
      personErzeugen.FormPage.GetTextValue ("LastName").FillWith ("Rauch");
      personErzeugen.FormPage.GetEnumValue ("Gender").SelectOption ("Male");
      personErzeugen.FormPage.Perform ("Save");

      personErzeugen.FormPage.GetOnlyTabbedMultiView().SwitchTo ("LegalEntityLinksFormPage_view");

      personErzeugen.FormPage.GetList ("Links").GetListMenu().SelectItem ("NewEmailCommand_2");

      var dialog = personErzeugen.FormPage.GetDialog();
      dialog.GetAutoComplete ("LinkClassification").FillWith ("Inhaber");
      dialog.GetTextValue ("Address").FillWith ("dominik.rauch@rubicon.eu");
      dialog.Perform ("TakeOverAndNew");

      dialog.GetAutoComplete ("LinkClassification").FillWith ("Nutzer");
      dialog.GetTextValue ("Address").FillWith ("max.mustermann@rubicon.eu");
      dialog.Perform ("TakeOverDetails");

      home = personErzeugen.FormPage.Perform ("SaveAndReturn").ExpectMainPage();

      var personBearbeiten =
          home.Tree.GetNode()
              .WithDisplayText ("Zuletzt gespeicherte Objekte")
              .Expand()
              .GetNode()
              .WithDisplayText ("Rauch")
              .Expand()
              .GetNode ("WrappedLinks")
              .Select()
              .ExpectMainPage();

      Assert.That (personBearbeiten.FormPage.GetList ("Links").GetRowCount(), Is.EqualTo (2));
    }
  }
}
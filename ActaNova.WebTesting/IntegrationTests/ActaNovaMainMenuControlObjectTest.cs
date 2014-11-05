﻿using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaMainMenuControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      home.MainMenu.Select ("Neu", "Akt");
      Assert.That (home.GetTitle(), Is.EqualTo ("Akt erzeugen"));

      var aktErzeugenPage = home.MainMenu.Select (new[] { "Verfahrensbereich", "BW - Bauen und Wohnen" }, Continue.When (Wxe.PostBackCompleted))
          .ExpectActaNova (ActaNovaMessageBox.OkayWithoutPostback);

      home = aktErzeugenPage.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompleted).AndModalDialogHasBeenAccepted()).ExpectActaNova();

      home = home.MainMenu.Select ("Verfahrensbereich", "BW - Bauen und Wohnen").ExpectActaNova();

      home = home.MainMenu.Select ("Suchen", "Stammdatenobjekt", "Rechtsträger", "Organisation").ExpectActaNova();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");
      administration.Close();

      var wuensche = home.MainMenu.Select (new[] { "Extras", "Fehlerberichte/Wünsche" }, Continue.When (Wxe.PostBackCompleted))
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
      wuensche.Close();

      home = home.MainMenu.Select (new[] { "Extras", "Befehle vormerken" }, Continue.When (Wxe.PostBackCompleted)).ExpectActaNova();
      home = home.MainMenu.Select (new[] { "Extras", "Befehle sofort ausführen" }, Continue.When (Wxe.PostBackCompleted)).ExpectActaNova();

      home.Refresh();
    }
  }
}
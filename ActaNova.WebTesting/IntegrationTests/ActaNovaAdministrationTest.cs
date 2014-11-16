﻿using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaAdministrationTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestClassificationTypesTab ()
    {
      var home = Start();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");

      var tabbedMenu = administration.GetOnlyTabbedMenu();
      tabbedMenu.SelectSubItem ("StandardClassificationTypes");

      var classificationTypeField = administration.GetOnlyFormGrid().GetDropDownList().Single();
      classificationTypeField.SelectOption().WithText ("Adressart");

      var classificationTypeList = administration.GetList().Single();
      Assert.That (classificationTypeList.GetRowCount(), Is.EqualTo (4));

      var downloadNotification = administration.GetImageButton ("ExcelExportButton").Click().Expect<ActaNovaMessageBoxPageObject>();
      downloadNotification.Confirm();
      
      administration.Close();

      var tempExportDokumente = home.MainMenu.Select ("Extras", "Temp. Export Dokumente").ExpectMainPage();
      var itemsList = tempExportDokumente.FormPage.GetList ("Items");
      Assert.That (itemsList.GetRowCount(), Is.EqualTo (1));

      var deletionConfirmation = itemsList.GetRow().WithIndex (1).GetCell().WithIndex (2).ExecuteCommand().Expect<ActaNovaMessageBoxPageObject>();
      deletionConfirmation.Yes();

      Assert.That (itemsList.GetRowCount(), Is.EqualTo (0));
    }

    [Test]
    public void TestSecurityTab ()
    {
      var home = Start();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");

      var tabbedMenu = administration.GetOnlyTabbedMenu();
      tabbedMenu.SelectItem ("SecurityTab");
      tabbedMenu.SelectSubItem ("AccessControlSubMenuTab");

      var securableClassesTree = administration.GetTreeView ("DerivedClasses");

      var permissions = securableClassesTree.GetRootNode()
          .GetNode()
          .WithIndex (1)
          .Click()
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      var objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Beim Objekt nur lesend berechtigt");
      permissions.PerformAndCloseWindow ("Save");

      permissions = securableClassesTree.GetRootNode().GetNode().WithIndex (1).Click().ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Standard");
      permissions.PerformAndCloseWindow ("Save");

      administration.Close();
    }
  }
}
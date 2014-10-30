﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents the ActaNova main menu.
  /// </summary>
  public class ActaNovaMainMenuControlObject : ActaNovaMainFrameControlObject
  {
    public ActaNovaMainMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Selects the menu item using a path through the menu given by <paramref name="menuItems"/>, e.g.: <c>.Select("New", "File")</c>.
    /// </summary>
    public UnspecifiedPageObject Select (params string[] menuItems)
    {
      return Select (menuItems.AsEnumerable());
    }

    /// <summary>
    /// See <see cref="Select(string[])"/>, however, a <paramref name="completionDetection"/> may be given which is used instead of the default one.
    /// </summary>
    public UnspecifiedPageObject Select (IEnumerable<string> menuItems, ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);

      Scope.PerformAction (
          s =>
          {
            // HACK: Use Selenium directly, Coypu does not support Actions API yet.
            var webDriver = (IWebDriver) Context.Browser.Native;
            var nativeMainMenuScope = (IWebElement) s.Native;

            var actions = new ActionsWithWaitSupport (webDriver);
            var timeout = WebTestingFrameworkConfiguration.Current.SearchTimeout;

            var nativeLastMenuItemScope = AddMenuItemHoverActions (actions, nativeMainMenuScope, menuItems, timeout);
            actions.Click (nativeLastMenuItemScope);

            actions.Perform();
          },
          Context,
          actualCompletionDetection);

      return UnspecifiedPage();
    }

    private static IWebElement AddMenuItemHoverActions (
        ActionsWithWaitSupport actions,
        IWebElement nativeMainMenuScope,
        IEnumerable<string> menuItems,
        TimeSpan timeout)
    {
      var nativeMenuItemScope = nativeMainMenuScope;

      foreach (var menuItem in menuItems)
      {
        nativeMenuItemScope = nativeMenuItemScope.FindElement (By.XPath (string.Format ("ul/li[a[contains(.,'{0}')]]", menuItem)));
        actions.WaitFor (nativeMenuItemScope, ActionsWithWaitSupport.IsVisible, timeout);
        actions.MoveToElement (nativeMenuItemScope);
      }

      return nativeMenuItemScope;
    }
  }
}
﻿using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class regarding <see cref="ElementScope.SelectOption"/>.
  /// </summary>
  public static class CoypuElementScopeSelectExtensions
  {
    /// <summary>
    /// Returns the text of the currently selected option. If more than one option is selected, this method returns the first selected item's text.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <returns>The text of the currently selected option.</returns>
    public static string GetSelectedOptionText ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      // Hack: Coypu does not yet support SelectElement, use Selenium directly.
      return RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;

            var select = new SelectElement (webElement);
            return select.SelectedOption.Text;
          });
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <paramref name="index"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="index">The index of the option to select.</param>
    public static void SelectOptionByIndex ([NotNull] this ElementScope scope, int index)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      // Hack: Coypu does not yet support SelectElement, use Selenium directly.
      RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;

            var select = new SelectElement (webElement);
            select.SelectByIndex (index - 1);
          });
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <paramref name="value"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to select.</param>
    public static void SelectOptionByValue ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);

      // Hack: Coypu does not yet support SelectElement, use Selenium directly.
      RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;

            var select = new SelectElement (webElement);
            select.SelectByValue (value);
          });
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <see cref="DiagnosticMetadataAttributes"/> given by
    /// <paramref name="diagnosticMetadataAttributeName"/> and <paramref name="diagnosticMetadataAttributeValue"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="diagnosticMetadataAttributeName">The diagnostic metadata attribute name.</param>
    /// <param name="diagnosticMetadataAttributeValue">The diagnostic metadata attribute value.</param>
    public static void SelectOptionByDMA (
        [NotNull] this ElementScope scope,
        [NotNull] string diagnosticMetadataAttributeName,
        [NotNull] string diagnosticMetadataAttributeValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeName", diagnosticMetadataAttributeName);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeValue", diagnosticMetadataAttributeValue);
      
      // Hack: Coypu does not yet support SelectElement, use Selenium directly.
      RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;

            var select = new SelectElement (webElement);
            foreach (var option in select.Options)
            {
              if (option.GetAttribute (diagnosticMetadataAttributeName) == diagnosticMetadataAttributeValue)
                select.SelectByValue (option.GetAttribute ("value"));
            }
          });
    }
  }
}
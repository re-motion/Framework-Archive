﻿using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Represents a generic page object which is returned by control object interactions (e.g. click on an HtmlAnchorControlObject). The user of
  /// the framework must specify which actual page object he is expecting by calling one of the various Expect* methods or extension methods.
  /// </summary>
  public class UnspecifiedPageObject : WebTestObject<ControlObjectContext>
  {
    /// <param name="context">Context of the <see cref="ControlObject"/> which triggered the interaction.</param>
    public UnspecifiedPageObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/>. It is implicitly assumed that the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> () where TPageObject : PageObject
    {
      return Expect<TPageObject> (po => true);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/>. A conditon given by <paramref name="pageIsShownAssertion"/> checks
    /// whether the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> ([NotNull] Func<TPageObject, bool> pageIsShownAssertion) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewPage();
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new window with title <paramref name="windowLocator"/>. It
    /// is implicitly assumed that the actual page on that window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new window (or a uniquely identifying part of the title).</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewWindow<TPageObject> ([NotNull] string windowLocator)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);
      
      return ExpectNewWindow<TPageObject> (windowLocator, po => true);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new window with title <paramref name="windowLocator"/>. A
    /// conditon given by <paramref name="pageIsShownAssertion"/> checks whether the actual page on that window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new window (or a uniquely identifying part of the title).</param>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewWindow<TPageObject> ([NotNull] string windowLocator, [NotNull] Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewWindow (windowLocator);
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new popup window with title
    /// <paramref name="windowLocator"/>. It is implicitly assumed that the actual page on that window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new popup window (or a uniquely identifying part of the title).</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewPopupWindow<TPageObject> ([NotNull] string windowLocator) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return ExpectNewPopupWindow<TPageObject> (windowLocator, po => true);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new popup window with title
    /// <paramref name="windowLocator"/>. A conditon given by <paramref name="pageIsShownAssertion"/> checks whether the actual page on that popup window
    /// matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new popup window (or a uniquely identifying part of the title).</param>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewPopupWindow<TPageObject> ([NotNull] string windowLocator, [NotNull] Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewPopupWindow (windowLocator);
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    private static TPageObject AssertPageIsShownAndReturnNewPageObject<TPageObject> (
        WebTestObjectContext newContext,
        Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      var pageObject = (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { newContext });
      pageIsShownAssertion (pageObject);
      return pageObject;
    }
  }
}
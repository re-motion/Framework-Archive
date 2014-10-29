﻿using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocDateTimeValueControlObject : BocControlObject
  {
    private readonly bool _hasTimeField;

    public BocDateTimeValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      _hasTimeField = Scope[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField] == "true";
    }

    /// <summary>
    /// Returns the current <see cref="DateTime"/> respresented by the control or null if an invalid <see cref="DateTime"/> is displayed.
    /// </summary>
    /// <returns>The DateTime contains </returns>
    public DateTime? GetDateTime ()
    {
      DateTime result;
      string dateTimeString;

      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
      {
        dateTimeString = Scope.FindCss ("span:nth-child(1)").Text;
        if (_hasTimeField)
          dateTimeString += " " + Scope.FindCss ("span:nth-child(2)").Text;
      }
      else
      {
        dateTimeString = GetDateScope().Value;
        if (_hasTimeField)
          dateTimeString += " " + GetTimeScope().Value;
      }

      if (DateTime.TryParse (dateTimeString, out result))
        return result;

      return null;
    }

    /// <summary>
    /// Sets the date component of the control to <paramref name="newDate"/>.
    /// </summary>
    public UnspecifiedPageObject SetDate (DateTime newDate, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      var newDateString = newDate.ToShortDateString();
      return SetDate (newDateString, actionBehavior);
    }

    /// <summary>
    /// Sets the date component of the control to <paramref name="newDateString"/>.
    /// </summary>
    public UnspecifiedPageObject SetDate (string newDateString, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      var dateScope = GetDateScope();

      var actualActionBehavior = GetActualActionBehavior (dateScope, actionBehavior);
      dateScope.FillWithAndWait (newDateString, Then.TabAway, Context, actualActionBehavior);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Sets the time component of the control to <paramref name="newTime"/>.
    /// </summary>
    public UnspecifiedPageObject SetTime (TimeSpan newTime, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      var newTimeAsDateTime = DateTime.MinValue.Add (newTime);

      string newTimeString;

      var timeScope = GetTimeScope();
      if (timeScope[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds] == "true")
        newTimeString = newTimeAsDateTime.ToLongTimeString();
      else
        newTimeString = newTimeAsDateTime.ToShortTimeString();

      return SetTime (newTimeString, actionBehavior);
    }

    /// <summary>
    /// Sets the time component of the control to <paramref name="newTimeString"/>.
    /// </summary>
    public UnspecifiedPageObject SetTime (string newTimeString, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      var timeScope = GetTimeScope();

      var actualActionBehavior = GetActualActionBehavior (timeScope, actionBehavior);
      timeScope.FillWithAndWait (newTimeString, Then.TabAway, Context, actualActionBehavior);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Sets the date component and the time component of the control to <paramref name="newDateTime"/>.
    /// </summary>
    public UnspecifiedPageObject SetDateTime (DateTime newDateTime, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      SetDate (newDateTime, actionBehavior);
      if(_hasTimeField)
        SetTime (newDateTime.TimeOfDay, actionBehavior);
      return UnspecifiedPage();
    }

    private ElementScope GetDateScope ()
    {
      return Scope.FindDMA ("input", DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField, "true");
    }

    private ElementScope GetTimeScope ()
    {
      return Scope.FindDMA ("input", DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField, "true");
    }
  }
}
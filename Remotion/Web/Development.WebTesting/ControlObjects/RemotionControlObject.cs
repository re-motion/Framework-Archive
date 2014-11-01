﻿using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based control.
  /// </summary>
  public abstract class RemotionControlObject : ControlObject
  {
    /// <summary>
    /// Initializes the control object with the given <paramref name="context"/>.
    /// </summary>
    protected RemotionControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns a child element of the control, specified by an <paramref name="idSuffix"/> parameter.
    /// </summary>
    protected ElementScope FindChild ([NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      return FindChild (GetHtmlID(), idSuffix);
    }

    protected ElementScope FindChild ([NotNull] string id, [NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      var fullId = string.Format ("{0}_{1}", id, idSuffix);
      return Scope.FindId (fullId);
    }

    /// <summary>
    /// Returns the actual <see cref="ICompletionDetection"/> to be used when acting on the control object's scope.
    /// </summary>
    /// <param name="userDefinedCompletionDetection">User-provided <see cref="ICompletionDetection"/>.</param>
    /// <returns><see cref="ICompletionDetection"/> to be used.</returns>
    protected ICompletionDetector DetermineActualCompletionDetection ([CanBeNull] ICompletionDetection userDefinedCompletionDetection)
    {
      return DetermineActualCompletionDetection (Scope, userDefinedCompletionDetection);
    }

    /// <summary>
    /// Returns the actual <see cref="ICompletionDetection"/> to be used.
    /// </summary>
    /// <param name="scope">Scope which is to be acted on.</param>
    /// <param name="userDefinedCompletionDetection">User-provided <see cref="ICompletionDetection"/>.</param>
    /// <returns><see cref="ICompletionDetection"/> to be used.</returns>
    protected ICompletionDetector DetermineActualCompletionDetection ([NotNull] ElementScope scope, [CanBeNull] ICompletionDetection userDefinedCompletionDetection)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      if (userDefinedCompletionDetection != null)
        return userDefinedCompletionDetection.Build();

      if (scope[DiagnosticMetadataAttributes.TriggersPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersPostBack]);
        if (hasAutoPostBack)
          return Continue.When (Wxe.PostBackCompleted).Build();
      }

      if (scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return Continue.When (Wxe.Reset).Build();
      }

      return Continue.Immediately().Build();
    }
  }
}
using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListCellControlObject : BocControlObject, ICommandHost
  {
    private readonly BocListCellFunctionality _impl;

    public BocListCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _impl = new BocListCellFunctionality (id, context);
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return _impl.GetText();
    }

    public CommandControlObject GetCommand ()
    {
      return _impl.GetCommand();
    }

    public UnspecifiedPageObject ExecuteCommand (ICompletionDetection completionDetection = null)
    {
      return _impl.ExecuteCommand (completionDetection);
    }
  }
}
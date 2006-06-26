using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  public class TestCommand : Command
  {
    // types

    // static members

    // member fields

    private bool? _active = null;
	
    // construction and disposing

    public TestCommand ()
    {
    }

    // methods and properties

    public bool? Active
    {
      get { return _active; }
      set { _active = value; }
    }

    public override bool IsActive ()
    {
      if (_active.HasValue)
        return _active.Value;

      return base.IsActive ();
    }
  }
}
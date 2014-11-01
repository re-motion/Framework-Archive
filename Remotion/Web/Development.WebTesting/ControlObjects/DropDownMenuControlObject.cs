﻿using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for the default <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public class DropDownMenuControlObject : DropDownMenuControlObjectBase
  {
    public DropDownMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected override void OpenDropDownMenu ()
    {
      var dropDownMenuButtonScope = Scope.FindCss ("a.DropDownMenuButton");
      dropDownMenuButtonScope.Click();
    }
  }
}
﻿using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocMultilineTextValueControlObject"/>.
  /// </summary>
  public class BocMultilineTextValueSelector : BocSelectorBase<BocMultilineTextValueControlObject>
  {
    public BocMultilineTextValueSelector ()
        : base ("span", "bocMultilineTextValue")
    {
    }
  }
}
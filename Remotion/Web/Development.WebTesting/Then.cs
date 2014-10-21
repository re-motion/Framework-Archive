﻿using System;
using Coypu;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting
{
  public static class Then
  {
    public static readonly ThenAction DoNothing = s => { };
    public static readonly ThenAction TabAway = s => s.SendKeysFixed (Keys.Tab);
    // Todo RM-6297: Why does PressEnter not trigger an auto postback in IE? Is this a bug? See BocListCO.GoToSpecificPage().
    public static readonly ThenAction PressEnter = s => s.SendKeysFixed (Keys.Enter);
  }

  public delegate void ThenAction (ElementScope scope);
}
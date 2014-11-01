﻿using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaPageObject : AppToolsPageObject
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public ActaNovaPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }
  }
}
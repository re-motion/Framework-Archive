﻿using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public abstract class ActaNovaPageObject : PageObject
  {
    protected ActaNovaPageObject ([NotNull] PageObjectContext context)
        : base(context)
    {
    }
  }
}
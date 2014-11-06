﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class of <see cref="PageObject"/> and <see cref="ControlObject"/>, providing common state and logic.
  /// </summary>
  public abstract class WebTestObject<TWebTestObjectContext>
      where TWebTestObjectContext : WebTestObjectContext
  {
    private readonly TWebTestObjectContext _context;

    protected WebTestObject ([NotNull] TWebTestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    /// <summary>
    /// The web test object's <see cref="WebTestObjectContext"/>.
    /// </summary>
    public TWebTestObjectContext Context
    {
      get { return _context; }
    }

    /// <summary>
    /// Shortcut for <see cref="Context"/>.<see cref="WebTestObjectContext.Scope"/>.
    /// </summary>
    public ElementScope Scope
    {
      get { return Context.Scope; }
    }
  }
}
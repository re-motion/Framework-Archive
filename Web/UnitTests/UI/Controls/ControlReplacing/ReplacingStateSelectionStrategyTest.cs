/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class ReplacingStateSelectionStrategyTest : TestBase
  {
    private ControlReplacer _replacer;
    private IStateModificationStrategy _stateModificationStrategy;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _replacer = new ControlReplacer (MemberCallerMock);

      Pair state = new Pair (new Hashtable(), new object());
      LosFormatter formatter = new LosFormatter ();
      StringWriter writer = new StringWriter ();
      formatter.Serialize (writer, state);

      _stateModificationStrategy = new ReplacingStateSelectionStrategy (writer.ToString ());
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (((ReplacingStateSelectionStrategy)_stateModificationStrategy).ViewState, Is.Not.Null);
      Assert.That (((ReplacingStateSelectionStrategy) _stateModificationStrategy).ControlState, Is.Not.Null);
    }

    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add (testPageHolder.NamingContainer);

      MemberCallerMock.Expect (mock => mock.SetChildControlState (Arg<ControlReplacer>.Is.Same (_replacer), Arg<Hashtable>.Is.NotNull));
      MockRepository.ReplayAll ();

      _stateModificationStrategy.LoadControlState (_replacer, MemberCallerMock);

      MockRepository.VerifyAll ();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add (testPageHolder.NamingContainer);

      MemberCallerMock.Expect (mock => mock.LoadViewStateRecursive (Arg<ControlReplacer>.Is.Same (_replacer), Arg<Hashtable>.Is.NotNull));

      MockRepository.ReplayAll ();

      _stateModificationStrategy.LoadViewState (_replacer, MemberCallerMock);

      MemberCallerMock.VerifyAllExpectations ();
    }
  }
}
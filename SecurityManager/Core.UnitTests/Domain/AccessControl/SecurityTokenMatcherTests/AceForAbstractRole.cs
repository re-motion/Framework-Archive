// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForAbstractRole : SecurityTokenMatcherTestBase
  {
    [Test]
    public void TokenWithAbstractRole_Matches ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithOtherAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (TestHelper.CreateTestAbstractRole ());

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}

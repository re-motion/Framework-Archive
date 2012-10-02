// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ArgumentTypeExceptionTest
  {
    [Test]
    public void Initialization ()
    {
      var exception = new ArgumentTypeException ("Msg", "arg", typeof (string), typeof (int));

      Assert.That (exception.ExpectedType, Is.SameAs (typeof (string)));
      Assert.That (exception.ActualType, Is.SameAs (typeof (int)));
      Assert.That (exception.ParamName, Is.EqualTo ("arg"));
      Assert.That (exception.Message, Is.EqualTo ("Msg\r\nParameter name: arg"));
    }

    [Test]
    public void Initialization_WithoutMessage_WithExpectedType ()
    {
      var exception = new ArgumentTypeException ("arg", typeof (string), typeof (int));
      Assert.That (exception.Message, Is.EqualTo ("Argument arg has type System.Int32 when type System.String was expected.\r\nParameter name: arg"));
    }

    [Test]
    public void Initialization_WithoutMessage_WithoutExpectedType ()
    {
      var exception = new ArgumentTypeException ("arg", typeof (int));
      Assert.That (exception.Message, Is.EqualTo ("Argument arg has unexpected type System.Int32.\r\nParameter name: arg"));
    }

    [Test]
    public void Serialization ()
    {
      var exception = new ArgumentTypeException ("Msg", "arg", typeof (string), typeof (int));
      
      var deserializedException = Serializer.SerializeAndDeserialize (exception);

      Assert.That (deserializedException.ExpectedType, Is.SameAs (typeof (string)));
      Assert.That (deserializedException.ActualType, Is.SameAs (typeof (int)));
      Assert.That (deserializedException.ParamName, Is.EqualTo ("arg"));
      Assert.That (exception.Message, Is.EqualTo ("Msg\r\nParameter name: arg"));
    }
  }
}
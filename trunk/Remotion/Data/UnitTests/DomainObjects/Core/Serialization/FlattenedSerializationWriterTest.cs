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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class FlattenedSerializationWriterTest
  {
    [Test]
    public void InitialWriter ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      int[] data = writer.GetData();
      Assert.IsNotNull (data);
      Assert.IsEmpty (data);
    }

    [Test]
    public void AddSimpleValue ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1 }));
    }

    [Test]
    public void AddSimpleValue_Twice ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      writer.AddSimpleValue (2);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1, 2 }));
    }
  }
}

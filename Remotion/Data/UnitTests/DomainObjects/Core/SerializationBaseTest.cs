// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class SerializationBaseTest : ClientTransactionBaseTest
  {
    protected object SerializeAndDeserialize (object graph)
    {
      using (MemoryStream stream = new MemoryStream ())
      {
        Serialize (stream, graph);
        return Deserialize (stream);
      }
    }

    protected void Serialize (Stream stream, object graph)
    {
      BinaryFormatter serializationFormatter = new BinaryFormatter ();
      serializationFormatter.Serialize (stream, graph);
    }

    protected object Deserialize (Stream stream)
    {
      stream.Position = 0;

      BinaryFormatter deserializationFormatter = new BinaryFormatter ();
      return deserializationFormatter.Deserialize (stream);
    }
  }
}

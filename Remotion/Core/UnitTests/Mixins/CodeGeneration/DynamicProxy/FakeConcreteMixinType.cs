// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [IgnoreForMixinConfiguration]
  public class FakeConcreteMixinType : MixinWithAbstractMembers, ISerializable, IDeserializationCallback
  {
    public interface IOverriddenMethods
    {
    }

    public bool OnDeserializingCalled = false;
    public bool OnDeserializedCalled = false;
    public bool OnDeserializationCalled = false;
    public bool CtorCalled = true;
    public bool SerializationCtorCalled = false;

    public FakeConcreteMixinType ()
    {
    }

    protected FakeConcreteMixinType (SerializationInfo info, StreamingContext context)
    {
      SerializationCtorCalled = true;
    }

    [OnDeserializing]
    public void OnDeserializing (StreamingContext context)
    {
      OnDeserializingCalled = true;
    }

    [OnDeserialized]
    public void OnDeserialized (StreamingContext context)
    {
      OnDeserializedCalled = true;
    }

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException();
    }

    protected override string AbstractMethod (int i)
    {
      throw new NotImplementedException();
    }

    protected override string AbstractProperty
    {
      get { throw new NotImplementedException(); }
    }

    protected override event Func<string> AbstractEvent;
    protected override string RaiseEvent ()
    {
      throw new NotImplementedException();
    }
  }
}
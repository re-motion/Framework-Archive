// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  [TestFixture]
  public class CustomEventInfoTest
  {
    private MethodInfo _publicAddMethod;
    private MethodInfo _publicRemoveMethod;
    private MethodInfo _publicRaiseMethod;
    private MethodInfo _nonPublicAddMethod;
    private MethodInfo _nonPublicRemoveMethod;
    private MethodInfo _nonPublicRaiseMethod;

    [SetUp]
    public void SetUp ()
    {
      _publicAddMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.PublicAddMethod (null));
      _publicRemoveMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.PublicRemoveMethod (null));
      _publicRaiseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.PublicRaiseMethod ("", 7));
      _nonPublicAddMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.NonPublicAddMethod (null));
      _nonPublicRemoveMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.NonPublicRemoveMethod (null));
      _nonPublicRaiseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType o) => o.NonPublicRaiseMethod ("", 7));
    }

    [Test]
    public void Initialization ()
    {
      var declaringType = CustomTypeObjectMother.Create();
      var name = "Event";
      var attributes = (EventAttributes) 7;

      var result = new TestableCustomEventInfo (declaringType, name, attributes, _publicAddMethod, _publicRemoveMethod, _publicRaiseMethod);

      Assert.That (result.DeclaringType, Is.EqualTo (declaringType));
      Assert.That (result.Name, Is.EqualTo (name));
      Assert.That (result.Attributes, Is.EqualTo (attributes));
      Assert.That (result.GetAddMethod(), Is.SameAs (_publicAddMethod));
      Assert.That (result.GetRemoveMethod(), Is.SameAs (_publicRemoveMethod));
      Assert.That (result.GetRaiseMethod(), Is.SameAs (_publicRaiseMethod));
    }

    [Test]
    public void GetAddMethod ()
    {
      var event1 = CustomEventInfoObjectMother.Create (addMethod: _publicAddMethod, removeMethod: _publicRemoveMethod);
      var event2 = CustomEventInfoObjectMother.Create (addMethod: _nonPublicAddMethod, removeMethod: _nonPublicRemoveMethod);

      Assert.That (event1.GetAddMethod (true), Is.SameAs (_publicAddMethod));
      Assert.That (event1.GetAddMethod (false), Is.SameAs (_publicAddMethod));
      Assert.That (event2.GetAddMethod (true), Is.SameAs (_nonPublicAddMethod));
      Assert.That (event2.GetAddMethod (false), Is.Null);
    }

    [Test]
    public void GetRemoveMethod ()
    {
      var event1 = CustomEventInfoObjectMother.Create (removeMethod: _publicRemoveMethod, addMethod: _publicAddMethod);
      var event2 = CustomEventInfoObjectMother.Create (removeMethod: _nonPublicRemoveMethod, addMethod: _nonPublicAddMethod);

      Assert.That (event1.GetRemoveMethod (true), Is.SameAs (_publicRemoveMethod));
      Assert.That (event1.GetRemoveMethod (false), Is.SameAs (_publicRemoveMethod));
      Assert.That (event2.GetRemoveMethod (true), Is.SameAs (_nonPublicRemoveMethod));
      Assert.That (event2.GetRemoveMethod (false), Is.Null);
    }

    [Test]
    public void GetRaiseMethod ()
    {
      var event1 = CustomEventInfoObjectMother.Create (raiseMethod: _publicRaiseMethod);
      var event2 = CustomEventInfoObjectMother.Create (raiseMethod: _nonPublicRaiseMethod);

      Assert.That (event1.GetRaiseMethod (true), Is.SameAs (_publicRaiseMethod));
      Assert.That (event1.GetRaiseMethod (false), Is.SameAs (_publicRaiseMethod));
      Assert.That (event2.GetRaiseMethod (true), Is.SameAs (_nonPublicRaiseMethod));
      Assert.That (event2.GetRaiseMethod (false), Is.Null);
    }

    [Test]
    public void CustomAttributeMethods ()
    {
      var event_ = CustomEventInfoObjectMother.Create (
          customAttributes: new[] { CustomAttributeDeclarationObjectMother.Create (typeof (ObsoleteAttribute)) });

      Assert.That (event_.GetCustomAttributes (false).Select (a => a.GetType()), Is.EqualTo (new[] { typeof (ObsoleteAttribute) }));
      Assert.That (event_.GetCustomAttributes (typeof (NonSerializedAttribute), false), Is.Empty);

      Assert.That (event_.IsDefined (typeof (ObsoleteAttribute), false), Is.True);
      Assert.That (event_.IsDefined (typeof (NonSerializedAttribute), false), Is.False);
    }

    [Test]
    public new void ToString ()
    {
      var name = "MyEvent";
      var event_ = CustomEventInfoObjectMother.Create (name: name, addMethod: _publicAddMethod, removeMethod:_publicRemoveMethod);

      Assert.That (event_.ToString(), Is.EqualTo ("MyEventDelegate MyEvent"));
    }

    [Test]
    public void ToDebugString ()
    {
      var declaringType = CustomTypeObjectMother.Create (name: "MyType");
      var name = "MyEvent";
      var event_ = CustomEventInfoObjectMother.Create (declaringType, name, addMethod: _publicAddMethod, removeMethod: _publicRemoveMethod);

      var expected = "TestableCustomEvent = \"MyEventDelegate MyEvent\", DeclaringType = \"MyType\"";

      Assert.That (event_.ToDebugString(), Is.EqualTo (expected));
    }

    [Test]
    public void UnsupportedMembers ()
    {
      var event_ = CustomEventInfoObjectMother.Create();

      UnsupportedMemberTestHelper.CheckProperty (() => event_.ReflectedType, "ReflectedType");
      UnsupportedMemberTestHelper.CheckMethod (() => event_.GetOtherMethods (true), "GetOtherMethods");
    }

    delegate void MyEventDelegate (string arg1, int arg2);

    class DomainType
    {
      // ReSharper disable UnusedParameter.Local
      public void PublicAddMethod (MyEventDelegate delegate_) { }

      public void PublicRemoveMethod (MyEventDelegate delegate_) { }
      public void PublicRaiseMethod (string arg1, int arg2) { }

      internal void NonPublicAddMethod (MyEventDelegate delegate_) { }
      internal void NonPublicRemoveMethod (MyEventDelegate delegate_) { }
      internal void NonPublicRaiseMethod (string arg1, int arg2) { }
      // ReSharper restore UnusedParameter.Local
    }
  }
}
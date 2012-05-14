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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeMethodCollectionTest
  {
    private MutableType _declaringType;

    private MutableTypeMethodCollection _collection;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      Func<MethodInfo, MutableMethodInfo> mutableMemberProvider = mi => MutableMethodInfoObjectMother.CreateForExisting (_declaringType, mi);

      _collection = new MutableTypeMethodCollection (_declaringType, typeof (DomainType).GetMethods (), mutableMemberProvider);
    }

    [Test]
    public void GetEnumerator_FiltersOverriddenBaseMembers ()
    {
      var overriddenMethod = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.BaseMethod());
      Assert.That (_collection.ToArray (), Has.Member (overriddenMethod));
      
      var overridingMethod = MutableMethodInfoObjectMother.CreateForNew (_declaringType, baseMethod: overriddenMethod);
      _collection.Add (overridingMethod);

      var enumeratedMethods = _collection.ToArray();

      Assert.That (enumeratedMethods, Has.Member (overridingMethod));
      Assert.That (enumeratedMethods, Has.No.Member (overriddenMethod));
    }

    [Test]
    public void GetMutableMember_StandardMemberInfo_BaseDeclaringType ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.BaseMethod());

      var result = _collection.GetMutableMember (method);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method is declared by a type outside of this type's class hierarchy: 'IDomainInterface'.\r\nParameter name: method")]
    public void GetMutableMember_Interface ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.InterfaceMethod());
      _collection.GetMutableMember (method);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method is declared by a type outside of this type's class hierarchy: 'String'.\r\nParameter name: method")]
    public void GetMutableMember_UnrelatedDeclaringType ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((string obj) => obj.Trim());
      _collection.GetMutableMember (method);
    }

    public class DomainTypeBase
    {
      public virtual void BaseMethod () { }
    }

    public interface IDomainInterface
    {
      void InterfaceMethod ();
    }

    public class DomainType : DomainTypeBase, IDomainInterface
    {
      public void InterfaceMethod () { }
    }
  }
}
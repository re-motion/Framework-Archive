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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class UnderlyingInfoDescriptorBaseTest
  {
    [Test]
    public void Initialization ()
    {
      var member = ReflectionObjectMother.GetSomeMember();
      Func<ReadOnlyCollection<ICustomAttributeData>> customAttributeDataProvider = () => null;

      var descriptor = new TestableUnderlyingInfoDescriptorBase<MemberInfo> (member, "memberName or parameterName", customAttributeDataProvider);

      Assert.That (descriptor.UnderlyingSystemInfo, Is.SameAs (member));
      Assert.That (descriptor.Name, Is.EqualTo ("memberName or parameterName"));
      Assert.That (descriptor.CustomAttributeDataProvider, Is.SameAs (customAttributeDataProvider));
    }

    [Test]
    public void EmptyCustomAttributeProvider ()
    {
      var fieldContent = TestableUnderlyingInfoDescriptorBase<object>.GetEmptyProviderField();
      Assert.That (fieldContent.Invoke(), Is.Empty);
    }

    [Test]
    public void GetCustomAttributeProvider_MemberInfo ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember ((UnderlyingInfoDescriptorBaseTest obj) => obj.Method (null));

      var result = TestableUnderlyingInfoDescriptorBase<Dev.T>.GetCustomAttributeProvider (member);

      CheckSingleCustomAttributeProviderResult (result, "member", "xxx", 7);
    }

    [Test]
    public void GetCustomAttributeProvider_ParameterInfo ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((UnderlyingInfoDescriptorBaseTest obj) => obj.Method (null));
      var parameter = method.GetParameters().Single();

      var result = TestableUnderlyingInfoDescriptorBase<Dev.T>.GetCustomAttributeProvider (parameter);

      CheckSingleCustomAttributeProviderResult (result, "parameter", "yyy", 8);
    }

    private void CheckSingleCustomAttributeProviderResult (
    Func<ReadOnlyCollection<ICustomAttributeData>> result, string ctorArg, string namedArgProperty, int namedArgField)
    {
      var providerResult = result.Invoke ();

      Assert.That (providerResult, Has.Count.EqualTo (1));
      var attributeData = providerResult.Single ();

      var ctor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new AbcAttribute (""));
      Assert.That (attributeData.Constructor, Is.EqualTo (ctor));
      Assert.That (attributeData.ConstructorArguments, Is.EqualTo (new object[] { ctorArg }));
      Assert.That (attributeData.NamedArguments, Has.Count.EqualTo (2));
      Assert.That (
          attributeData.NamedArguments,
          Has.Some.Matches<ICustomAttributeNamedArgument> (x => x.MemberInfo.Name == "Property" && x.Value.Equals (namedArgProperty)));
      Assert.That (
          attributeData.NamedArguments,
          Has.Some.Matches<ICustomAttributeNamedArgument> (x => x.MemberInfo.Name == "Field" && x.Value.Equals (namedArgField)));
    }

    [Abc("member", Field = 7, Property = "xxx")]
    public void Method ([Abc ("parameter", Field = 8, Property = "yyy")]object parameter)
    {
    }

    public class AbcAttribute : Attribute
    {
      public int Field;

      public AbcAttribute (string ctorArg) { }

      public string Property { get; set; }
    }
  }
}
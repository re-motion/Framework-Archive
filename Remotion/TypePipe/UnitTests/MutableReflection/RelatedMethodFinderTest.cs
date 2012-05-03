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
using NUnit.Framework;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class RelatedMethodFinderTest
  {
    private RelatedMethodFinder _finder;

    [SetUp]
    public void SetUp ()
    {
      _finder = new RelatedMethodFinder();
    }

    [Test]
    public void FindFirstOverriddenMethod_SingleMatchingCandidate ()
    {
      var candidate1 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method (7, "7"));
      var candidate2 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method ("7", 7));
      var candidate3 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((OtherDomainType obj) => obj.Method (7, "7"));
      var signature = new MethodSignature (typeof (void), new[] { typeof (int), typeof(string)}, 0);

      var result = _finder.FindFirstOverriddenMethod ("Method", signature, new[] { candidate1, candidate2, candidate3 });

      Assert.That (result, Is.SameAs (candidate3));
    }

    [Test]
    public void FindFirstOverriddenMethod_MultipleMatchingCandidates ()
    {
      var candidate1 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method (7, "7"));
      var candidate2 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method ("7", 7));
      var candidate3 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method ("7", 7));
      var signature = new MethodSignature (typeof (void), new[] { typeof (string), typeof(int) }, 0);

      var result = _finder.FindFirstOverriddenMethod ("Method", signature, new[] { candidate1, candidate2, candidate3 });

      Assert.That (result, Is.SameAs (candidate2));
    }

    [Test]
    public void FindFirstOverriddenMethod_NoMatchingName ()
    {
      var candidate = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method (7, "7"));
      var signature = new MethodSignature (typeof (int), new[] { typeof (int), typeof (string) }, 0);
      Assert.That (signature, Is.EqualTo (MethodSignature.Create (candidate)));

      var result = _finder.FindFirstOverriddenMethod ("DoesNotExist", signature, new[] { candidate });

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindFirstOverriddenMethod_NoMatchingSignature ()
    {
      var candidate1 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method (7, "7"));
      var candidate2 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method ("7", 7));
      var candidate3 = MemberInfoFromExpressionUtility.GetMethodBaseDefinition ((DomainType obj) => obj.Method ("7", 7));
      var signature = new MethodSignature (typeof (int), new[] { typeof (string), typeof (int) }, 0);

      var result = _finder.FindFirstOverriddenMethod ("Method", signature, new[] { candidate1, candidate2, candidate3 });

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindFirstOverriddenMethod_NonVirtualMethodsAreIgnored ()
    {
      var candidate = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.NonVirtualMethod());
      var signature = new MethodSignature (typeof (void), Type.EmptyTypes, 0);
      Assert.That (signature, Is.EqualTo (MethodSignature.Create (candidate)));

      var result = _finder.FindFirstOverriddenMethod ("NonVirtualMethod", signature, new[] { candidate });

      Assert.That (result, Is.Null);
    }

    private class DomainType
    {
      public virtual int Method (int i, string s) { return 0; }
      public virtual void Method (string s, int i) { }
      public void NonVirtualMethod () { }
    }

    private class OtherDomainType
    {
      public virtual void Method (int i, string s) { }
    }
  }
}
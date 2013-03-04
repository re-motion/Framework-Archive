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
using System.Reflection;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation.MemberFactory;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation.MemberFactory
{
  [TestFixture]
  public class FieldFactoryTest
  {
    private FieldFactory _factory;

    private ProxyType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _factory = new FieldFactory();

      _proxyType = ProxyTypeObjectMother.Create();
    }

    [Test]
    public void CreateField ()
    {
      var field = _factory.CreateField (_proxyType, "_newField", typeof (string), FieldAttributes.FamANDAssem);

      Assert.That (field.DeclaringType, Is.SameAs (_proxyType));
      Assert.That (field.Name, Is.EqualTo ("_newField"));
      Assert.That (field.FieldType, Is.EqualTo (typeof (string)));
      Assert.That (field.Attributes, Is.EqualTo (FieldAttributes.FamANDAssem));
    }

    [Test]
    public void CreateField_ThrowsForInvalidFieldAttributes ()
    {
      var message = "The following FieldAttributes are not supported for fields: " +
                    "Literal, HasFieldMarshal, HasDefault, HasFieldRVA.\r\nParameter name: attributes";
      Assert.That (() => CreateField (_proxyType, FieldAttributes.Literal), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => CreateField (_proxyType, FieldAttributes.HasFieldMarshal), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => CreateField (_proxyType, FieldAttributes.HasDefault), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => CreateField (_proxyType, FieldAttributes.HasFieldRVA), Throws.ArgumentException.With.Message.EqualTo (message));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Field cannot be of type void.\r\nParameter name: type")]
    public void CreateField_VoidType ()
    {
      _factory.CreateField (_proxyType, "NotImportant", typeof (void), FieldAttributes.ReservedMask);
    }

    [Test]
    public void CreateField_ThrowsIfAlreadyExist ()
    {
      var field = _proxyType.AddField ("Field", FieldAttributes.Private, typeof (int));

      Assert.That (
          () => _factory.CreateField (_proxyType, "OtherName", field.FieldType, 0),
          Throws.Nothing);

      Assert.That (
          () => _factory.CreateField (_proxyType, field.Name, typeof (string), 0),
          Throws.Nothing);

      Assert.That (
          () => _factory.CreateField (_proxyType, field.Name, field.FieldType, 0),
          Throws.InvalidOperationException.With.Message.EqualTo ("Field with equal name and signature already exists."));
    }

    private MutableFieldInfo CreateField (ProxyType proxyType, FieldAttributes attributes)
    {
      return _factory.CreateField (proxyType, "dummy", typeof (int), attributes);
    }
  }
}
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

namespace TypePipe.IntegrationTests
{
  [TestFixture]
  public class AddFieldTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void PrivateInstance ()
    {
      Assert.That (GetAllFieldNames (typeof (OriginalType)), Is.EquivalentTo (new[] { "OriginalField" }));

      var type = AssembleType<OriginalType> (mutableType => mutableType.AddField (typeof (string), "_privateInstanceField", FieldAttributes.Private));

      Assert.That (GetAllFieldNames (type), Is.EquivalentTo (new[] { "OriginalField", "_privateInstanceField" }));

      var fieldInfo = type.GetField ("_privateInstanceField", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.IsNotNull (fieldInfo);
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (string)));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
    }

    [Test]
    public void PublicStaticInitOnly ()
    {
      Assert.That (GetAllFieldNames (typeof (OriginalType)), Is.EquivalentTo (new[] { "OriginalField" }));

      var fieldAttributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly;
      var type = AssembleType<OriginalType> (mutableType => mutableType.AddField (typeof (int), "PublicStaticField", fieldAttributes));

      Assert.That (GetAllFieldNames (type), Is.EquivalentTo (new[] { "OriginalField", "PublicStaticField" }));

      var fieldInfo = type.GetField ("PublicStaticField");
      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (int)));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (fieldAttributes));
    }

    [Test]
    public void WithCustomAttribute ()
    {
      var type = AssembleType<OriginalType> (
          mutableType =>
          {
            var mutableFieldInfo = mutableType.AddField (typeof (int), "_fieldWithCustomAttributes");

            var attributeCtor = typeof (AddedAttribute).GetConstructor (new[] { typeof (string) });
            var namedProperty = typeof (AddedAttribute).GetProperty ("NamedPropertyArg");
            var namedField = typeof (AddedAttribute).GetField ("NamedFieldArg");
            var customAttributeDeclaration = new CustomAttributeDeclaration (
                attributeCtor,
                new[] { "ctorArg" },
                new NamedAttributeArgumentDeclaration (namedProperty, 7),
                new NamedAttributeArgumentDeclaration (namedField, new[] { MyEnum.Other, MyEnum.Default }));
            mutableFieldInfo.AddCustomAttribute (customAttributeDeclaration);
          });

      var fieldInfo = type.GetField ("_fieldWithCustomAttributes", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.IsNotNull (fieldInfo);
      var customAttributes = fieldInfo.GetCustomAttributes (false);
      Assert.That (customAttributes, Has.Length.EqualTo (1));

      var customAttribute = (AddedAttribute) customAttributes.Single();
      Assert.That(customAttribute.CtorArg, Is.EqualTo("ctorArg"));
      Assert.That (customAttribute.NamedPropertyArg, Is.EqualTo (7));
      Assert.That (customAttribute.NamedFieldArg, Is.EqualTo (new[] { MyEnum.Other, MyEnum.Default }));
    }

    private string[] GetAllFieldNames (Type type)
    {
      return type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
          .Select (field => field.Name)
          .ToArray (); // better error message
    }

    public class OriginalType
    {
      // protected so that Reflection on the subclass proxy will return the field
      protected object OriginalField;
    }

    public class AddedAttribute : Attribute
    {
      public MyEnum[] NamedFieldArg;

      private readonly string _ctorArg;

      public AddedAttribute (string ctorArg)
      {
        _ctorArg = ctorArg;
      }

      public string CtorArg
      {
        get { return _ctorArg; }
      }

      public int NamedPropertyArg { get; set; }
    }

    public enum MyEnum
    {
      Default,
      Other
    }
  }
}
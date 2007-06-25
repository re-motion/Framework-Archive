using System;
using NUnit.Framework;
using System.Reflection;
using Mixins.Utilities;
using System.Reflection.Emit;
using NUnit.Framework.SyntaxHelpers;

namespace Mixins.UnitTests.Utilities
{
  [TestFixture]
  public class ReflectionEmitUtilityTests
  {
    public class AttributeWithParams : Attribute
    {
      public AttributeWithParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
      {
      }

      public int INamed { get { return 0; } set { } }
      public string SNamed { get { return null; } set { } }
      public object ONamed { get { return null; } set { } }
      public Type TNamed { get { return null; }  set { } }

      public int[] INamedArray { get { return null; }  set { } }
      public string[] SNamedArray { get { return null; }  set { } }
      public object[] ONamedArray { get { return null; }  set { } }
      public Type[] TNamedArray { get { return null; }  set { } }

      public int INamedF;
      public string SNamedF;
      public object ONamedF;
      public Type TNamedF;

      public int[] INamedArrayF;
      public string[] SNamedArrayF;
      public object[] ONamedArrayF;
      public Type[] TNamedArrayF;
    }

    [AttributeWithParams (1, "1", null, typeof (object),
      new int[]{2, 3}, new string[] {"2", "3"}, new object[] {null, "foo", typeof (object)}, new Type[] {typeof (string), typeof (int), typeof(double)},
      
      INamed = 5, SNamed = "5", ONamed = "bla", TNamed = typeof (float),
      INamedArray = new int[] {1, 2, 3}, SNamedArray = new string[] {"1", null, "2"}, ONamedArray = new object[] {1, 2, null}, TNamedArray = new Type[] {typeof (Random), null},
    
      INamedF = 5, SNamedF = "5", ONamedF = "bla", TNamedF = typeof (float),
    INamedArrayF = new int[] { 1, 2, 3 }, SNamedArrayF = new string[] { "1", null, "2" }, ONamedArrayF = new object[] { 1, 2, null }, TNamedArrayF = new Type[] { typeof (Random), null })
    ]
    public class TestAttributeApplication
    {
    }

    [Test]
    public void CreateAttributeBuilderFromData ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplication))[0];
      ReflectionEmitUtility.CustomAttributeBuilderData data = ReflectionEmitUtility.GetCustomAttributeBuilderData (cad);

      Assert.AreEqual (8, data.ConstructorArgs.Length);

      Assert.AreEqual (1, data.ConstructorArgs[0]);
      Assert.AreEqual ("1", data.ConstructorArgs[1]);
      Assert.AreEqual (null, data.ConstructorArgs[2]);
      Assert.AreEqual (typeof(object), data.ConstructorArgs[3]);

      Assert.That (data.ConstructorArgs[4], Is.EquivalentTo (new int[] {2, 3}));
      Assert.That (data.ConstructorArgs[5], Is.EquivalentTo (new string[] {"2", "3"}));
      Assert.That (data.ConstructorArgs[6], Is.EquivalentTo (new object[] {null, "foo", typeof (object)}));
      Assert.That (data.ConstructorArgs[7], Is.EquivalentTo (new Type[] { typeof (string), typeof (int), typeof (double) }));


      Assert.AreEqual (8, data.NamedProperties.Length);

      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("INamed"), data.NamedProperties[0]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("SNamed"), data.NamedProperties[1]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("ONamed"), data.NamedProperties[2]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("TNamed"), data.NamedProperties[3]);

      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("INamedArray"), data.NamedProperties[4]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("SNamedArray"), data.NamedProperties[5]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("ONamedArray"), data.NamedProperties[6]);
      Assert.AreEqual (typeof (AttributeWithParams).GetProperty ("TNamedArray"), data.NamedProperties[7]);

      Assert.AreEqual (8, data.PropertyValues.Length);

      Assert.AreEqual (5, data.PropertyValues[0]);
      Assert.AreEqual ("5", data.PropertyValues[1]);
      Assert.AreEqual ("bla", data.PropertyValues[2]);
      Assert.AreEqual (typeof (float), data.PropertyValues[3]);

      Assert.That (data.PropertyValues[4], Is.EquivalentTo (new int[] {1, 2, 3}));
      Assert.That (data.PropertyValues[5], Is.EquivalentTo (new string[] {"1", null, "2"}));
      Assert.That (data.PropertyValues[6], Is.EquivalentTo (new object[] {1, 2, null}));
      Assert.That (data.PropertyValues[7], Is.EquivalentTo (new Type[] {typeof (Random), null}));


      Assert.AreEqual (8, data.NamedFields.Length);

      Assert.AreEqual (typeof (AttributeWithParams).GetField ("INamedF"), data.NamedFields[0]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("SNamedF"), data.NamedFields[1]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("ONamedF"), data.NamedFields[2]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("TNamedF"), data.NamedFields[3]);

      Assert.AreEqual (typeof (AttributeWithParams).GetField ("INamedArrayF"), data.NamedFields[4]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("SNamedArrayF"), data.NamedFields[5]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("ONamedArrayF"), data.NamedFields[6]);
      Assert.AreEqual (typeof (AttributeWithParams).GetField ("TNamedArrayF"), data.NamedFields[7]);

      Assert.AreEqual (8, data.FieldValues.Length);

      Assert.AreEqual (5, data.FieldValues[0]);
      Assert.AreEqual ("5", data.FieldValues[1]);
      Assert.AreEqual ("bla", data.FieldValues[2]);
      Assert.AreEqual (typeof (float), data.FieldValues[3]);

      Assert.That (data.FieldValues[4], Is.EquivalentTo (new int[] { 1, 2, 3 }));
      Assert.That (data.FieldValues[5], Is.EquivalentTo (new string[] { "1", null, "2" }));
      Assert.That (data.FieldValues[6], Is.EquivalentTo (new object[] { 1, 2, null }));
      Assert.That (data.FieldValues[7], Is.EquivalentTo (new Type[] { typeof (Random), null }));



      Assert.AreEqual (1, data.ConstructorArgs[0]);
      Assert.AreEqual ("1", data.ConstructorArgs[1]);
      Assert.AreEqual (null, data.ConstructorArgs[2]);
      Assert.AreEqual (typeof (object), data.ConstructorArgs[3]);

      Assert.That (data.ConstructorArgs[4], Is.EquivalentTo (new int[] { 2, 3 }));
      Assert.That (data.ConstructorArgs[5], Is.EquivalentTo (new string[] { "2", "3" }));
      Assert.That (data.ConstructorArgs[6], Is.EquivalentTo (new object[] { null, "foo", typeof (object) }));
      Assert.That (data.ConstructorArgs[7], Is.EquivalentTo (new Type[] { typeof (string), typeof (int), typeof (double) }));

    }
  }
}

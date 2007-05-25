using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using Mixins.Utilities.Serialization;
using NUnit.Framework;
using System.Runtime.Serialization;
using Rubicon;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using AssertionException=NUnit.Framework.AssertionException;

namespace Mixins.UnitTests.Utilities.Serialization
{
  [TestFixture]
  public class SafeReflectionSerializationTests
  {
    [Serializable]
    class SerializationTester<T> : ISerializable
    {
      public readonly T Value;
      public static Proc<T, string, SerializationInfo> Serializer;
      public static Func<string, SerializationInfo, T> Deserializer;

      public SerializationTester (T value)
      {
        ArgumentUtility.CheckNotNull ("value", value);
        Value = value;
      }

      protected SerializationTester (SerializationInfo info, StreamingContext context)
      {
        Value = Deserializer ("Value", info);
        Assert.IsNotNull (Value);
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        Serializer (Value, "Value", info);
      }
    }

    private static T PerformSerialization<T> (T value, Proc<T, string, SerializationInfo> serializer, Func<string, SerializationInfo, T> deserializer)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("serializer", serializer);
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);

      SerializationTester<T>.Serializer = serializer;
      SerializationTester<T>.Deserializer = deserializer;
      SerializationTester<T> tester = new SerializationTester<T>(value);
      return Serializer.SerializeAndDeserialize (tester).Value;
    }

    [Test]
    public void SerializeTypes()
    {
      Assert.AreEqual (typeof (object),
          PerformSerialization (typeof (object), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
      Assert.AreEqual (typeof (Dictionary<,>.Enumerator),
          PerformSerialization (typeof (Dictionary<,>.Enumerator), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
      Assert.AreEqual (typeof (Dictionary<int, string>.Enumerator),
          PerformSerialization (typeof (Dictionary<int, string>.Enumerator), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
    }

    public void SameName<T> () { }
    public void SameName () { }

    private static void TestMethodSerialization (MethodBase method)
    {
      Assert.AreEqual (method, PerformSerialization (method, ReflectionObjectSerializer.SerializeMethodBase,
          ReflectionObjectSerializer.DeserializeMethodBase));
    }

    [Test]
    public void SerializeMethods ()
    {
      TestMethodSerialization (typeof (object).GetMethod ("Equals", BindingFlags.Public | BindingFlags.Instance));
      TestMethodSerialization (typeof (Console).GetMethod ("WriteLine", new Type[] { typeof (string), typeof (object[]) }));
      TestMethodSerialization (typeof (SafeReflectionSerializationTests).GetMethod ("PerformSerialization", BindingFlags.NonPublic | BindingFlags.Static));

      MethodInfo[] methods =
        Array.FindAll (typeof (SafeReflectionSerializationTests).GetMethods (), delegate (MethodInfo m) { return m.Name == "SameName"; });
      Assert.AreEqual (2, methods.Length);

      TestMethodSerialization (methods[0]);
      TestMethodSerialization (methods[1]);

      TestMethodSerialization (typeof (GenericType<>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (SafeReflectionSerializationTests)));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));

      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (SafeReflectionSerializationTests)));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));

      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (SafeReflectionSerializationTests)));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));
    }

    [Test]
    public void SerializeConstructors()
    {
      TestMethodSerialization (typeof (GenericType<>).GetConstructor (Type.EmptyTypes));
      TestMethodSerialization (typeof (GenericType<int>).GetConstructor (Type.EmptyTypes));
      TestMethodSerialization (typeof (GenericType<object>).GetConstructor (Type.EmptyTypes));

      TestMethodSerialization (typeof (GenericType<>).GetConstructor (new Type[] {typeof (GenericType<>).GetGenericArguments()[0]}));
      TestMethodSerialization (typeof (GenericType<int>).GetConstructor (new Type[] {typeof (int)}));
      TestMethodSerialization (typeof (GenericType<object>).GetConstructor (new Type[] { typeof (object) }));
    }

    private static void TestPropertySerialization (PropertyInfo property)
    {
      throw new NotImplementedException();
      /*Assert.AreEqual (property, PerformSerialization (property, ReflectionObjectSerializer.SerializePropertyInfo,
          ReflectionObjectSerializer.DeserializePropertyInfo));*/
    }

    [Test]
    [Ignore("TODO: Add safe serialization support for properties")]
    public void SerializeProperties()
    {
      TestPropertySerialization (typeof (DateTime).GetProperty ("Now"));
      TestPropertySerialization (typeof (OrderedDictionary).GetProperty ("Item", new Type[] { typeof (int) }));
      TestPropertySerialization (typeof (OrderedDictionary).GetProperty ("Item", new Type[] { typeof (string) }));

      PropertyInfo[] properties = typeof (GenericType<>).GetProperties();
      Assert.AreEqual (7, properties.Length);

      foreach (PropertyInfo property in properties)
      {
        Console.WriteLine (property);
        TestPropertySerialization (property);
      }

      properties = typeof (GenericType<int>).GetProperties();
      Assert.AreEqual (7, properties.Length);

      foreach (PropertyInfo property in properties)
      {
        Console.WriteLine (property);
        TestPropertySerialization (property);
      }

      properties = typeof (GenericType<string>).GetProperties ();
      Assert.AreEqual (7, properties.Length);

      foreach (PropertyInfo property in properties)
      {
        Console.WriteLine (property);
        TestPropertySerialization (property);
      }

      properties = typeof (GenericType<DateTime>).GetProperties ();
      Assert.AreEqual (7, properties.Length);

      foreach (PropertyInfo property in properties)
      {
        Console.WriteLine (property);
        TestPropertySerialization (property);
      }

      properties = typeof (GenericType<SafeReflectionSerializationTests>).GetProperties ();
      Assert.AreEqual (7, properties.Length);

      foreach (PropertyInfo property in properties)
      {
        Console.WriteLine (property);
        TestPropertySerialization (property);
      }
    }

    /*

    [Serializable]
    class SerializableHolder<T> : Base<T>
    {
      private SerializableHolder (SerializationInfo info, StreamingContext context)
          : base (info, context)
      {
      }

      public SerializableHolder (T value)
          : base (value)
      {
      }
    }

    private static T PerformIndirectSerialization<T> (T value)
    {
      try
      {
        SerializableHolder<T> h = new SerializableHolder<T> (value);
        return Serializer.SerializeAndDeserialize (h).Value;
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    [ExpectedException(typeof(AssertionException))]
    public void TypeWithoutWrapper ()
    {
      PerformIndirectSerialization (typeof (object));
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void MethodInfoWithoutWrapper ()
    {
      PerformIndirectSerialization (typeof (object).GetMethod ("ToString"));
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void PropertyInfoWithoutWrapper ()
    {
      PerformIndirectSerialization (typeof (DateTime).GetProperty ("Now"));
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void EventInfoWithoutWrapper ()
    {
      PerformIndirectSerialization (typeof (AppDomain).GetEvent ("UnhandledException"));
    }

    [Serializable]
    class TestFunctionHolder
    {
      public int I;

      public int TestFunction ()
      {
        return I;
      }
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void DelegateWithoutWrapper ()
    {
      TestFunctionHolder holder = new TestFunctionHolder ();
      holder.I = 42;
      PerformIndirectSerialization (new Func<int> (holder.TestFunction));
    }

    [Test]
    public void TypeWithWrapper ()
    {
      Assert.AreEqual (typeof(object), (Type) PerformIndirectSerialization (new ReflectionObjectSerializer (typeof (object))));
    }*/
  }
}

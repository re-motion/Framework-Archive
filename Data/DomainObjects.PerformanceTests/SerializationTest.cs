using System;
using System.Diagnostics;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.PerformanceTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class SerializationTest : DatabaseTest
  {
    [Test]
    public void Serialize5ValuePropertyObjects ()
    {
      PerformSerializationTests ("Serialize5ValuePropertyObjects", 2, 5, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 5; ++i)
            CreateAndFillValuePropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize50ValuePropertyObjects ()
    {
      PerformSerializationTests ("Serialize50ValuePropertyObjects", 14, 14, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 50; ++i)
            CreateAndFillValuePropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize500ValuePropertyObjects ()
    {
      PerformSerializationTests ("Serialize500ValuePropertyObjects", 139, 211, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 500; ++i)
            CreateAndFillValuePropertyObject();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize1025ValuePropertyObjects ()
    {
      PerformSerializationTests ("Serialize1025ValuePropertyObjects", 281, 544, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 1025; ++i)
            CreateAndFillValuePropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize41RelationPropertyObjects ()
    {
      PerformSerializationTests ("Serialize41RelationPropertyObjects", 11, 9, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 1; ++i)
            CreateAndFillRelationPropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize410RelationPropertyObjects ()
    {
      PerformSerializationTests ("Serialize410RelationPropertyObjects", 114, 74, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 10; ++i)
            CreateAndFillRelationPropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    [Test]
    public void Serialize1025RelationPropertyObjects ()
    {
      PerformSerializationTests ("Serialize1025RelationPropertyObjects", 269, 219, delegate
      {
        using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < 25; ++i)
            CreateAndFillRelationPropertyObject ();
          return ClientTransaction.Current;
        }
      });
    }

    private void PerformSerializationTests (string nameOfTest, int expectedMSSerialization, int expectedMSDeserialization,
        Func<ClientTransaction> transactionInitializer)
    {
      const int numberOfTests = 10;

      Console.WriteLine ("Expected average duration of {0} on reference system: ~{1} ms/~{2} ms",
          nameOfTest, expectedMSSerialization, expectedMSDeserialization);

      Stopwatch serializationStopwatch = new Stopwatch ();
      Stopwatch deserializationStopwatch = new Stopwatch ();
      int dataContainers = 0;
      int relationEndPoints = 0;
      int dataSize = 0;

      for (int i = 0; i < numberOfTests; i++)
      {
        ClientTransaction transaction = transactionInitializer();
        DataManager dataManager = (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
        dataContainers += dataManager.DataContainerMap.Count;
        relationEndPoints += dataManager.RelationEndPointMap.Count;

        serializationStopwatch.Start ();
        byte[] data = Serializer.Serialize (transaction);
        serializationStopwatch.Stop ();

        deserializationStopwatch.Start ();
        Serializer.Deserialize (data);
        deserializationStopwatch.Stop ();
        
        dataSize += data.Length;
      }

      double serAverageMilliSeconds = (double)serializationStopwatch.ElapsedMilliseconds / numberOfTests;
      double deserAverageMilliSeconds = (double) deserializationStopwatch.ElapsedMilliseconds / numberOfTests;
      double averageDataContainers = ((double) dataContainers) / numberOfTests;
      double averageRelationEndPoints = ((double) relationEndPoints) / numberOfTests;
      double averageSize = ((double) dataSize) / numberOfTests;

      Console.WriteLine ("{0} (executed {1}x): Average duration: serialization {2} ms, deserialization {3} ms; data size {4} bytes, "
          + "{5} data containers, {6} relation end points", nameOfTest, numberOfTests, serAverageMilliSeconds.ToString ("n"),
          deserAverageMilliSeconds.ToString ("n"), averageSize.ToString ("n0"), averageDataContainers.ToString ("n0"),
          averageRelationEndPoints.ToString ("n0"));
    }

    private void CreateAndFillValuePropertyObject ()
    {
      Random random = new Random();
      ClassWithValueProperties instance = ClassWithValueProperties.NewObject().With();

      instance.BoolProperty1 = random.Next () % 2 == 0;
      instance.BoolProperty2 = random.Next () % 2 == 0;
      instance.BoolProperty3 = random.Next () % 2 == 0;
      instance.BoolProperty4 = random.Next () % 2 == 0;
      instance.BoolProperty5 = random.Next () % 2 == 0;
      instance.BoolProperty6 = random.Next () % 2 == 0;
      instance.BoolProperty7 = random.Next () % 2 == 0;
      instance.BoolProperty8 = random.Next () % 2 == 0;
      instance.BoolProperty9 = random.Next () % 2 == 0;
      instance.BoolProperty10 = random.Next () % 2 == 0;

      instance.IntProperty1 = random.Next();
      instance.IntProperty2 = random.Next();
      instance.IntProperty3 = random.Next();
      instance.IntProperty4 = random.Next();
      instance.IntProperty5 = random.Next();
      instance.IntProperty6 = random.Next();
      instance.IntProperty7 = random.Next();
      instance.IntProperty8 = random.Next();
      instance.IntProperty9 = random.Next();
      instance.IntProperty10 = random.Next();

      instance.DateTimeProperty1 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty2 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty3 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty4 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty5 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty6 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty7 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty8 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty9 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty10 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);

      instance.StringProperty1 = random.Next ().ToString ();
      instance.StringProperty2 = random.Next ().ToString ();
      instance.StringProperty3 = random.Next ().ToString ();
      instance.StringProperty4 = random.Next ().ToString ();
      instance.StringProperty5 = random.Next ().ToString ();
      instance.StringProperty6 = random.Next ().ToString ();
      instance.StringProperty7 = random.Next ().ToString ();
      instance.StringProperty8 = random.Next ().ToString ();
      instance.StringProperty9 = random.Next ().ToString ();
      instance.StringProperty10 = random.Next ().ToString ();
    }

    private void CreateAndFillRelationPropertyObject ()
    {
      ClassWithRelationProperties instance = ClassWithRelationProperties.NewObject().With();
      instance.Unary1 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary2 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary3 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary4 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary5 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary6 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary7 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary8 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary9 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();
      instance.Unary10 = OppositeClassWithAnonymousRelationProperties.NewObject ().With ();

      instance.Real1 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real2 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real3 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real4 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real5 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real6 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real7 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real8 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real9 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();
      instance.Real10 = OppositeClassWithVirtualRelationProperties.NewObject ().With ();

      instance.Virtual1 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual2 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual3 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual4 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual5 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual6 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual7 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual8 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual9 = OppositeClassWithRealRelationProperties.NewObject ().With ();
      instance.Virtual10 = OppositeClassWithRealRelationProperties.NewObject ().With ();

      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ().With ());
    }
  }
}
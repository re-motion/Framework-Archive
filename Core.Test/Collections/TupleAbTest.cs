using System;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class TupelAbTest
  {
    [Test]
    public void Initialize ()
    {
      Tuple<int, string> tupel = new Tuple<int, string> (1, "X");

      Assert.AreEqual (1, tupel.A);
      Assert.AreEqual ("X", tupel.B);
    }

    [Test]
    public void Equals_WithNull ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      Tuple<int, string> tupel = new Tuple<int, string> (1, "X");

      Assert.IsTrue (tupel.Equals (tupel));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");
      Tuple<int, string> right = new Tuple<int, string> (1, "X");

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");
      Tuple<int, string> right = new Tuple<int, string> (-1, "X");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");
      Tuple<int, string> right = new Tuple<int, string> (1, "A");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");
      Tuple<int, string> right = new Tuple<int, string> (1, "X");

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Tuple<int, string> left = new Tuple<int, string> (1, "X");
      Tuple<int, string> right = new Tuple<int, string> (1, "X");

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class SimpleDataStoreTest
  {
    private SimpleDataStore<string, int?> _store;
      
    [SetUp]
    public void SetUp ()
    {
      _store = new SimpleDataStore<string, int?>();
      _store.Add ("a", 1);
      _store.Add ("b", 2);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey_True ()
    {
      Assert.That (_store.ContainsKey ("a"));
      Assert.That (_store.ContainsKey ("b"));
    }

    [Test]
    public void ContainsKey_False ()
    {
      Assert.That (_store.ContainsKey ("c"), Is.False);
    }

    [Test]
    public void Add ()
    {
      _store.Add ("c", 5);
      Assert.That (_store.ContainsKey ("c"));
      Assert.That (_store["c"], Is.EqualTo (5));
    }

    [Test]
    public void Add_Null ()
    {
      _store.Add ("d", null);
      Assert.That (_store.ContainsKey ("d"));
      Assert.That (_store["d"], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The store already contains an element with key 'd'. (Old value: '1', new value: '2')\r\n" 
        + "Parameter name: key")]
    public void Add_Twice ()
    {
      _store.Add ("d", 1);
      _store.Add ("d", 2);
    }

    [Test]
    public void Remove_True ()
    {
      Assert.That (_store.Remove ("a"), Is.True);
      Assert.That (_store.ContainsKey ("a"), Is.False);
    }

    [Test]
    public void Remove_False ()
    {
      Assert.That (_store.Remove ("d"), Is.False);
    }

    [Test]
    public void Clear ()
    {
      _store.Clear();
      Assert.That (_store.ContainsKey ("a"), Is.False);
      Assert.That (_store.ContainsKey ("b"), Is.False);
    }

    [Test]
    public void Clear_Twice ()
    {
      _store.Clear ();
      _store.Clear ();
      Assert.That (_store.ContainsKey ("a"), Is.False);
      Assert.That (_store.ContainsKey ("b"), Is.False);
    }

    [Test]
    public void GetValue_Succeed ()
    {
      Assert.That (_store["a"], Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "There is no element with key 'c' in the store.")]
    public void GetValue_Fail ()
    {
      Dev.Null = _store["c"];
    }

    [Test]
    public void SetValue_New ()
    {
      _store["c"] = 5;
      Assert.That (_store["c"], Is.EqualTo (5));
    }

    [Test]
    public void SetValue_Overwrite ()
    {
      _store["a"] = 5;
      Assert.That (_store["a"], Is.EqualTo (5));
    }

    [Test]
    public void SetValue_Null ()
    {
      _store["c"] = null;
      Assert.That (_store["c"], Is.Null);
    }

    [Test]
    public void GetValueOrDefault_Value ()
    {
      Assert.That (_store.GetValueOrDefault ("a"), Is.EqualTo (1));
    }

    [Test]
    public void GetValueOrDefault_Default ()
    {
      Assert.That (_store.GetValueOrDefault ("c"), Is.Null);
    }

    [Test]
    public void TryGetValue_True ()
    {
      int? value;
      Assert.That (_store.TryGetValue ("a", out value));
      Assert.That (value, Is.EqualTo (1));
    }

    [Test]
    public void TryGetValue_False ()
    {
      int? value;
      Assert.That (_store.TryGetValue ("c", out value), Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_Get ()
    {
      bool delegateCalled = false;
      Assert.That (_store.GetOrCreateValue ("a", delegate (string key) { delegateCalled = true; return 7; }), Is.EqualTo (1));
      Assert.That (delegateCalled, Is.False);
    }

    [Test]
    public void GetOrCreateValue_Create ()
    {
      bool delegateCalled = false;
      Assert.That (_store.GetOrCreateValue ("c", delegate (string key)
          {
            delegateCalled = true;
            Assert.That (key, Is.EqualTo ("c"));
            return 7;
          }),
          Is.EqualTo (7));
      Assert.That (delegateCalled);
    }
  }
}
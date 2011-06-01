// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ExpiringDataStoreTest
  {
    private IExpirationPolicy<object, DateTime> _expirationPolicyMock;
    private ExpiringDataStore<string, object, DateTime> _dataStore;
    private object _fakeValue;
    private object _fakeValue2;
    private object _fakeValue3;
    private DateTime _fakeExpirationInfo;
    private DateTime _fakeExpirationInfo2;

    [SetUp]
    public void SetUp ()
    {
      _expirationPolicyMock = MockRepository.GenerateStrictMock<IExpirationPolicy<object, DateTime>>();
      _dataStore = new ExpiringDataStore<string, object, DateTime> (_expirationPolicyMock, new ReferenceEqualityComparer<string>());
      _fakeValue = new object ();
      _fakeValue2 = new object ();
      _fakeValue3 = new object ();
      _fakeExpirationInfo = new DateTime (0);
      _fakeExpirationInfo2 = new DateTime (0);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _dataStore).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey_False_NotContained ()
    {
      StubShouldScanForExpiredItems_False ();
      Assert.That (_dataStore.ContainsKey ("Test"), Is.False);
    }

    [Test]
    [Ignore ("TODO 3919")]
    public void ContainsKey_False_Expired ()
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      StubShouldScanForExpiredItems_False ();

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);

      Assert.That (_dataStore.ContainsKey ("Test1"), Is.False);

      CheckKeyNotContained ("Test1");
    }

    [Test]
    public void ContainsKey_True ()
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      StubShouldScanForExpiredItems_False ();

      Assert.That (_dataStore.ContainsKey ("Test1"), Is.True);
    }

    [Test]
    [Ignore ("TODO 3919")]
    public void ContainsKey_ScansForExpiredItems_True ()
    {
      CheckScansForExpiredItems_WithShouldScanTrue (store => store.ContainsKey ("Test"));
    }

    [Test]
    [Ignore ("TODO 3919")]
    public void ContainsKey_ScansForExpiredItems_False ()
    {
      CheckScansForExpiredItems_WithShouldScanFalse (store => store.ContainsKey ("Test"));
    }

    [Test]
    public void Add ()
    {
      StubShouldScanForExpiredItems_False();
      
      StubExpirationInfo (_fakeValue, _fakeExpirationInfo);
      _dataStore.Add ("Test", _fakeValue);

      _expirationPolicyMock.VerifyAllExpectations();
      CheckItemContained ("Test", _fakeValue, _fakeExpirationInfo);
    }

    [Test]
    public void Add_ScansForExpiredItems_True ()
    {
      CheckScansForExpiredItems_WithShouldScanTrue (store =>
      {
        StubExpirationInfo (_fakeValue, _fakeExpirationInfo);
        store.Add ("Test", _fakeValue);
      });
    }

    [Test]
    public void Add_ScansForExpiredItems_False ()
    {
      CheckScansForExpiredItems_WithShouldScanFalse (store =>
      {
        StubExpirationInfo (_fakeValue, _fakeExpirationInfo);
        store.Add ("Test", _fakeValue);
      });
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsFalse_KeyDoesNotExist ()
    {
      StubShouldScanForExpiredItems_False();

      var result = _dataStore.Remove ("Test");

      _expirationPolicyMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsFalse_KeyDoesExist ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);
      StubShouldScanForExpiredItems_False ();

      var result = _dataStore.Remove ("Test");

      _expirationPolicyMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ShouldScanForExpiredItems_True ()
    {
      CheckScansForExpiredItems_WithShouldScanTrue (store => store.Remove ("Test"));
    }

    [Test]
    public void Remove_ShouldScanForExpiredItems_False ()
    {
      CheckScansForExpiredItems_WithShouldScanFalse (store => store.Remove ("Test"));
    }

   [Test]
    public void Clear_NoItems ()
    {
      _expirationPolicyMock.Replay();

      _dataStore.Clear();

      _expirationPolicyMock.VerifyAllExpectations();
    }

    [Test]
    public void Clear_WithItems ()
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      PrepareItem ("Test2", _fakeValue2, _fakeExpirationInfo2);

      _dataStore.Clear();

      _expirationPolicyMock.VerifyAllExpectations();
      CheckKeyNotContained ("Test1");
      CheckKeyNotContained ("Test2");
    }

    [Test]
    public void GetValue ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);

      Assert.That (_dataStore["Test"], Is.SameAs (_fakeValue));
    }

    [Test]
    public void SetValue ()
    {
      CheckKeyNotContained ("Test");
      StubExpirationInfo (_fakeValue, _fakeExpirationInfo);

      CheckScansForExpiredItems_WithShouldScanTrue (store => store["Test"] = _fakeValue);

      CheckItemContained ("Test", _fakeValue, _fakeExpirationInfo);
    }

    [Test]
    public void GetValueOrDefault_KeyDoesNotExist ()
    {
      StubShouldScanForExpiredItems_False();

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_KeyDoesExist_NotExpired ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);
      StubShouldScanForExpiredItems_False ();

      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Replay ();

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.SameAs (_fakeValue));
    }

    [Test]
    public void GetValueOrDefault_KeyDoesExist_Expired ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);
      StubShouldScanForExpiredItems_False ();

      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Replay ();

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.Null);
      CheckKeyNotContained ("Test");
    }

    [Test]
    [Ignore ("TODO 3919")]
    public void GetValueOrDefault_ScansForExpiredItems_True ()
    {
      CheckScansForExpiredItems_WithShouldScanTrue (store => store.GetValueOrDefault ("Test"));
    }

    [Test]
    [Ignore ("TODO 3919")]
    public void GetValueOrDefault_ScansForExpiredItems_False ()
    {
      CheckScansForExpiredItems_WithShouldScanFalse (store => store.GetValueOrDefault ("Test"));
    }

    [Test]
    public void TryGetValue_KeyDoesNotExist ()
    {
      object value;
      var result = CheckScansForExpiredItems_WithShouldScanTrue (store => store.TryGetValue("Test", out value));
      
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanFalse_NotExpired ()
    {
      PrepareItem ("Test", _fakeValue3, _fakeExpirationInfo);

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue3, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Replay();

      object value = null;
      var result = CheckScansForExpiredItems_WithShouldScanTrue (store => store.TryGetValue ("Test", out value));

      Assert.That (result, Is.True);
      Assert.That (value, Is.SameAs (_fakeValue3));
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanFalse_Expired ()
    {
      PrepareItem ("Test", _fakeValue3, _fakeExpirationInfo);

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue3, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Replay();

      object value = null;
      var result = CheckScansForExpiredItems_WithShouldScanTrue (store => store.TryGetValue ("Test", out value));

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);
      CheckKeyNotContained ("Test");
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanTrue_NotExpired ()
    {
      PrepareItem ("Test", _fakeValue3, _fakeExpirationInfo);

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue3, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Replay ();

      object value = null;
      var result = CheckScansForExpiredItems_WithShouldScanTrue (store => store.TryGetValue ("Test", out value));

      Assert.That (result, Is.True);
      Assert.That (value, Is.SameAs (_fakeValue3));
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanTrue_Expired ()
    {
      PrepareItem ("Test", _fakeValue3, _fakeExpirationInfo);

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue3, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Replay ();

      object value = null;
      var result = CheckScansForExpiredItems_WithShouldScanTrue (store => store.TryGetValue ("Test", out value));

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_KeyDoesExist_NotExpired ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);

      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);

      object result = null;
      CheckScansForExpiredItems_WithShouldScanFalse (store => { result = store.GetOrCreateValue ("Test", k => new object ()); });
      
      Assert.That (result, Is.SameAs (_fakeValue));
    }

    [Test]
    public void GetOrCreateValue_KeyDoesExist_Expired ()
    {
      PrepareItem ("Test", _fakeValue, _fakeExpirationInfo);

      var newValue = new object ();
      StubExpirationInfo (newValue, _fakeExpirationInfo2);

      _expirationPolicyMock.Stub (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);

      object result = null;
      CheckScansForExpiredItems_WithShouldScanFalse (store => { result = store.GetOrCreateValue ("Test", k => newValue); });
      
      Assert.That (result, Is.SameAs (newValue));
      CheckItemContained ("Test", newValue, _fakeExpirationInfo2);
    }

    [Test]
    public void GetOrCreateValue_KeyDoesNotExist ()
    {
      var newValue = new object ();
      StubExpirationInfo (newValue, _fakeExpirationInfo);

      object result = null;
      CheckScansForExpiredItems_WithShouldScanFalse (store => { result = store.GetOrCreateValue ("Test", k => newValue); });

      Assert.That (result, Is.SameAs (newValue));
    }

    private void PrepareItem (string key, object value, DateTime expirationInfo)
    {
      var innerStore = GetInnerDataStore (_dataStore);
      innerStore.Add (key, Tuple.Create (value, expirationInfo));
    }

    private void StubExpirationInfo (object fakeValue, DateTime expirationInfo)
    {
      _expirationPolicyMock.Stub (stub => stub.GetExpirationInfo (fakeValue)).Return (expirationInfo);
    }

    private void CheckScansForExpiredItems_WithShouldScanFalse (Action<ExpiringDataStore<string, object, DateTime>> func)
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      PrepareItem ("Test2", _fakeValue2, _fakeExpirationInfo2);

      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Replay ();

      func (_dataStore);

      _expirationPolicyMock.VerifyAllExpectations ();
      CheckItemContained ("Test1", _fakeValue, _fakeExpirationInfo);
      CheckItemContained ("Test2", _fakeValue2, _fakeExpirationInfo2);
    }

    private bool CheckScansForExpiredItems_WithShouldScanFalse (Func<ExpiringDataStore<string, object, DateTime>, bool > func)
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      PrepareItem ("Test2", _fakeValue2, _fakeExpirationInfo2);

      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Replay ();

      var result = func (_dataStore);

      _expirationPolicyMock.VerifyAllExpectations ();
      CheckItemContained ("Test1", _fakeValue, _fakeExpirationInfo);
      CheckItemContained ("Test2", _fakeValue2, _fakeExpirationInfo2);
      return result;
    }

    private void CheckScansForExpiredItems_WithShouldScanTrue (Action<ExpiringDataStore<string, object, DateTime>> action)
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      PrepareItem ("Test2", _fakeValue2, _fakeExpirationInfo2);

      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      using (_expirationPolicyMock.GetMockRepository ().Ordered ())
      {
        _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
        _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue2, _fakeExpirationInfo2)).Return (true);
        _expirationPolicyMock.Expect (mock => mock.ItemsScanned());
      }
      _expirationPolicyMock.Replay ();

      action (_dataStore);

      _expirationPolicyMock.VerifyAllExpectations ();

      CheckItemContained ("Test1", _fakeValue, _fakeExpirationInfo);
      CheckKeyNotContained ("Test2");
    }

    private bool CheckScansForExpiredItems_WithShouldScanTrue (Func<ExpiringDataStore<string, object, DateTime>, bool> action)
    {
      PrepareItem ("Test1", _fakeValue, _fakeExpirationInfo);
      PrepareItem ("Test2", _fakeValue2, _fakeExpirationInfo2);

      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      using (_expirationPolicyMock.GetMockRepository ().Ordered ())
      {
        _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
        _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue2, _fakeExpirationInfo2)).Return (true);
        _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      }
      _expirationPolicyMock.Replay ();

      var result = action (_dataStore);

      _expirationPolicyMock.VerifyAllExpectations ();

      CheckItemContained ("Test1", _fakeValue, _fakeExpirationInfo);
      CheckKeyNotContained ("Test2");
      return result;
    }

    private void CheckItemContained (string key, object value, DateTime expectedExpirationInfo)
    {
      var innerStore = GetInnerDataStore (_dataStore);

      Assert.That (innerStore.ContainsKey (key), Is.True);
      Assert.That (innerStore[key].Item1, Is.SameAs (value));
      Assert.That (innerStore[key].Item2, Is.EqualTo (expectedExpirationInfo));
    }

    private void CheckKeyNotContained (string key)
    {
      var innerStore = GetInnerDataStore (_dataStore);

      Assert.That (innerStore.ContainsKey (key), Is.False);
    }

    private SimpleDataStore<string, Tuple<object, DateTime>> GetInnerDataStore (ExpiringDataStore<string, object, DateTime> expiringDataStore)
    {
      return (SimpleDataStore<string, Tuple<object, DateTime>>) PrivateInvoke.GetNonPublicField (expiringDataStore, "_innerDataStore");
    }

    private void StubShouldScanForExpiredItems_False ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
    }

  }
}
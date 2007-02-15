using System;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxePageTest: WxeTest
{

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
  }

  [Test]
  public void TestIsAbortEnabled()
  {
    WxePage page = new WxePage();
    
    page.EnableAbort = NaBooleanEnum.Undefined;
    Assert.IsTrue (((IWxePage)page).IsAbortEnabled, "Abort disabled with EnableAbort=Undefined.");
    
    page.EnableAbort = NaBooleanEnum.True;
    Assert.IsTrue (((IWxePage)page).IsAbortEnabled, "Abort disabled with EnableAbort=True.");
    
    page.EnableAbort = NaBooleanEnum.False;
    Assert.IsFalse (((IWxePage)page).IsAbortEnabled, "Abort enabled with EnableAbort=False.");
  }


  [Test]
  public void TestIsAbortConfimationEnabledWithAbortEnabledAndPageNotDirty()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.True;
    page.IsDirty = false;

    page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
    Assert.IsTrue (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation disabled with ShowAbortConfirmation=Always.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    Assert.IsTrue (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation disabled with ShowAbortConfirmation=OnlyIfDirty.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Never.");
  }

  [Test]
  public void TestIsAbortConfimationEnabledWithAbortEnabledTrueAndPageDirty()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.True;
    page.IsDirty = true;

    page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
    Assert.IsTrue (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation disabled with ShowAbortConfirmation=Always.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    Assert.IsTrue (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation disabledwith ShowAbortConfirmation=OnlyIfDirty.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Never.");
  }

  [Test]
  public void TestIsAbortConfimationEnabledWithAbortDisabledFalseAndPageNotDirty()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.False;
    page.IsDirty = false;

    page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Always.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=OnlyIfDirty.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Never.");
  }

  [Test]
  public void TestIsAbortConfimationEnabledWithAbortDisabledFalseAndPageDirty()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.False;
    page.IsDirty = true;

    page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Always.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=OnlyIfDirty.");
    
    page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
    Assert.IsFalse (((IWxePage)page).IsAbortConfirmationEnabled, "Abort confirmation enabled with ShowAbortConfirmation=Never.");
  }


  [Test]
  public void TestAreOutOfSequencePostBacksEnabledWithAbortEnabled()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.True;

    page.EnableOutOfSequencePostBacks = NaBooleanEnum.Undefined;
    Assert.IsFalse (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=Undefined.");
    
    page.EnableOutOfSequencePostBacks = NaBooleanEnum.True;
    Assert.IsFalse (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=True.");
    
    page.EnableOutOfSequencePostBacks = NaBooleanEnum.False;
    Assert.IsFalse (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=False.");
  }

  [Test]
  public void TestAreOutOfSequencePostBacksEnabledWithAbortDisabled()
  {
    WxePage page = new WxePage();
    page.EnableAbort = NaBooleanEnum.False;

    page.EnableOutOfSequencePostBacks = NaBooleanEnum.Undefined;
    Assert.IsFalse (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=Undefined.");
    
    page.EnableOutOfSequencePostBacks = NaBooleanEnum.True;
    Assert.IsTrue (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks disabled with EnableOutOfSequencePostBacks=True.");
    
    page.EnableOutOfSequencePostBacks = NaBooleanEnum.False;
    Assert.IsFalse (((IWxePage)page).AreOutOfSequencePostBacksEnabled, "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=False.");
  }
}
}

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.ObjectBinding.UnitTests
{
  [TestFixture]
  public class ListInfoTest
  {
    [Test]
    public void GetItemType ()
    {
      IListInfo listInfo = new ListInfo (typeof (string));
      Assert.That (listInfo.ItemType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetRequiresWriteBack ()
    {
      IListInfo listInfo = new ListInfo (typeof (string));
      Assert.That (listInfo.RequiresWriteBack, Is.True);
    }

    [Test]
    public void CreateList ()
    {
      IListInfo listInfo = new ListInfo (typeof (string));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new string[1]));
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void InsertItem ()
    {      
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void RemvoeItem ()
    {
    }
  }
}
using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{

  [TestFixture]
  public class LazyContainerTest : WebControlTest
  {
    // types

    // static members and constants

    // member fields

    private StringCollectionChecker _stringCollectionChecker;
    private StringCollection _actualEvents;
    
    private LazyContainer _lazyContainer;
    private ControlInvoker _lazyContainerInvoker;

    // construction and disposing

    public LazyContainerTest ()
    {
    }

    // methods and properties

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _stringCollectionChecker = new StringCollectionChecker ();
      _actualEvents = new StringCollection ();

      _lazyContainer = new LazyContainer ();
      _lazyContainer.ID = "LazyContainer";
      _lazyContainer.Init += new EventHandler(LazyContainer_Init);
      _lazyContainer.Load += new EventHandler(LazyContainer_Load);
      NamingContainer.Controls.Add (_lazyContainer);
      
      _lazyContainerInvoker = new ControlInvoker (_lazyContainer);    
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsTrue (_lazyContainer.Controls is EmptyControlCollection);

      Assert.IsNotNull (_lazyContainer.RealControls);
      Assert.IsFalse (_lazyContainer.RealControls is EmptyControlCollection);
    }

    [Test]
    public void Ensure ()
    {
      Assert.IsTrue (_lazyContainer.Controls is EmptyControlCollection);

      _lazyContainer.Ensure ();

      Assert.IsNotNull (_lazyContainer.Controls);
      Assert.IsFalse (_lazyContainer.Controls is EmptyControlCollection);
    }

    [Test]
    public void ControlInit ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    private void LazyContainer_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_lazyContainer));
    }

    private void LazyContainer_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_lazyContainer));
    }

    private string FormatInitEvent (Control sender)
    {
      return FormatEvent (sender, "Init");
    }

    private string FormatLoadEvent (Control sender)
    {
      return FormatEvent (sender, "Load");
    }

    private string FormatEvent (Control sender, string eventName)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);

      return sender.ID + " " + eventName;
    }
  }

}

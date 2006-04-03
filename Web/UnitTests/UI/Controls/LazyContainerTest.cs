using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;

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

    private ControlMock _parent;
    private ControlMock _child;

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
      _lazyContainer.Init += new EventHandler (LazyContainer_Init);
      _lazyContainer.Load += new EventHandler (LazyContainer_Load);
      NamingContainer.Controls.Add (_lazyContainer);
      
      _lazyContainerInvoker = new ControlInvoker (_lazyContainer);    

      _parent = new ControlMock ();
      _parent.ID = "Parent";
      _parent.Init += new EventHandler (Parent_Init);
      _parent.Load += new EventHandler (Parent_Load);

      _child = new ControlMock ();
      _child.ID = "Child";
      _child.Init += new EventHandler (Child_Init);
      _child.Load += new EventHandler (Child_Load);

      _parent.Controls.Add (_child);
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
    public void Control_Add_Init_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));

      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.InitRecursive ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));

      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));

      _lazyContainer.RealControls.Add (_parent);

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    
    [Test]
    public void Control_Add_Init_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));

      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Add_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.LoadRecursive ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));

      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Add_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));

      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));

      _lazyContainer.Ensure ();

      _stringCollectionChecker.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Add_SaveViewStateRecursive ()
    {
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState ();
      
      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Pair);
      Pair values = (Pair) viewState;
      Assert.IsNull (values.Second);
    }
    
    [Test]
    public void Control_Init_LoadViewState ()
    {
      NamingContainerInvoker.InitRecursive ();

      _lazyContainerInvoker.LoadViewState (new Pair (null, new object()));
    }
    
    [Test]
    public void Control_Init_LoadViewStateWithNull ()
    {
      NamingContainerInvoker.InitRecursive ();

      _lazyContainerInvoker.LoadViewState (null);
    }

    [Test]
    public void Control_Init_Load_Add_Ensure_SaveViewStateRecursive ()
    {
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      _lazyContainer.Ensure ();

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState ();
      
      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Pair);
      Pair values = (Pair) viewState;
      Assert.IsNotNull (values.Second);
    }

    private void LazyContainer_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_lazyContainer));
    }

    private void LazyContainer_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_lazyContainer));
    }

    private void Parent_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_parent));
    }

    private void Parent_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_parent));
    }

    private void Child_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_child));
    }

    private void Child_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_child));
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

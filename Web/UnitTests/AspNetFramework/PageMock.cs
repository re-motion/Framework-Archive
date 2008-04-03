using System;
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.AspNetFramework
{

  public class PageMock : Page
  {
    // types

    // static members and constants

    // member fields

    private PageStatePersister _pageStatePersister;

    // construction and disposing

    public PageMock ()
    {
    }

    // methods and properties

    public NameValueCollection RequestValueCollection
    {
      get { return (NameValueCollection) PrivateInvoke.GetNonPublicField (this,"_requestValueCollection"); }
    }

    public void SetRequestValueCollection (NameValueCollection requestValueCollection)
    {
      ArgumentUtility.CheckNotNull ("requestValueCollection", requestValueCollection);

      PrivateInvoke.SetNonPublicField (this, "_requestValueCollection", requestValueCollection);
    }

    protected override PageStatePersister PageStatePersister
    {
      get { return GetPageStatePersister (); }
    }

    public PageStatePersister GetPageStatePersister ()
    {
      EnsurePageStatePersister ();
      return _pageStatePersister;
    }

    public void SetPageStatePersister (PageStatePersister pageStatePersister)
    {
      ArgumentUtility.CheckNotNull ("pageStatePersister", pageStatePersister);

      _pageStatePersister = pageStatePersister;
    }

    private void EnsurePageStatePersister ()
    {
      if (_pageStatePersister == null)
        _pageStatePersister = new HiddenFieldPageStatePersister (this);
    }

    public void LoadAllState ()
    {
      PrivateInvoke.InvokeNonPublicMethod (this, typeof (Page), "LoadAllState", new object[0]);
    }

    public void SaveAllState ()
    {
      PrivateInvoke.InvokeNonPublicMethod (this, typeof (Page), "SaveAllState", new object[0]);
    }
  }

}

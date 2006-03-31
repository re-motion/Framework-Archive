using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

  [ToolboxItem (false)]
  [DefaultProperty ("RealControls")]
  internal class LazyContainer : PlaceHolder
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public LazyContainer ()
    {
    }

    // methods and properties

    public void Ensure ()
    {
//      if (_isEnsured)
//        return;
//
//      _isEnsured = true;
    }

    public override ControlCollection Controls
    {
      get
      {
//        if (ControlHelper.IsDesignMode (this))
          return base.Controls;
//        else if (ParentMultiView == null || ! ParentMultiView.EnableLazyLoading)
//          return base.Controls;
//        else if (! _isEnsured)
//          return new EmptyControlCollection (this);
//        else if (Active)
//          return base.Controls;
//        else
//          return new EmptyControlCollection (this);
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public ControlCollection RealControls
    {
      get
      {
        EnsureContentPlaceHolderCreated ();
//        if (_contentPlaceHolder != null)
//          return _contentPlaceHolder.Controls; 
//        else
          return base.Controls;
      }
    }

    private void EnsureContentPlaceHolderCreated ()
    {
//      if (_isContentPlaceHolderCreated)
//        return;
//
//      _isContentPlaceHolderCreated = true;
//      _contentPlaceHolder = CreateContentPlaceHolder ();
    }

    protected virtual PlaceHolder CreateContentPlaceHolder ()
    {
      return new PlaceHolder ();
    }
  }

}

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
  public class LazyContainer : PlaceHolder
  {
    // types

    // static members and constants

    // member fields

    private bool _isEnsured;
    private EmptyControlCollection _emptyControlCollection;

    // construction and disposing

    public LazyContainer ()
    {
    }

    // methods and properties

    public void Ensure ()
    {
      if (_isEnsured)
        return;

      _isEnsured = true;
    }

    public override ControlCollection Controls
    {
      get
      {
        if (_isEnsured)
        {
          return base.Controls;
        }
        else
        {
          _emptyControlCollection = new EmptyControlCollection (this);
          return _emptyControlCollection;          
        }
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public ControlCollection RealControls
    {
      get
      {
        return base.Controls;
//        EnsureContentPlaceHolderCreated ();
//        if (_contentPlaceHolder != null)
//          return _contentPlaceHolder.Controls; 
//        else
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

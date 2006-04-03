using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

  [ToolboxItem (false)]
  [PersistChildren (true)]
  [ParseChildren (true, "RealControls")]
  public class LazyContainer : Control
  {
    // types

    // static members and constants

    // member fields

    private bool _isEnsured;
    private EmptyControlCollection _emptyControlCollection;
    private PlaceHolder _placeHolder;
    private object _recursiveViewState;
    private bool _isSavingViewStateRecursive;
    private bool _isLoadingViewStateRecursive;
    private bool _isLazyLoadingEnabled;

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
      EnsurePlaceHolderCreated ();
      Controls.Add (_placeHolder);
    }

    public bool IsLazyLoadingEnabled
    {
      get { return _isLazyLoadingEnabled; }
      set { _isLazyLoadingEnabled = value; }
    }

    public override ControlCollection Controls
    {
      get
      {
        if (! _isLazyLoadingEnabled)
          Ensure ();

        if (_isEnsured)
        {
          return base.Controls;
        }
        else
        {
          if (_emptyControlCollection == null)
            _emptyControlCollection = new EmptyControlCollection (this);
          return _emptyControlCollection;          
        }
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    public ControlCollection RealControls
    {
      get
      {
        EnsurePlaceHolderCreated ();
        return _placeHolder.Controls; 
      }
    }

    private void EnsurePlaceHolderCreated ()
    {
      if (_placeHolder == null)
        _placeHolder = new PlaceHolder ();
    }

    protected override void LoadViewState (object savedState)
    {
      if (_isLoadingViewStateRecursive)
        return;

      if (savedState != null)
      {
        Pair values = (Pair) savedState;
        base.LoadViewState (values.First);
        _recursiveViewState = values.Second;

        if (_isLazyLoadingEnabled)
        {
          _isLoadingViewStateRecursive = true;
          ControlHelper.LoadViewStateRecursive (this, _recursiveViewState);
          _isLoadingViewStateRecursive = false;
        }
      }
    }

    protected override object SaveViewState ()
    {
      if (_isSavingViewStateRecursive)
        return null;

      if (_isLazyLoadingEnabled && _isEnsured)
      {
        _isSavingViewStateRecursive = true;
        _recursiveViewState = ControlHelper.SaveViewStateRecursive (this);
        _isSavingViewStateRecursive = false;
      }

      Pair values = new Pair ();
      values.First = base.SaveViewState ();
      values.Second = _recursiveViewState;

      return values;
    }
  }

}

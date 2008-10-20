/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  //TODO: Refactor to use SingleChildControlCollection
  public sealed class ControlReplacer : Control, INamingContainer, IPostBackDataHandler
  {
    private readonly IInternalControlMemberCaller _memberCaller;
    private Control _controlToWrap;
    private bool _hasLoaded;
    private IStateModificationStrategy _stateModificationStrategy;

    public ControlReplacer (IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _memberCaller = memberCaller;
    }

    public IStateModificationStrategy StateModificationStrategy
    {
      get { return _stateModificationStrategy; }
      set { _stateModificationStrategy = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public Control WrappedControl
    {
      get { return Controls.Count == 1 ? Controls[0] : null; }
    }

    public Control ControlToWrap
    {
      get { return _controlToWrap; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Page.RegisterRequiresControlState (this);
      Page.RegisterRequiresPostBack (this);
    }

    protected override void LoadControlState (object savedState)
    {
    }

    protected override object SaveControlState ()
    {
      return "value";
    }

    protected override void LoadViewState (object savedState)
    {
    }

    protected override object SaveViewState ()
    {
      return "value";
    }

    public bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      EnsureWrappedControl();
      return false;
    }

    public void RaisePostDataChangedEvent ()
    {
      throw new NotSupportedException ();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      EnsureWrappedControl ();
    }

    private void EnsureWrappedControl ()
    {
      if (!_hasLoaded)
      {
        _hasLoaded = true;
        Assertion.IsNotNull (_controlToWrap);

        _stateModificationStrategy.LoadControlState (this, _memberCaller);
        _stateModificationStrategy.LoadViewState (this, _memberCaller);

        _memberCaller.SetControlState (_controlToWrap, ControlState.Constructed);
        Control controlToWrap = _controlToWrap;
        _controlToWrap = null;
        Controls.Add (controlToWrap);
      }
    }

    public string SaveAllState ()
    {
      Pair state = new Pair (_memberCaller.SaveChildControlState (this), _memberCaller.SaveViewStateRecursive (this));
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      return writer.ToString();
    }

    public void ReplaceAndWrap<T> (T controlToReplace, T controlToWrap, IStateModificationStrategy stateModificationStrategy)
        where T : Control, IReplaceableControl
    {
      ArgumentUtility.CheckNotNull ("controlToReplace", controlToReplace);
      ArgumentUtility.CheckNotNull ("controlToWrap", controlToWrap);
      ArgumentUtility.CheckNotNull ("stateModificationStrategy", stateModificationStrategy);

      if (_memberCaller.GetControlState (controlToReplace) != ControlState.ChildrenInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped during OnInit phase.");

      if (controlToReplace.IsInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped before they are initialized.");

      controlToWrap.Replacer = this;

      _stateModificationStrategy = stateModificationStrategy;

      Control parent = controlToReplace.Parent;
      int index = parent.Controls.IndexOf (controlToReplace);

      //Mark parent collection as modifiable
      string errorMessage = _memberCaller.SetCollectionReadOnly (parent.Controls, null);

      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, this);

      //Mark parent collection as readonly
      _memberCaller.SetCollectionReadOnly (parent.Controls, errorMessage);

      _memberCaller.InitRecursive (this, parent);

      if (!parent.Page.IsPostBack)
      {
        _hasLoaded = true;
        Controls.Add (controlToWrap);
      }
      else
      {
        _controlToWrap = controlToWrap;
      }
    }
  }
}
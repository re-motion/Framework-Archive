using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding.Design
{
  internal class ConcreteEditorControlBase : EditorControlBase
  {
    public ConcreteEditorControlBase ()
    {
    }

    public override object Value
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException (); }
    }
  }
}
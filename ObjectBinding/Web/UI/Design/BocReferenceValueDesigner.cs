using System;
using System.ComponentModel;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{

public class BocReferenceValueDesigner: Design.BocDesigner
{
  public override void OnComponentChanged(object sender, System.ComponentModel.Design.ComponentChangedEventArgs ce)
  {
    base.OnComponentChanged (sender, ce);
    if (ce.Member.Name == "Command")
    {
      PropertyDescriptor persistedCommand = TypeDescriptor.GetProperties (Component)["PersistedCommand"];
      RaiseComponentChanged (persistedCommand, null, ((BocReferenceValue) Component).PersistedCommand);
    }
  }
}

}

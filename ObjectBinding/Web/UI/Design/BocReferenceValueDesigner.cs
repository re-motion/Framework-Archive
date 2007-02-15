using System;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
{

public class BocReferenceValueDesigner: WebControlDesigner
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

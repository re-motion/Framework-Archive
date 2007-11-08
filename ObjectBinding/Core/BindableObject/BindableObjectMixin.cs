using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public class BindableObjectMixin : BindableObjectMixinBase<object>
  {
    public BindableObjectMixin ()
    {
    }

    protected override BindableObjectClass InitializeBindableObjectClass()
    {
      return BindableObjectProvider.Current.GetBindableObjectClass (Configuration.TargetClass.Type);
    }
  }
}
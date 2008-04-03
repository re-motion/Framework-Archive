using System;

namespace Remotion.ObjectBinding.BindableObject
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
using System;
using System.Collections.Generic;

namespace Mixins.Configuration
{
  public class BaseClassConfiguration : ClassConfiguration
  {
    private List<MixinConfiguration> _mixins = new List<MixinConfiguration> ();
    private List<Type> _faceInterfaces = new List<Type> ();

    public BaseClassConfiguration (Type type)
        : base (type)
    {
    }

    public IEnumerable<MixinConfiguration> Mixins
    {
      get { return _mixins; }
    }

    public void AddMixin (MixinConfiguration newMixin)
    {
      _mixins.Add (newMixin);
    }

    public IEnumerable<Type> FaceInterfaces
    {
      get { return _faceInterfaces; }
    }

    public void AddFaceInterface (Type newFaceInterface)
    {
      _faceInterfaces.Add (newFaceInterface);
    }
  }
}

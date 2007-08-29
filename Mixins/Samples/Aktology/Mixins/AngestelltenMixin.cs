using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Samples.Aktology.Akten;

namespace Rubicon.Mixins.Samples.Aktology.Mixins
{
  public interface IAngestelltenMixin : IPersonenMixin
  {
    string Position { get; set; }
  }

  public class AngestelltenMixin : PersonenMixin, IAngestelltenMixin
  {
    private string _position;

    public string Position
    {
      get { return _position; }
      set { _position = value; }
    }
  }
}

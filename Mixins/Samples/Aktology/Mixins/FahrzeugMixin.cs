using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Samples.Aktology.Akten;

namespace Rubicon.Mixins.Samples.Aktology.Mixins
{
  public interface IFahrzeugMixin
  {
    string Type { get; set; }
    string Kennzeichen { get; set; }
  }

  public class FahrzeugMixin : IFahrzeugMixin // Wir k�nnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber f�r dieses Mixin nicht n�tig
  {
    private string _type;
    private string _kennzeichen;

    public string Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public string Kennzeichen
    {
      get { return _kennzeichen; }
      set { _kennzeichen = value; }
    }
  }
}

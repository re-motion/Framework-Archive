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

  public class FahrzeugMixin : IFahrzeugMixin // Wir könnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber für dieses Mixin nicht nötig
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

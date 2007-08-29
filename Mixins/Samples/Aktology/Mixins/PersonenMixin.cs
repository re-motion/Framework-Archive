using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Samples.Aktology.Akten;

namespace Rubicon.Mixins.Samples.Aktology.Mixins
{
  public interface IPersonenMixin
  {
    string Vorname { get; set; }
    string Nachname { get; set; }
  }

  public class PersonenMixin : IPersonenMixin // Wir könnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber für dieses Mixin nicht nötig
  {
    private string _vorname;
    private string _nachname;

    public string Vorname
    {
      get { return _vorname; }
      set { _vorname = value; }
    }

    public string Nachname
    {
      get { return _nachname; }
      set { _nachname = value; }
    }
  }
}

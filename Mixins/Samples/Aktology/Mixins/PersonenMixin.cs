using System;
using Mixins;
using Samples.Aktology.Akten;

namespace Samples.Aktology.Mixins
{
  public interface IPersonenMixin
  {
    string Vorname { get; set; }
    string Nachname { get; set; }
  }

  public class PersonenMixin : IPersonenMixin // Wir k�nnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber f�r dieses Mixin nicht n�tig
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

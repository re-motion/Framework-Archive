using System;
using Rubicon.Mixins;
using Samples.Aktology.Akten;

namespace Samples.Aktology.Mixins
{
  public interface ITransportFahrzeugMixin : IFahrzeugMixin
  {
    string MaximaleLadung { get; set; }
  }

  public class TransportFahrzeugMixin : FahrzeugMixin, ITransportFahrzeugMixin
  {
    private string _maximaleLadung;

    public string MaximaleLadung
    {
      get { return _maximaleLadung; }
      set { _maximaleLadung = value; }
    }
  }
}

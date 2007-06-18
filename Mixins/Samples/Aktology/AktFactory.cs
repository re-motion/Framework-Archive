using System;
using Samples.Aktology.Akten;
using Samples.Aktology.Mixins;
using Mixins;
using Mixins.Context;

namespace Samples.Aktology
{
  public interface IFahrzeugAkt : IAkt, IFahrzeugMixin {}
  public interface IFahrzeugSachAkt : ISachAkt, IFahrzeugMixin {}

  public interface ITransportFahrzeugAkt : IAkt, ITransportFahrzeugMixin {}
  public interface ITransportFahrzeugSachAkt : ISachAkt, ITransportFahrzeugMixin {}

  public interface IPersonenAkt : IAkt, IPersonenMixin {}
  public interface IPersonenSachAkt : ISachAkt, IPersonenMixin {}

  public interface IAngestelltenAkt : IAkt, IAngestelltenMixin {}
  public interface IAngestelltenSachAkt : ISachAkt, IAngestelltenMixin {}

  public static class AktFactory
  {
    public static IFahrzeugAkt CreateFahrzeugAkt ()
    {
      return Create<IFahrzeugAkt, Akt, FahrzeugMixin> ();
    }

    public static IFahrzeugSachAkt CreateFahrzeugSachAkt ()
    {
      return Create<IFahrzeugSachAkt, SachAkt, FahrzeugMixin> ();
    }

    public static ITransportFahrzeugAkt CreateTransportFahrzeugAkt ()
    {
      return Create<ITransportFahrzeugAkt, Akt, TransportFahrzeugMixin> ();
    }

    public static ITransportFahrzeugSachAkt CreateTransportFahrzeugSachAkt ()
    {
      return Create<ITransportFahrzeugSachAkt, SachAkt, TransportFahrzeugMixin> ();
    }

    public static IPersonenAkt CreatePersonenAkt ()
    {
      return Create<IPersonenAkt, Akt, PersonenMixin> ();
    }

    public static IPersonenSachAkt CreatePersonenSachAkt ()
    {
      return Create<IPersonenSachAkt, SachAkt, PersonenMixin> ();
    }

    public static IAngestelltenAkt CreateAngestelltenAkt ()
    {
      return Create<IAngestelltenAkt, Akt, AngestelltenMixin> ();
    }

    public static IAngestelltenSachAkt CreateAngestelltenSachAkt ()
    {
      return Create<IAngestelltenSachAkt, SachAkt, AngestelltenMixin> ();
    }

    private static TInterface Create<TInterface, TBaseType, TMixin> ()
    {
      using (MixinConfiguration.CreateEmptyConfiguration())
      {
        ClassContext specificContext = new ClassContext(typeof (TBaseType), typeof (TMixin));
        specificContext.AddCompleteInterface (typeof (TInterface));

        MixinConfiguration.ActiveContext.AddClassContext (specificContext);
        MixinConfiguration.ActiveContext.RegisterInterface (typeof (TInterface), specificContext);

        return ObjectFactory.Create<TInterface>().With();
      }
    }
  }
}

using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  public class InvertingClientTransactionMixin : Mixin<ClientTransaction, InvertingClientTransactionMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      void Commit ();
      void Rollback ();
    }

    [Override]
    public void Commit ()
    {
      Base.Rollback (); // okay, this is not really realistic
    }

    [Override]
    public void Rollback ()
    {
      Base.Commit ();
    }
  }
}
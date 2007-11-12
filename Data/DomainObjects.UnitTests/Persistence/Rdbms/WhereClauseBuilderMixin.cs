using System;
using System.Text;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class WhereClauseBuilderMixin : Mixin<WhereClauseBuilderMixin.IRequirements>
  {
    public interface IRequirements
    {
      StringBuilder Builder { get; }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();
      This.Builder.Append ("Mixed!");
    }
  }
}
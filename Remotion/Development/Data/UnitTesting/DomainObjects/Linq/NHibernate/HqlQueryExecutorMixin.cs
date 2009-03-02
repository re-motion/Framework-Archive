using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq.NHibernate
{
  /// <summary>
  /// This mixin writes the generated Linq statements to the console. 
  /// Use the <see cref="ApplyQueryExecutorMixinAttribute"/> to your assembly to actually apply the mixin.
  /// </summary>
  public class HqlQueryExecutorMixin : Mixin<object, HqlQueryExecutorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      IQuery CreateQuery (string id, ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters);
    }

    [OverrideTarget]
    public IQuery CreateQuery (string id, ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      IQuery query = Base.CreateQuery (id, classDefinition, statement, commandParameters);
      QueryConstructed (query);
      return query;
    }

    private void QueryConstructed (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      Console.WriteLine (query.Statement);
      foreach (QueryParameter parameter in query.Parameters)
        Console.WriteLine ("{0} = {1}", parameter.Name, parameter.Value);
    }
  }
}
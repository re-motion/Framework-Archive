using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Rubicon.Data.DomainObjects.Linq.Clauses;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Linq.Parsing
{
  public class QueryParser
  {
    private readonly List<FromLetWhereExpressionBase> _fromLetWhereExpressions = new List<FromLetWhereExpressionBase> ();
    private readonly List<LambdaExpression> _projectionExpressions = new List<LambdaExpression> ();

    public QueryParser (Expression expressionTreeRoot)
    {
      ArgumentUtility.CheckNotNull ("expressionTreeRoot", expressionTreeRoot);
      MethodCallExpression rootExpression = ParserUtility.GetTypedExpression<MethodCallExpression> (expressionTreeRoot,
          "expression tree root", expressionTreeRoot);

      SourceExpression = expressionTreeRoot;

      switch (ParserUtility.CheckMethodCallExpression (rootExpression, expressionTreeRoot, "Select", "Where","SelectMany"))
      {
        case "Select":
          SelectExpressionParser selectExpressionParser = new SelectExpressionParser(rootExpression, expressionTreeRoot);
          _fromLetWhereExpressions.AddRange (selectExpressionParser.FromLetWhereExpressions);
          _projectionExpressions.AddRange (selectExpressionParser.ProjectionExpressions);
          break;
        case "Where":
          WhereExpressionParser whereExpressionParser = new WhereExpressionParser (rootExpression, expressionTreeRoot, true);
          _fromLetWhereExpressions.AddRange (whereExpressionParser.FromLetWhereExpressions);
          _projectionExpressions.AddRange (whereExpressionParser.ProjectionExpressions);
          break;
        case "SelectMany":
          SelectManyExpressionParser selectManyExpressionParser = new SelectManyExpressionParser (rootExpression, expressionTreeRoot);
          _fromLetWhereExpressions.AddRange (selectManyExpressionParser.FromLetWhereExpressions);
          _projectionExpressions.AddRange (selectManyExpressionParser.ProjectionExpressions);
          break;
      }
    }

    public Expression SourceExpression { get; private set; }

    public QueryExpression GetParsedQuery ()
    {
      FromExpression mainFromExpression = (FromExpression) _fromLetWhereExpressions[0];
      MainFromClause mainFromClause = new MainFromClause (mainFromExpression.Identifier,
          (IQueryable) ((ConstantExpression) mainFromExpression.Expression).Value);

      List<IFromLetWhereClause> fromLetWhereClauses = new List<IFromLetWhereClause>();
      IClause previousClause = mainFromClause;
      
      int currentProjection = 0;
      for (int currentFromLetWhere = 1; currentFromLetWhere < _fromLetWhereExpressions.Count; currentFromLetWhere++)
      {
        FromExpression fromExpression = _fromLetWhereExpressions[currentFromLetWhere] as FromExpression;
        if (fromExpression != null)
        {
          AdditionalFromClause additionalFromClause = new AdditionalFromClause (previousClause, fromExpression.Identifier,
              (LambdaExpression) fromExpression.Expression, _projectionExpressions[currentProjection]);
          fromLetWhereClauses.Add (additionalFromClause);
          previousClause = additionalFromClause;
          ++currentProjection;
        }

        WhereExpression whereExpression = _fromLetWhereExpressions[currentFromLetWhere] as WhereExpression;
        if (whereExpression != null)
        {
          WhereClause whereClause = new WhereClause (previousClause, whereExpression.Expression);
          fromLetWhereClauses.Add (whereClause);
          previousClause = whereClause;
        }
      }

      SelectClause selectClause = new SelectClause (previousClause, _projectionExpressions.Last ());
      QueryBody queryBody = new QueryBody (selectClause);

      foreach (IFromLetWhereClause fromLetWhereClause in fromLetWhereClauses)
        queryBody.Add (fromLetWhereClause);

      return new QueryExpression (mainFromClause, queryBody, SourceExpression);
    }
  }
}
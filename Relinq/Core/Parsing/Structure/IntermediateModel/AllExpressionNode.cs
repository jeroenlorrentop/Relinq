// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for the 
  /// <see cref="Queryable.All{TSource}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,bool}})"/> and
  /// <see cref="Enumerable.All{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/> methods.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class AllExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.All<object> (null, null)),
                                                               GetSupportedMethod (() => Enumerable.All<object> (null, null))
                                                           };

    private readonly ResolvedExpressionCache<Expression> _cachedPredicate;

    public AllExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate)
        : base (parseInfo, null, null)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      Predicate = predicate;
      _cachedPredicate = new ResolvedExpressionCache<Expression> (this);
    }

    public LambdaExpression Predicate { get; private set; }

    public Expression GetResolvedPredicate (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedPredicate.GetOrCreate (r => r.GetResolvedExpression (Predicate.Body, Predicate.Parameters[0], clauseGenerationContext));
    }

    public override Expression Resolve (
        ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      // no data streams out from this node, so we cannot resolve All expressions
      throw CreateResolveNotSupportedException();
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new AllResultOperator (GetResolvedPredicate (clauseGenerationContext));
    }
  }
}

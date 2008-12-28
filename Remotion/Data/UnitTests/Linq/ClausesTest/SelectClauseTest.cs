// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;
using Rhino.Mocks;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;

namespace Remotion.Data.UnitTests.Linq.ClausesTest
{
  [TestFixture]
  public class SelectClauseTest
  {
    [Test]
    public void InitializeWithExpression ()
    {
      LambdaExpression expression = ExpressionHelper.CreateLambdaExpression ();
      IClause clause = ExpressionHelper.CreateClause();

      SelectClause selectClause = new SelectClause (clause, expression, null);
      Assert.AreSame (clause, selectClause.PreviousClause);
      Assert.AreEqual (expression, selectClause.ProjectionExpression);
    }

    [Test]
    public void InitializeWithExpression_New ()
    {
      LambdaExpression expression = ExpressionHelper.CreateLambdaExpression ();
      IClause clause = ExpressionHelper.CreateClause ();
      var query = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQuerySource());
      var methodInfo = ParserUtility.GetMethod (() => Enumerable.Count (query));
      MethodCallExpression methodCallExpression = Expression.Call (methodInfo, query.Expression);
      List<MethodCallExpression> methodCallExpressions = new List<MethodCallExpression>();
      methodCallExpressions.Add (methodCallExpression);
      
      SelectClause selectClause = new SelectClause (clause, expression, methodCallExpressions);
      Assert.AreSame (clause, selectClause.PreviousClause);
    }

    [Test]
    public void SelectWithMethodCall_ResultModifierData ()
    {
      LambdaExpression expression = ExpressionHelper.CreateLambdaExpression ();
      IClause clause = ExpressionHelper.CreateClause ();
      var query = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQuerySource ());
      var methodInfo = ParserUtility.GetMethod (() => Enumerable.Count (query));
      MethodCallExpression methodCallExpression = Expression.Call (methodInfo, query.Expression);

      SelectClause selectClause = new SelectClause (clause, expression);
      ResultModifierClause resultModifierClause = new ResultModifierClause (selectClause, methodCallExpression);
      selectClause.AddResultModifierData (resultModifierClause);

      Assert.IsNotEmpty (selectClause.ResultModifierData);
      Assert.That (selectClause.ResultModifierData, Is.EqualTo (new[] { resultModifierClause }));
    }

    [Test]
    public void InitializeWithoutExpression ()
    {
      SelectClause selectClause = new SelectClause (ExpressionHelper.CreateClause (), null, null);
      Assert.IsNull (selectClause.ProjectionExpression);
    }
    
    [Test]
    public void SelectClause_ImplementISelectGroupClause()
    {
      SelectClause selectClause = ExpressionHelper.CreateSelectClause();

      Assert.IsInstanceOfType (typeof(ISelectGroupClause),selectClause);
    }
        
    [Test]
    public void SelectClause_ImplementIQueryElement()
    {
      SelectClause selectClause = ExpressionHelper.CreateSelectClause();
      Assert.IsInstanceOfType (typeof (IQueryElement), selectClause);
    }

    [Test]
    public void Accept()
    {
      SelectClause selectClause = ExpressionHelper.CreateSelectClause();

      MockRepository repository = new MockRepository();
      IQueryVisitor visitorMock = repository.StrictMock<IQueryVisitor>();

      visitorMock.VisitSelectClause (selectClause);

      repository.ReplayAll();

      selectClause.Accept (visitorMock);

      repository.VerifyAll();
    }
  }
}
/*******************************************************************************
* MIT License
*
* Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
* 
* Permission is hereby granted, free of charge, to any person obtaining a 
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the 
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
* DEALINGS IN THE SOFTWARE.
*
*******************************************************************************/

namespace ProvisionData.Specifications
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class ExpressionVisitor<TVisitor, T, TDto>
        where TVisitor : ISpecificationVisitor<TVisitor, T>
    {
        public Expression<Func<TDto, Boolean>> Expr { get; protected set; }

        public abstract Expression<Func<TDto, Boolean>> ConvertSpecToExpression(ISpecification<T, TVisitor> spec);

        [SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "Required for Single Dispatch")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for Single Dispatch")]
        public void Visit(AllSpecification<T, TVisitor> spec) => Expr = _ => true;

        public void Visit(AndSpecification<T, TVisitor> spec)
        {
            var leftExpr = ConvertSpecToExpression(spec.Left);
            var rightExpr = ConvertSpecToExpression(spec.Right);

            var exprBody = Expression.AndAlso(leftExpr.Body, rightExpr.Body);
            Expr = Expression.Lambda<Func<TDto, Boolean>>(exprBody, leftExpr.Parameters.Single());
        }

        public void Visit(OrSpecification<T, TVisitor> spec)
        {
            var leftExpr = ConvertSpecToExpression(spec.Left);
            var rightExpr = ConvertSpecToExpression(spec.Right);

            var exprBody = Expression.Or(leftExpr.Body, rightExpr.Body);
            Expr = Expression.Lambda<Func<TDto, Boolean>>(exprBody, leftExpr.Parameters.Single());
        }

        public void Visit(NotSpecification<T, TVisitor> spec)
        {
            var specExpr = ConvertSpecToExpression(spec);

            var exprBody = Expression.Not(specExpr.Body);
            Expr = Expression.Lambda<Func<TDto, Boolean>>(exprBody, specExpr.Parameters.Single());
        }
    }
}

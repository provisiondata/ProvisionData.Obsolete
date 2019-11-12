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

namespace ProvisionData.EntityFramework
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class DbContextExtensions
    {
        // https://www.tabsoverspaces.com/233797-convenient-method-for-explicit-lazy-loading-in-entity-framework-core-or-entity-framework-6
        public static void LoadRelated<TEntity, TReference>(this DbContext context, TEntity entity, Expression<Func<TEntity, TReference>> selector)
            where TEntity : class
            where TReference : class
        {
            var reference = context.Entry(entity).Reference(selector);
            if (!reference.IsLoaded)
                reference.Load();
        }

        public static void LoadRelated<TEntity, TReference>(this DbContext context, TEntity entity, Expression<Func<TEntity, IEnumerable<TReference>>> selector)
            where TEntity : class
            where TReference : class
        {
            var collection = context.Entry(entity).Collection(selector);
            if (!collection.IsLoaded)
                collection.Load();
        }
    }
}

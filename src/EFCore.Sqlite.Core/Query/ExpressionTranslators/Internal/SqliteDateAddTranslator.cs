// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqliteDateAddTranslator : IMethodCallTranslator
    {
        private static readonly string _sqliteFormatDate = "'%Y-%m-%d %H:%M:%S'";
        private static readonly string _sqliteFunctionDateFormat = "strftime";
        private static readonly string _sqliteUtc = "'utc'";

        private static readonly MethodInfo _concatExpression
            = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(Expression), typeof(string) });

        private readonly Dictionary<MethodInfo, string> _methodInfoDatePartMapping = new Dictionary<MethodInfo, string>
        {
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), "years" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), "months" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), "days" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), "hours" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), "minutes" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), "seconds" }
            // TODO: Future implementation, how to do the translation de TIME ZONE?
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddYears), new[] { typeof(int) }), "years" },
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMonths), new[] { typeof(int) }), "months" },
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddDays), new[] { typeof(double) }), "days" },
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddHours), new[] { typeof(double) }), "hours" },
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMinutes), new[] { typeof(double) }), "minutes" },
            //    { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddSeconds), new[] { typeof(double) }), "seconds" }
        };

        /// <summary>
        ///     Translates the given method call expression.
        /// </summary>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns>
        ///     A SQL expression representing the translated MethodCallExpression.
        /// </returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoDatePartMapping.TryGetValue(methodCallExpression.Method, out var datePart))
            {
                var firstArgument = methodCallExpression.Arguments[0];

                if (firstArgument.NodeType == ExpressionType.Convert)
                {
                    firstArgument = new ExplicitCastExpression(firstArgument, typeof(string));
                }

                var concatDateAdd =
                    firstArgument.NodeType == ExpressionType.Extension
                        ? Expression.Add(firstArgument, new SqlFragmentExpression($"' {datePart}'"), _concatExpression)
                        : (Expression)new SqlFragmentExpression($"'{firstArgument.ToString()}  {datePart}'");

                return new SqlFunctionExpression(
                    functionName: _sqliteFunctionDateFormat,
                    returnType: methodCallExpression.Type,
                    arguments:
                        methodCallExpression.Type == typeof(DateTime)
                            ? new[]
                                {
                                    new SqlFragmentExpression(_sqliteFormatDate),
                                    methodCallExpression.Object,
                                    concatDateAdd
                                }
                            : new[]
                                {
                                    new SqlFragmentExpression(_sqliteFormatDate),
                                    methodCallExpression.Object,
                                    concatDateAdd,
                                    new SqlFragmentExpression(_sqliteUtc)
                                });
            }

            return null;
        }
    }
}

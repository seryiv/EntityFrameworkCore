// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class DbFunctionsSqliteTest : DbFunctionsTestBase<NorthwindQuerySqliteFixture<NoopModelCustomizer>>
    {
        public DbFunctionsSqliteTest(NorthwindQuerySqliteFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        public override void String_DateDiff_Day()
        {
            base.String_DateDiff_Day();

            AssertSql(
                @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE CAST((strftime('%s', ""c"".""OrderDate"") - strftime('%s', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime'))) / 86400 AS INTEGER) = 0");

        }

        public override void String_DateDiff_Month()
        {
            base.String_DateDiff_Month();

            AssertSql(
                @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE (12 * ((CAST(strftime('%Y', ""c"".""OrderDate"") AS INTEGER) - CAST(strftime('%Y', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime')) AS INTEGER)) + (CAST(strftime('%m', ""c"".""OrderDate"") AS INTEGER) - CAST(strftime('%m', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime')) AS INTEGER)))) = 0");

        }

        public override void String_DateDiff_Year()
        {
            base.String_DateDiff_Year();

            AssertSql(
                @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE (CAST(strftime('%Y', ""c"".""OrderDate"") AS INTEGER) - CAST(strftime('%Y', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime')) AS INTEGER)) = 0");

        }

        public override void String_DateDiff_Hour()
        {
            base.String_DateDiff_Hour();

            AssertSql(
               @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE CAST((strftime('%s', ""c"".""OrderDate"") - strftime('%s', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime'))) / 3600 AS INTEGER) = 0");

        }

        public override void String_DateDiff_Minute()
        {
            base.String_DateDiff_Minute();

            AssertSql(
                @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE CAST((strftime('%s', ""c"".""OrderDate"") - strftime('%s', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime'))) / 60 AS INTEGER) = 0");



        }

        public override void String_DateDiff_Second()
        {
            base.String_DateDiff_Second();

            AssertSql(
                @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE (CAST(strftime('%S', ""c"".""OrderDate"") AS INTEGER) - CAST(strftime('%S', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime')) AS INTEGER)) = 0");

        }

        public override void String_DateDiff_Convert_To_Date()
        {
            base.String_DateDiff_Convert_To_Date();

            AssertSql(
               @"SELECT COUNT(*)
FROM ""Orders"" AS ""c""
WHERE CAST((strftime('%s', ""c"".""OrderDate"") - strftime('%s', strftime('%Y-%m-%d %H:%M:%S', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime'), 'start of day'))) / 86400 AS INTEGER) = 0");

        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}

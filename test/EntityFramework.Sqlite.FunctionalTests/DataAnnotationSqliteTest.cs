﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Metadata;
using Xunit;

namespace Microsoft.Data.Entity.Sqlite.FunctionalTests
{
    public class DataAnnotationSqliteTest : DataAnnotationTestBase<SqliteTestStore, DataAnnotationSqliteFixture>
    {
        public DataAnnotationSqliteTest(DataAnnotationSqliteFixture fixture)
            : base(fixture)
        {
        }

        public override void ConcurrencyCheckAttribute_throws_if_value_in_database_changed()
        {
            base.ConcurrencyCheckAttribute_throws_if_value_in_database_changed();

            Assert.Equal(@"SELECT ""r"".""UniqueNo"", ""r"".""MaxLengthProperty"", ""r"".""Name"", ""r"".""RowVersion""
FROM ""Sample"" AS ""r""
WHERE ""r"".""UniqueNo"" = 1
LIMIT 1

SELECT ""r"".""UniqueNo"", ""r"".""MaxLengthProperty"", ""r"".""Name"", ""r"".""RowVersion""
FROM ""Sample"" AS ""r""
WHERE ""r"".""UniqueNo"" = 1
LIMIT 1

@p2: 1
@p0: ModifiedData
@p1: 00000000-0000-0000-0003-000000000001
@p3: 00000001-0000-0000-0000-000000000001

UPDATE ""Sample"" SET ""Name"" = @p0, ""RowVersion"" = @p1
WHERE ""UniqueNo"" = @p2 AND ""RowVersion"" = @p3;
SELECT changes();

@p2: 1
@p0: ChangedData
@p1: 00000000-0000-0000-0002-000000000001
@p3: 00000001-0000-0000-0000-000000000001

UPDATE ""Sample"" SET ""Name"" = @p0, ""RowVersion"" = @p1
WHERE ""UniqueNo"" = @p2 AND ""RowVersion"" = @p3;
SELECT changes();",
                Sql);
        }

        public override void DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity()
        {
            base.DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity();

            Assert.Equal(@"@p0: 
@p1: Third
@p2: 00000000-0000-0000-0000-000000000003

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2);
SELECT ""UniqueNo""
FROM ""Sample""
WHERE changes() = 1 AND ""UniqueNo"" = last_insert_rowid();",
                Sql);
        }

        public override void MaxLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(10, context.Model.GetEntityType(typeof(One)).GetProperty("MaxLengthProperty").GetMaxLength());
            }
        }

        public override void NotMappedAttribute_ignores_the_property()
        {
            base.NotMappedAttribute_ignores_the_property();

            Assert.Equal(@"@p0: 
@p1: Fourth
@p2: 00000000-0000-0000-0000-000000000004

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2);
SELECT ""UniqueNo""
FROM ""Sample""
WHERE changes() = 1 AND ""UniqueNo"" = last_insert_rowid();",
                Sql);
        }

        public override void RequiredAttribute_throws_while_inserting_null_value()
        {
            base.RequiredAttribute_throws_while_inserting_null_value();

            Assert.Equal(@"@p0: 
@p1: ValidString
@p2: 00000000-0000-0000-0000-000000000001

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2);
SELECT ""UniqueNo""
FROM ""Sample""
WHERE changes() = 1 AND ""UniqueNo"" = last_insert_rowid();

@p0: 
@p1: 
@p2: 00000000-0000-0000-0000-000000000002

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2);
SELECT ""UniqueNo""
FROM ""Sample""
WHERE changes() = 1 AND ""UniqueNo"" = last_insert_rowid();",
                Sql);
        }

        public override void StringLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(16, context.Model.GetEntityType(typeof(Two)).GetProperty("Data").GetMaxLength());
            }
        }

        public override void TimestampAttribute_throws_if_value_in_database_changed()
        {
//            base.TimestampAttribute_throws_if_value_in_database_changed();

//            Assert.Equal(@"SELECT ""r"".""Id"", ""r"".""Data"", ""r"".""Timestamp""
//FROM ""Two"" AS ""r""
//WHERE ""r"".""Id"" = 1
//LIMIT 1

//SELECT ""r"".""Id"", ""r"".""Data"", ""r"".""Timestamp""
//FROM ""Two"" AS ""r""
//WHERE ""r"".""Id"" = 1
//LIMIT 1

//@p2: 1
//@p0: ModifiedData
//@p1: System.Byte[]
//@p3: System.Byte[]

//UPDATE ""Two"" SET ""Data"" = @p0, ""Timestamp"" = @p1
//WHERE ""Id"" = @p2 AND ""Timestamp"" = @p3;
//SELECT changes();

//@p2: 1
//@p0: ChangedData
//@p1: System.Byte[]
//@p3: System.Byte[]

//UPDATE ""Two"" SET ""Data"" = @p0, ""Timestamp"" = @p1
//WHERE ""Id"" = @p2 AND ""Timestamp"" = @p3;
//SELECT changes();",
//                Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
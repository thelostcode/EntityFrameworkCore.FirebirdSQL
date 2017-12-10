/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using FirebirdSql.Data.FirebirdClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class FbDatabaseModelFactory : IDatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;

        private const string NamePartRegex
            = @"(?:(?:\[(?<part{0}>(?:(?:\]\])|[^\]])+)\])|(?<part{0}>[^\.\[\]]+))";

        private static readonly Regex _partExtractor
            = new Regex(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"^{0}(?:\.{1})?$",
                    string.Format(CultureInfo.InvariantCulture, NamePartRegex, 1),
                    string.Format(CultureInfo.InvariantCulture, NamePartRegex, 2)),
                RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(1000.0));

        public FbDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
            => _logger = logger;

        public DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            using (var connection = new FbConnection(connectionString))
            {
                return Create(connection, tables, schemas);
            }
        }

        public DatabaseModel Create(DbConnection connection, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            var databaseModel = new DatabaseModel();

            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            databaseModel.DefaultSchema = null;
            var tableList = tables.ToList();
            var tableFilter = GenerateTableFilter(tableList.Select(p => p).ToList());

            return databaseModel;
        }

        private void GetTables(
            DbConnection connection,
            Func<string, string> tableFilter,
            DatabaseModel databaseModel)
        {
            using (var command = connection.CreateCommand())
            {
                var commandBuilder = new StringBuilder();
                commandBuilder.Append("SELECT RDB$RELATION_NAME FROM ");
                commandBuilder.Append("RDB$RELATIONS t ");

                var filter = $"WHERE t.RDB$RELATION_NAME <> '{HistoryRepository.DefaultTableName}' { (tableFilter != null ? $" AND {tableFilter("t.RDB$RELATION_NAME")}" : "")}";

                commandBuilder.Append(filter);
                commandBuilder.Append(" AND RDB$VIEW_BLR IS NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)");
                command.CommandText = commandBuilder.ToString();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetValueOrDefault<string>("RDB$RELATION_NAME");

                        _logger.TableFound(name);

                        var table = new DatabaseTable
                        {
                            Schema = name,
                            Name = name
                        };

                        databaseModel.Tables.Add(table);
                    }
                }
            }
        }

        private static Func<string, string> GenerateTableFilter(IReadOnlyList<string> tables)
        {
            if (tables.Any())
            {
                return (t) =>
                {
                    var tableFilterBuilder = new StringBuilder();

                    tableFilterBuilder.Append(t);
                    tableFilterBuilder.Append(" IN (");
                    tableFilterBuilder.Append(string.Join(", ", tables.Select(e => EscapeLiteral(e))));
                    tableFilterBuilder.Append(")");

                    return tableFilterBuilder.ToString();
                };
            }
            return null;
        }

        private static string EscapeLiteral(string s) => $"N'{s}'";
    }
}

﻿using Newtonsoft.Json.Linq;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Exceptions;
using Tachyon.Server.Common.DatabricksClient.Models.Enums;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Handlers
{
    internal class DatabricksResultHandler : IDatabricksResultHandler
    {
        public async Task<List<T>> HandleResultAsync<T>(StatementResult result, CancellationToken cancellationToken)
        {
            var columnMap = result.Manifest.Schema.Columns
                .ToDictionary(column => column.Position, column => column.Name);

            var dataRows = result.Result?.Data ?? Enumerable.Empty<List<string>>();

            var parsedResult = dataRows
                .Select(row => CreateObject<T>(row, columnMap));

            return await Task.FromResult(parsedResult.ToList());
        }

        private static T CreateObject<T>(IReadOnlyList<string> row, IReadOnlyDictionary<int, string> columnMap)
        {
            var obj = new JObject();
            for (var i = 0; i < row.Count; i++)
            {
                obj[columnMap[i]] = row[i];
            }
            return obj.ToObject<T>() ?? throw new DatabricksException(ErrorCode.PARSE_ERROR, $"Failed to parse row to {typeof(T).Name}");
        }
    }
}
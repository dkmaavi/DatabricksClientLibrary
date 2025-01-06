using Newtonsoft.Json.Linq;
using Tachyon.Server.Common.DatabricksClient.ApiRequest;
using Tachyon.Server.Common.DatabricksClient.ApiResponse;

namespace Tachyon.Server.Common.DatabricksClient
{
    public class DatabricksRepository
    {
        private readonly DatabricksApi statementApiClient;

        protected DatabricksRepository(DatabricksApi statementApiClient)
        {
            this.statementApiClient = statementApiClient;
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery)
        {
            var response = await statementApiClient.SendQueryAsync(statementQuery);

            if (response.Status.State == State.Succeeded)
            {
                return ParseResult<T>(response);
            }
            else
            {
                throw new Exception($"Error executing query: {response.Status.Error.ErrorCode} - {response.Status.Error.Message}");
            }
        }

        public async Task ExecuteQueryAsync(StatementQuery statementQuery)
        {
            var response = await statementApiClient.SendQueryAsync(statementQuery);

            if (response.Status.State != State.Succeeded)
            {
                throw new Exception($"Error executing query: {response.Status.Error.ErrorCode} - {response.Status.Error.Message}");
            }
        }

        private List<T> ParseResult<T>(StatementResult statementResult)
        {
            var response = new List<JObject>();
            var columnIndexDictionary = statementResult.Manifest.Schema.Columns.ToDictionary(column => column.Position, column => column.Name);
            var data = statementResult.Result.Data ?? new List<List<string>>();

            foreach (var row in data)
            {
                var obj = new JObject();
                for (int i = 0; i < row.Count; i++)
                {
                    obj.Add(columnIndexDictionary[i], row[i]);
                }

                response.Add(obj);
            }

            return response.Select(x => x.ToObject<T>()).ToList();
        }
    }
}
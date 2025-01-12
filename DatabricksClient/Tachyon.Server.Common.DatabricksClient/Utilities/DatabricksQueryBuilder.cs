using System.Text;
using Tachyon.Server.Common.DatabricksClient.Constants;
using Tachyon.Server.Common.DatabricksClient.Extensions;
using Tachyon.Server.Common.DatabricksClient.Models.Request;

namespace Tachyon.Server.Common.DatabricksClient.Utilities
{
    public class DatabricksQueryBuilder
    {
        private string sqlQuery = string.Empty;
        private Guid tenantId;
        private StringBuilder insertQuery = new();
        private int insertParameterCount;
        private List<StatementQueryParameter> parameters = new();

        public DatabricksQueryBuilder WithTenant(Guid tenantId)
        {
            this.tenantId = tenantId;
            parameters.Add(new StatementQueryParameter
            {
                Name = "tenantId",
                Value = tenantId.ToString(),
                Type = typeof(Guid).ToDbxType()
            });
            return this;
        }

        public DatabricksQueryBuilder WithQuery(string sqlQuery)
        {
            this.sqlQuery = sqlQuery;
            return this;
        }

        public DatabricksQueryBuilder WithParameter(StatementQueryParameter parameter)
        {
            parameters.Add(parameter);
            return this;
        }

        public DatabricksQueryBuilder WithRecord(params object[] values)
        {
            insertQuery.Append("(");
            foreach (var value in values)
            {
                if (value is string stringValue && stringValue.StartsWith(":"))
                {
                    insertQuery.Append($"{value}, ");
                }
                else
                {
                    insertParameterCount++;
                    var paramName = $":param{insertParameterCount}";
                    insertQuery.Append($"{paramName}, ");
                    parameters.Add(new StatementQueryParameter
                    {
                        Name = paramName.Substring(1),
                        Value = value.ToString(),
                        Type = value.GetType().ToDbxType()
                    });
                }
            }

            if (insertQuery.Length > 1)
            {
                insertQuery.Length -= 2;
            }

            insertQuery.Append("),");
            return this;
        }

        public StatementQuery Build()
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                throw new ArgumentNullException(nameof(sqlQuery), "SQL query cannot be null or empty.");
            }

            if (!sqlQuery.Contains("tenantId", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("The SQL query must include the tenantId parameter.");
            }

            if (insertQuery.Length > 0)
            {
                insertQuery.Length--;
            }

            sqlQuery = sqlQuery.Replace(DatabricksConstant.InsertQueryToken, insertQuery.ToString());

            return new StatementQuery
            {
                Statement = sqlQuery,
                Parameters = parameters
            };
        }
    }
}
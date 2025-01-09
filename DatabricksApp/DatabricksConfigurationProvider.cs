using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Configuration;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

namespace DatabricksApp
{
    internal class DatabricksConfigurationProvider : IDatabricksConfigurationProvider
    {
        
        public HttpClientSettings GetHttpClientSettings()
        {
            return new HttpClientSettings
            {
               
            };
        }
        public StatementApiSettings GetStatementApiSettings()
        {
            return new StatementApiSettings
            {
              
            };
        }
    }
}

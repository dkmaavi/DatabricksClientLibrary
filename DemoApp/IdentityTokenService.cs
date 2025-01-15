using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{
    using Azure.Identity;
    using Azure.Core;

    public class IdentityTokenService
    {

        public async Task<string> GetAccessTokenAsync()
        {
            // Explicitly identify the User-Assigned Managed Identity
           var _credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = "0e4d428f-70b6-4b98-82d7-a9f56752b7fd"
           });

            var tokenRequestContext = new TokenRequestContext(
            new[] { "https://portal.azure.com/.default" }
        );

            var token = await _credential.GetTokenAsync(tokenRequestContext);
            return token.Token;
        }
    }

}

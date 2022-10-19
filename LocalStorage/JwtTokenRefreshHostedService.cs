using Forge.Security.Jwt.Client.Storage.Browser.Abstraction;
using Forge.Security.Jwt.Client.Storage.Browser.Models;
using Forge.Security.Jwt.Shared.Client.Api;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Forge.Security.Jwt.Client.Storage.Browser.LocalStorage
{

    /// <summary>Automatically refresh the token before it expires</summary>
    public class JwtTokenRefreshHostedService : JwtTokenRefreshHostedServiceBase
    {

        /// <summary>Initializes a new instance of the <see cref="JwtTokenRefreshHostedService" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="jsRuntime">The JSRuntime.</param>
        /// <param name="apiService">The communication service.</param>
        /// <param name="authenticationStateProvider">The authentication state provider.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <param name="authCoreOptions">The client authentication core options.</param>
        /// <param name="browserStorageOptions">The browser storage options.</param>
        public JwtTokenRefreshHostedService(ILogger<JwtTokenRefreshHostedService> logger,
            IJSRuntime jsRuntime,
            ITokenizedApiCommunicationService apiService,
            AuthenticationStateProvider authenticationStateProvider,
            IAdditionalData additionalData,
            IOptions<JwtClientAuthenticationCoreOptions> authCoreOptions,
            IOptions<BrowserStorageOptions> browserStorageOptions) : 
            base(logger, jsRuntime, apiService, authenticationStateProvider, additionalData, StorageModeEnum.LocalStorage, authCoreOptions, browserStorageOptions)
        {
        }

    }

}

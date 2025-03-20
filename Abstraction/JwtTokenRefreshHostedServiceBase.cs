using Forge.Security.Jwt.Client.Storage.Browser.Models;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Client.Storage.Browser.Abstraction
{

    /// <summary>Automatically refresh the token before it expires</summary>
    public abstract class JwtTokenRefreshHostedServiceBase : IRefreshTokenService
    {

        private readonly ILogger _logger;
        private readonly IJSRuntime _jsRuntime;
        private readonly IJwtTokenAuthenticationStateProvider _authenticationStateProvider;
        private readonly IAdditionalData _additionalData;
        private readonly StorageModeEnum _storageMode;
        private readonly JwtClientAuthenticationCoreOptions _authCoreOptions;
        private readonly BrowserStorageOptions _browserStorageOptions;
        private readonly DataStore _dataStore;

        private readonly DotNetObjectReference<JwtTokenRefreshHostedServiceBase> _reference;

        private ParsedTokenData _parsedTokenData = new ParsedTokenData();

        /// <summary>Initializes a new instance of the <see cref="JwtTokenRefreshHostedServiceBase" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="authenticationStateProvider">The authentication state provider.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <param name="storageMode">The storage mode.</param>
        /// <param name="dataStore">The dataStore.</param>
        /// <param name="authCoreOptions">The authentication core options.</param>
        /// <param name="browserStorageOptions">The browser storage options.</param>
        /// <exception cref="System.ArgumentNullException">logger
        /// or
        /// jsRuntime
        /// or
        /// apiService
        /// or
        /// authenticationStateProvider
        /// or
        /// additionalData
        /// or
        /// authCoreOptions
        /// or
        /// browserStorageOptions</exception>
        protected JwtTokenRefreshHostedServiceBase(ILogger logger,
            IJSRuntime jsRuntime,
            AuthenticationStateProvider authenticationStateProvider,
            IAdditionalData additionalData,
            StorageModeEnum storageMode,
            DataStore dataStore,
            IOptions<JwtClientAuthenticationCoreOptions> authCoreOptions,
            IOptions<BrowserStorageOptions> browserStorageOptions)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (jsRuntime == null) throw new ArgumentNullException(nameof(jsRuntime));
            if (authenticationStateProvider == null) throw new ArgumentNullException(nameof(authenticationStateProvider));
            if (additionalData == null) throw new ArgumentNullException(nameof(additionalData));
            if (dataStore == null) throw new ArgumentNullException(nameof(dataStore));
            if (authCoreOptions == null) throw new ArgumentNullException(nameof(authCoreOptions));
            if (browserStorageOptions == null) throw new ArgumentNullException(nameof(browserStorageOptions));

            _logger = logger;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = (IJwtTokenAuthenticationStateProvider)authenticationStateProvider;
            _additionalData = additionalData;
            _dataStore = dataStore;
            _storageMode = storageMode;
            _authCoreOptions = authCoreOptions.Value;
            _browserStorageOptions = browserStorageOptions.Value;

            _reference = DotNetObjectReference.Create<JwtTokenRefreshHostedServiceBase>(this);

            _logger.LogDebug($"{this.GetType().Name}.ctor, AuthenticationStateProvider, hash: {authenticationStateProvider.GetHashCode()}");
        }

        /// <summary>Starts the service</summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>
        ///   Task
        /// </returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync, starting");

            await ConnectToBrowser();

            _authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateChangedEventHandler;

            await ConfigureServiceAsync();

            _logger.LogInformation("StartAsync, started");
        }

        /// <summary>Stops the service</summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>
        ///   Task
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync, stopping");

            _authenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateChangedEventHandler;

            _logger.LogInformation("StopAsync, stopped");

            return Task.CompletedTask;
        }

        /// <summary>Callbacks the receive authentication response.</summary>
        /// <param name="authenticationResponseStr">The authentication response string.</param>
        /// <returns>
        ///   JSON string as a result
        /// </returns>
        [JSInvokable]
        public async Task<string> CallbackReceiveAuthenticationResponse(string authenticationResponseStr)
        {
            IAuthenticationResponse authenticationResponse = JsonSerializer.Deserialize(authenticationResponseStr, 
                _browserStorageOptions.AuthenticationResponseType, 
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IAuthenticationResponse;

            _dataStore.TokenData = _parsedTokenData = await _authenticationStateProvider.ParseTokenAsync(authenticationResponse);

            ReceiveAuthenticationResult result = new ReceiveAuthenticationResult();
            result.ParsedTokenData = _parsedTokenData;
            result.StartService = _parsedTokenData.RefreshTokenExpireAt > DateTime.UtcNow;
            result.ServiceDueTime = GetDueTimeForService();

            return JsonSerializer.Serialize(result, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private async void AuthenticationStateChangedEventHandler(Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> task)
        {
            _logger.LogInformation("AuthenticationStateChangedEventHandler, authentication state changed");
            _parsedTokenData = await GetParsedTokenDataAsync();
            await ConfigureServiceAsync();
        }

        private async Task ConnectToBrowser()
        {
            Assembly assembly = typeof(JwtTokenRefreshHostedServiceBase).Assembly;
            string resourceName = string.Format("{0}.RefreshTokenService.js", typeof(JwtTokenRefreshHostedServiceBase).Assembly.GetName().Name);
            string jsScript = string.Empty;
            
            _logger.LogDebug($"ConnectToBrowser, resourceName: {resourceName}");

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                jsScript = await reader.ReadToEndAsync();
            }

            _logger.LogDebug($"ConnectToBrowser, invoking JS 'eval', script length: {jsScript?.Length}");

            await _jsRuntime.InvokeVoidAsync("eval", jsScript);

            string refreshUrl = _authCoreOptions.BaseAddress;
            if (!refreshUrl.EndsWith("/")) refreshUrl = $"{refreshUrl}/";

            _logger.LogDebug($"ConnectToBrowser, refresh url: {refreshUrl}");
            _logger.LogDebug("ConnectToBrowser, invoking JS 'initRefreshTokenService'");

            await _jsRuntime.InvokeVoidAsync("initRefreshTokenService", 
                _reference, 
                $"{refreshUrl}{_authCoreOptions.RefreshUri}", 
                (int)_storageMode,
                _additionalData.SecondaryKeys
                );

            _logger.LogDebug("ConnectToBrowser, invoking JS 'initRefreshTokenService'");

            _parsedTokenData = await GetParsedTokenDataAsync();
        }

        private async Task ConfigureServiceAsync()
        {
            _logger.LogDebug($"ConfigureServiceAsync, current time: {DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss:ttt")}, refresh token will expire: {_parsedTokenData.RefreshTokenExpireAt.ToString("yyyy.MM.dd HH:mm:ss:ttt")}");
            
            if (_parsedTokenData.RefreshTokenExpireAt < DateTime.UtcNow)
            {
                // token has already expired
                _logger.LogInformation("ConfigureServiceAsync, refresh token expired. It is not possible to regenerate the current access token, if it exists.");
                await _jsRuntime.InvokeVoidAsync("stopRefreshTokenService");
            }
            else
            {
                // start timer
                int dueTime = GetDueTimeForService();
                _logger.LogInformation($"ConfigureServiceAsync, timer due time value: {dueTime} ms");
                await _jsRuntime.InvokeVoidAsync("startRefreshTokenService", dueTime.ToString());
            }
        }

        private int GetDueTimeForService()
        {
            int dueTime = Convert.ToInt32(TimeSpan.FromTicks(_parsedTokenData.RefreshTokenExpireAt.Ticks - DateTime.UtcNow.Ticks).TotalMilliseconds) - _authCoreOptions.RefreshTokenBeforeExpirationInMilliseconds;
            if (dueTime < 0) dueTime = 0;

            _logger.LogInformation($"GetDueTimeForService, timer due time value: {dueTime} ms");
            
            return dueTime;
        }

        private async Task<ParsedTokenData> GetParsedTokenDataAsync()
        {
            ParsedTokenData result = _dataStore.TokenData;
            if (string.IsNullOrWhiteSpace(result.AccessToken)) result = await _authenticationStateProvider.GetParsedTokenDataAsync();
            return result;
        }

    }

}

using Forge.Security.Jwt.Client.Storage.Browser.Abstraction;
using Forge.Wasm.BrowserStorages.Services.SessionStorage;
using Microsoft.Extensions.Logging;

namespace Forge.Security.Jwt.Client.Storage.Browser.SessionStorage
{

    /// <summary>Stores data into the browser's session storage</summary>
    public class Storage : StorageBase
    {

        /// <summary>Initializes a new instance of the <see cref="Storage" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sessionStorage">The session storage.</param>
        public Storage(ILogger<Storage> logger, ISessionStorageServiceAsync sessionStorage) : base(sessionStorage)
        {
            logger.LogDebug($"Storage.ctor, ISessionStorageServiceAsync, hash: {sessionStorage.GetHashCode()}");
        }

    }

}

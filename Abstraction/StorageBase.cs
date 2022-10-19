using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Storage;
using Forge.Wasm.BrowserStorages.Services.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Client.Storage.Browser.Abstraction
{

    /// <summary>Stores data into the browser's local storage</summary>
    public abstract class StorageBase : IStorage<ParsedTokenData>
    {

        private readonly ILogger _logger;
        private readonly IServiceAsync _storage;

        /// <summary>Initializes a new instance of the <see cref="StorageBase" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="storage">The local storage.</param>
        /// <exception cref="System.ArgumentNullException">localStorage</exception>
        protected StorageBase(ILogger logger, IServiceAsync storage)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (storage == null) throw new ArgumentNullException(nameof(storage));
            _logger = logger;
            _storage = storage;
        }

        /// <summary>Clears items from the storage</summary>
        public async Task ClearAsync()
        {
            await _storage.ClearAsync();
        }

        /// <summary>Determines whether the specified key exist or not.</summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _storage.ContainsKeyAsync(key);
        }

        /// <summary>Gets stored data</summary>
        /// <returns>List of data</returns>
        public async Task<IEnumerable<ParsedTokenData>> GetAsync()
        {
            IEnumerable<string> keys = await _storage.KeysAsync();
            return keys.Select((key) =>
            {
                return _storage.GetAsync<ParsedTokenData>(key).GetAwaiter().GetResult();
            }).ToList();
        }

        /// <summary>Gets the item by key</summary>
        /// <param name="key">The key.</param>
        /// <returns>Data or default</returns>
        public async Task<ParsedTokenData> GetAsync(string key)
        {
            return await _storage.GetAsync<ParsedTokenData>(key);
        }

        /// <summary>Removes an item from the storage</summary>
        /// <param name="key">The key.</param>
        /// <returns>True, if it was successful, otherwise, False.</returns>
        public async Task<bool> RemoveAsync(string key)
        {
            bool result = await _storage.ContainsKeyAsync(key);
            await _storage.RemoveAsync(key);
            return result;
        }

        /// <summary>Sets an item</summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        public async Task SetAsync(string key, ParsedTokenData data)
        {
            await _storage.SetAsync<ParsedTokenData>(key, data);
        }

    }

}

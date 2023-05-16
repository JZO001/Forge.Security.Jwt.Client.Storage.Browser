using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Storage;
using Forge.Wasm.BrowserStorages.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Client.Storage.Browser.Abstraction
{

    /// <summary>Stores data into the browser's local storage</summary>
    public abstract class StorageBase : IStorage<ParsedTokenData>
    {

        private readonly IServiceAsync _storage;

        /// <summary>Initializes a new instance of the <see cref="StorageBase" /> class.</summary>
        /// <param name="storage">The local storage.</param>
        /// <exception cref="System.ArgumentNullException">localStorage</exception>
        protected StorageBase(IServiceAsync storage)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));

            _storage = storage;
        }

        /// <summary>Clears items from the storage</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            await _storage.ClearAsync(cancellationToken);
        }

        /// <summary>Determines whether the specified key exist or not.</summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if the specified key exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _storage.ContainsKeyAsync(key, cancellationToken);
        }

        /// <summary>Gets stored data</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of data</returns>
        public async Task<IEnumerable<ParsedTokenData>> GetAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<string> keys = await _storage.KeysAsync(cancellationToken);

            List<ParsedTokenData> result = new List<ParsedTokenData>();
            
            foreach (string key in keys)
            {
                result.Add(await _storage.GetAsync<ParsedTokenData>(key, cancellationToken));
            }

            return result;
        }

        /// <summary>Gets the item by key</summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Data or default</returns>
        public async Task<ParsedTokenData> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _storage.GetAsync<ParsedTokenData>(key, cancellationToken);
        }

        /// <summary>Removes an item from the storage</summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True, if it was successful, otherwise, False.</returns>
        public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            bool result = await _storage.ContainsKeyAsync(key, cancellationToken);
            await _storage.RemoveAsync(key, cancellationToken);
            return result;
        }

        /// <summary>Sets an item</summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SetAsync(string key, ParsedTokenData data, CancellationToken cancellationToken = default)
        {
            await _storage.SetAsync<ParsedTokenData>(key, data, cancellationToken);
        }

    }

}

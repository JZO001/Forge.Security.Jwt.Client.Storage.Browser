using Blazored.SessionStorage;
using Forge.Security.Jwt.Shared.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Client.Storage.Browser
{

    /// <summary>Stores data into the browser's session storage</summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class SessionStorage<TData> : IStorage<TData>
    {

        private readonly ISessionStorageService _sessionStorage;

        /// <summary>Initializes a new instance of the <see cref="SessionStorage{TData}" /> class.</summary>
        /// <param name="sessionStorage">The session storage.</param>
        /// <exception cref="System.ArgumentNullException">sessionStorage</exception>
        public SessionStorage(ISessionStorageService sessionStorage)
        {
            if (sessionStorage == null) throw new ArgumentNullException(nameof(sessionStorage));
            _sessionStorage = sessionStorage;
        }

        /// <summary>Clears items from the storage</summary>
        public async Task ClearAsync()
        {
            await _sessionStorage.ClearAsync();
        }

        /// <summary>Determines whether the specified key exist or not.</summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _sessionStorage.ContainKeyAsync(key);
        }

        /// <summary>Gets stored data</summary>
        /// <returns>List of data</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<TData>> GetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new NotSupportedException();
        }

        /// <summary>Gets the item by key</summary>
        /// <param name="key">The key.</param>
        /// <returns>Data or default</returns>
        public async Task<TData> GetAsync(string key)
        {
            TData result = default(TData);
            try
            {
                result = await _sessionStorage.GetItemAsync<TData>(key);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
            return result;
        }

        /// <summary>Removes an item from the storage</summary>
        /// <param name="key">The key.</param>
        /// <returns>True, if it was successful, otherwise, False.</returns>
        public async Task<bool> RemoveAsync(string key)
        {
            bool result = await _sessionStorage.ContainKeyAsync(key);
            await _sessionStorage.RemoveItemAsync(key);
            return result;
        }

        /// <summary>Sets an item</summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        public async Task SetAsync(string key, TData data)
        {
            await _sessionStorage.SetItemAsync<TData>(key, data);
        }

    }

}

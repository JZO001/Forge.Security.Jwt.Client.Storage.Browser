using Blazored.LocalStorage;
using Forge.Security.Jwt.Shared.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Client.Storage.Browser
{

    /// <summary>Stores data into the browser's local storage</summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class LocalStorage<TData> : IStorage<TData>
    {

        private readonly ILocalStorageService _localStorage;

        /// <summary>Initializes a new instance of the <see cref="LocalStorage{TData}" /> class.</summary>
        /// <param name="localStorage">The local storage.</param>
        /// <exception cref="System.ArgumentNullException">localStorage</exception>
        public LocalStorage(ILocalStorageService localStorage)
        {
            if (localStorage == null) throw new ArgumentNullException(nameof(localStorage));
            _localStorage = localStorage;
        }

        /// <summary>Clears items from the storage</summary>
        public async Task ClearAsync()
        {
            await _localStorage.ClearAsync();
        }

        /// <summary>Determines whether the specified key exist or not.</summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _localStorage.ContainKeyAsync(key);
        }

        /// <summary>Gets stored data</summary>
        /// <returns>List of data</returns>
        public async Task<IEnumerable<TData>> GetAsync()
        {
            IEnumerable<string> keys = await _localStorage.KeysAsync();
            return keys.Select((key) => 
            { 
                return _localStorage.GetItemAsync<TData>(key).GetAwaiter().GetResult(); 
            }).ToList();
        }

        /// <summary>Gets the item by key</summary>
        /// <param name="key">The key.</param>
        /// <returns>Data or default</returns>
        public async Task<TData> GetAsync(string key)
        {
            TData result = default(TData);
            try
            {
                result = await _localStorage.GetItemAsync<TData>(key);
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
            bool result = await _localStorage.ContainKeyAsync(key);
            await _localStorage.RemoveItemAsync(key);
            return result;
        }

        /// <summary>Sets an item</summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        public async Task SetAsync(string key, TData data)
        {
            await _localStorage.SetItemAsync<TData>(key, data);
        }

    }

}

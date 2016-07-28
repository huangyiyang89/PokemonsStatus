#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;

using PokemonGo.RocketAPI;
using POGOProtos.Data;
using POGOProtos.Networking.Responses;

#endregion

namespace PokemonsStatus
{
    public class Inventory
    {
        private readonly Client _client;
        private GetInventoryResponse _cachedInventory;
        private DateTime _lastRefresh;

        public Inventory(Client client)
        {
            _client = client;
        }
        
        private async Task<GetInventoryResponse> GetCachedInventory()
        {
            var now = DateTime.UtcNow;
            var ss = new SemaphoreSlim(10);

            if (_lastRefresh.AddSeconds(30).Ticks > now.Ticks)
            {
                return _cachedInventory;
            }
            await ss.WaitAsync();
            try
            {
                _lastRefresh = now;
                _cachedInventory = await _client.Inventory.GetInventory();
                return _cachedInventory;
            }
            finally
            {
                ss.Release();
            }
        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await GetCachedInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p.PokemonId > 0);
        }

       

    }
}
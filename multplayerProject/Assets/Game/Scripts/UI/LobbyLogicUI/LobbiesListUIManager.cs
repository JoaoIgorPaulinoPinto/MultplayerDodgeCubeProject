using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbiesListUIManager : MonoBehaviour {



    async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 24,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            Debug.Log("lobbies found: " + queryResponse.Results.Count);

            foreach (var result in queryResponse.Results)
            {
                Debug.Log(result.Name + " " + result.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }

    }
}

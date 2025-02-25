using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private List<Transform> players = new List<Transform>();
    [Range(0f, 1f)]
    public float rotationSmoothness = 0.1f; 

    public float rotationHolderIndex; 

    public void UpdatePlayersList()
    {
        players.Clear();

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.ConnectedClientsList.Count > 0)
        {
            foreach (var item in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (item.PlayerObject != null)
                {
                    players.Add(item.PlayerObject.transform);
                }
            }
        }
    }

    private void Update()
    {
        UpdatePlayersList();
        CameraMovement();
    }

    void CameraMovement()
    {
        if (players.Count == 0) return;

       
        Vector3 center = Vector3.zero;
        foreach (var player in players)
        {
            center += player.position;
        }
        center /= players.Count;

      
        Quaternion targetRotation = Quaternion.LookRotation((center/rotationHolderIndex) - transform.position);

        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness);
    }
}

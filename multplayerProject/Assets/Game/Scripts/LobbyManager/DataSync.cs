using Unity.Netcode;
using UnityEngine;
public class DataSync : MonoBehaviour
{
    public static DataSync instance = new DataSync();
    public string playername;
    private void Awake()
    {
        if (instance != null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }
    public void SetClientDataName(string _name)
    {
        playername = _name;
    }
}

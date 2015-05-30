using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    public ScrollList scrollList;

    private bool isRefreshing = false;


    //STATIC CLASS
    public static void StartSession(string name) {
        if(name == null || name == ""){
            Debug.Log("Can't create a session without a name.");
            return;
        }
        Debug.Log("Starting...");
    }

    public static void Connect(string url)
    {
        TNAutoJoin autoJoin = FindObjectOfType<TNAutoJoin>();
        char[] splitchar = { ':' };
        autoJoin.serverAddress = url.Split(splitchar)[0];
        autoJoin.Connect();
    }

    public static void Refresh()
    {
        Debug.Log("Refreshing...");
        //Master Server Request
    }


    //EVENTS
    void OnNetworkError(string error) {
    }
    void OnNetworkConnect (bool success, string message){
    }
    void OnNetworkDisconnect(){
    }

    /*
    public override void SessionListUpdated(Map<System.Guid, UdpSession> sessionList) {
        if (sessionList == null || sessionList.Count == 0)
        {
            Debug.Log("No sessions found.");
        }
        else
        {
            scrollList.itemList = new List<ScrollItem>();

            foreach (var entry in sessionList)
            {
                UdpSession session = entry.Value;
                ScrollItem item = new ScrollItem();
                item.id = entry.Key;
                item.name = session.HostName;
                item.fullName = (item.name + " (" + session.ConnectionsCurrent + "/" + session.ConnectionsMax + ")");
                item.ip = (session.HasLan) ? session.LanEndPoint.Address.ToString() : session.WanEndPoint.Address.ToString();

                scrollList.itemList.Add(item);
            }

            scrollList.PopulateList();
        }
    }*/

    //GAMEPLAY
    private void SpawnPlayer() {
        Debug.Log("Spawn");
        TNManager.Create(Game.Get().onlinePlayer, 
                         Game.Get().activeSpawn.transform.position, 
                         Game.Get().activeSpawn.transform.rotation);
    }
}

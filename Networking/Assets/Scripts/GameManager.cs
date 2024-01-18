using System.Collections.Generic;
using UnityEngine;
using System;
using static Packet;

public class GameManager : MonoBehaviour
{
    public Dictionary<byte, PacketHandle> Handlers = new();
    public delegate void PacketHandle(Packet packet);
    public static GameManager instance;
    
    public GameObject playerPrefab;
    public GameObject localPlayerPrefab;
    public Dictionary<int, Player> PlayerList = new();

    // lists for safety we can only instantiate object on main thread
    public List<Action> actions = new();
    public List<Action> actionsCopy = new();
    bool hasAction = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        Handlers.Add((byte)PacketID.S_welcome, WelcomeRecieved);
        Handlers.Add((byte)PacketID.S_spawnPlayer, SpawnPlayer);
        Handlers.Add((byte)PacketID.S_playerDisconnected, PlayerDisconnected);
        Handlers.Add((byte)PacketID.S_playerPosition, PlayerPosition);
        Handlers.Add((byte)PacketID.S_playerRotation, PlayerRotation);
    }
    private void Update()
    {
        if (hasAction)
        {
            actionsCopy.Clear();
            lock(actions)
            {
                actionsCopy.AddRange(actions);
                actions.Clear();
                hasAction = false;
            }

            foreach (Action action in actionsCopy) action.Invoke();
        }
    }
    public void WelcomeRecieved(Packet packet) 
    {
        string msg = packet.GetString();
        int yourID = packet.GetInt();
        Debug.Log(msg);

        Packet welcomeRecieved = new Packet();
        welcomeRecieved.Add((byte)PacketID.C_welcomeReceived);
        welcomeRecieved.Add(yourID);
        
        Client.instance.tcp.SendData(welcomeRecieved);
        Client.instance.udp.Connect();
    }
    public void SpawnPlayer(Packet packet) 
    {
        int id = packet.GetInt();
        string name = packet.GetString();
        Vector3 pos = packet.GetVector3();
        Quaternion rot = packet.GetQuaternion();

        GameObject prefabToSpawn = id == Client.instance.Id ? localPlayerPrefab : playerPrefab; 

        lock (actions)
        {
            hasAction = true;
            actions.Add(() =>
            {
                Player newPlayer = Instantiate(prefabToSpawn, pos, rot).GetComponent<Player>();
                newPlayer.name = name;
                PlayerList.Add(id, newPlayer);
            });
        }
    }
    public void PlayerDisconnected(Packet packet)
    {
        int id = packet.GetInt();
        if(PlayerList.TryGetValue(id, out Player player))
        {
            lock (actions)
            {
                hasAction = true;
                actions.Add(() =>
                {
                    PlayerList.Remove(id);
                    Destroy(player.gameObject);
                });
            }
        }
    }
    public void PlayerPosition(Packet packet)
    {
        int id = packet.GetInt();
        Vector3 position = packet.GetVector3();

        if (PlayerList.TryGetValue(id,out Player player)) player.targetPosition = position;
    }
    public void PlayerRotation(Packet packet)
    {
        int id = packet.GetInt();

        if (id == Client.instance.Id) return;

        Quaternion rotation = packet.GetQuaternion();

        if(PlayerList.TryGetValue(id,out Player player)) player.transform.rotation = rotation;
    }
}

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
    }
    public void SpawnPlayer(Packet packet) 
    {
        int id = packet.GetInt();
        string name = packet.GetString();
        Vector3 pos = packet.GetVector3();
        Quaternion rot = packet.GetQuaternion();

        lock (actions)
        {
            hasAction = true;
            actions.Add(() =>
            {
                Player newPlayer = Instantiate(playerPrefab, pos, rot).GetComponent<Player>();
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
}

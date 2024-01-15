using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Packet;

public class GameManager : MonoBehaviour
{
    public Dictionary<byte, PacketHandle> Handlers = new();
    public delegate void PacketHandle(Packet packet);
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        Handlers.Add((byte)PacketID.S_welcome, WelcomeRecieved);
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
}

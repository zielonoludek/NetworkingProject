using System.Collections.Generic;
using UnityEngine;
using static Packet;

public static class PacketHandlers
{
    public static Dictionary<PacketId, PacketHandel> Handlers = new()
    {
        { PacketId.S_Welcome, WelcomeRecived },
        { PacketId.S_SpawnPlayer, SpawnPlayer },
        { PacketId.S_PlayerDisconnected, PlayerDisconnected },
        { PacketId.S_PlayerMoveDirection, PlayerMoveDirection },
        { PacketId.S_PlayerPosition, PlayerPosition }
    };
    public delegate void PacketHandel(Packet packet);

    public static void WelcomeRecived(Packet packet)
    {
        int yourID = packet.GetInt();
        string msg = packet.GetString();

        Client.instance.SetId(yourID);
        Debug.Log(msg);

        PacketSenders.WelcomeReceived();

    }
    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.GetInt();
        string name = packet.GetString();
        Vector3 pos = packet.GetVector3();
        Quaternion rot = packet.GetQuaternion();

        GameManger.instance.SpawnPlayer(id, name, pos, rot);
    }
    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.GetInt();
        GameManger.instance.DisconnectPlayer(id);
    }
    public static void PlayerMoveDirection(Packet packet)
    {
        Vector3 position = packet.GetVector3();
        GameManger.instance.SetPlayerMoveDirection(position);
    }
    public static void PlayerPosition(Packet packet)
    {
        int id = packet.GetInt();
        Vector3 position = packet.GetVector3();
        Quaternion rotation = packet.GetQuaternion();
        GameManger.instance.SetPlayerPosition(id, position, rotation);
    }
}

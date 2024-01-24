using UnityEngine;

public static class PacketSenders
{

    public static void WelcomeReceived()
    {
        Packet packet = new(PacketId.C_WelcomeReceived);
        packet.Write(Client.instance.Id);
        SendTCPData(packet);
    }
    public static void PlayerMovement(bool[] inputs, Vector3 position, Quaternion rotation)
    {
        Packet packet = new(PacketId.C_PlayerInput);
        packet.Write(inputs.Length);
        foreach (bool input in inputs)
        {
            packet.Write(input);
        }
        packet.Write(position);
        packet.Write(rotation);
        SendUDPData(packet);
    }
    public static void PlayerDisconnect()
    {

    }

    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        Client.instance.udp.SendData(_packet);
    }
}

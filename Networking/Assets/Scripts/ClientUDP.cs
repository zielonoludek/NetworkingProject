using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Packet;

public class ClientUDP 
{
    public UdpClient socket;
    public IPEndPoint endPoint;

    public ClientUDP(string ip, int port) => endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

    public void Connect()
    {
        socket = new UdpClient(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        socket.Connect(endPoint);
        socket.BeginReceive(ReceiveCallback, null);
        Packet packet = new();
        SendData(packet);
    }
    public void SendData(Packet packet)
    {
        try 
        {
            packet.InsertInt(Client.instance.Id);
            socket?.BeginSend(packet.ToArray(), packet.Length, null, null);
        }
        catch (Exception ex) { Debug.LogException(ex); }
    }
    private void ReceiveCallback(IAsyncResult result)
    {
        if (socket == null) return;
        try
        {
            byte[] data = socket.EndReceive(result, ref endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            if (data.Length < 4)
            {
                Disconnect();
                return;
            }
            HandleData(data);
        }
        catch { Disconnect(); }
    }
    private void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        byte packetID = packet.GetByte();
        PacketHandlers.Handlers[(PacketId)packetID](packet);
    }
    public void Disconnect()
    {
        Client.instance.Disconnect();
        endPoint = null;
        socket = null;
    }
}

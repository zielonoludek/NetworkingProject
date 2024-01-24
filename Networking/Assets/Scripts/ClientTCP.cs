using System;
using System.Net.Sockets;
using UnityEngine;

public class ClientTCP
{
    public TcpClient socket;
    private NetworkStream stream;
    private byte[] receiveBuffer;


    private const int dataBufferSize = 4096;

    public void Connect(string ip, int port)
    {
        socket = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };

        receiveBuffer = new byte[dataBufferSize];
        socket.BeginConnect(ip, port, ConnectionCallback, socket);
    }

    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="packet">The packet to send.</param>
    public void SendData(Packet packet)
    {
        try
        {
            if (socket != null)
            {
                stream.BeginWrite(packet.ToArray(), 0, packet.Length, null, null); // Send data to server
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to server via TCP: {ex}");
        }
    }

    private void ConnectionCallback(IAsyncResult result)
    {
        socket.EndConnect(result);

        if (!socket.Connected)
        {
            return;
        }

        stream = socket.GetStream();

        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReciveCallback, null);
    }
    private void ReciveCallback(IAsyncResult result)
    {
        if (stream == null) { return; }
        try
        {
            int byteLen = stream.EndRead(result);
            if (byteLen <= 0)
            {
                Disconnect();
                return;
            }

            byte[] data = new byte[byteLen];
            Array.Copy(receiveBuffer, data, byteLen);
            HandelData(data);
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReciveCallback, null);
        }
        catch
        {
            Disconnect();
        }
    }

    private void HandelData(byte[] data)
    {
        Packet packet = new(data);
        byte packetID = packet.GetByte();
        PacketHandlers.Handlers[(PacketId)packetID](packet);
    }

    public void Disconnect()
    {
        Client.instance.Disconnect();
        stream = null;
        socket = null;
        receiveBuffer = null;
    }
}

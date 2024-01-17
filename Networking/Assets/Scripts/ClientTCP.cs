using System;
using System.Net.Sockets;
using UnityEngine;

public class ClientTCP
{
    private TcpClient socket;
    private NetworkStream stream;
    private byte[] recieveBuffer;

    public const int dataBufferSize = 4096;

    public void Connect(string ip, int port)
    {
        socket = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };

        recieveBuffer = new byte[dataBufferSize];
        socket.BeginConnect(ip, port, CinnectionCallback, socket);
    }
    public void SendData(Packet packet) 
    {
        byte[] packetBytes = packet.ToArray();
        try
        {
            if (socket != null) stream.BeginWrite(packetBytes, 0, packetBytes.Length, null, null);
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    public void CinnectionCallback(IAsyncResult result) 
    {
        socket.EndConnect(result);

        if (!socket.Connected) return;
        
        stream = socket.GetStream();

        stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
    }
    private void RecieveCallback(IAsyncResult result)
    {
        try
        {
            int byteLen = stream.EndRead(result);
            if (byteLen <= 0) return;
            byte[] data = new byte[byteLen];
            Array.Copy(recieveBuffer, data, byteLen);
            HandleData(data);
            stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    private void HandleData(byte[] data) 
    {
        Packet packet = new(data);
        byte packetID = packet.GetByte();
        GameManager.instance.Handlers[packetID](packet);
    }
    public void Disconnect()
    {
        stream.Close();
        socket.Close();
        stream = null;
        socket = null;
    }
}

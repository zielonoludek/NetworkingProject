using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Packet
{
    List<byte> writableData = new List<byte>();
    byte[] data;
    int readPos = 0;
    private int packetLength = 0;

    public Packet() { }
    public Packet(byte[] data)
    {
        this.data = data;
        packetLength = GetInt();
    }
    public byte[] ToArray() 
    { 
        writableData.InsertRange(0, BitConverter.GetBytes(writableData.Count));
        return writableData.ToArray();
    }
    public byte GetByte()
    {
        byte value = data[readPos];
        readPos += 4;
        return value;
    }
    public byte[] GetBytes(int len)
    {
        byte[] value = new byte[len];
        Array.Copy(data,readPos,value,0,len);
        readPos += len;
        return value;
    }
    public int GetInt() 
    { 
        int value = BitConverter.ToInt32(data,readPos);
        readPos += 4;
        return value;
    }
    public string GetString()
    {
        int len = GetInt();
        string val =  Encoding.ASCII.GetString(data, readPos, len);
        readPos += len;
        return val;
    }

    public void Add(byte value) => writableData.Add(value);
    public void Add(byte[] value) => writableData.AddRange(value);
    public void Add(int value) => writableData.AddRange(BitConverter.GetBytes(value));

    public enum PacketID
    {
        S_welcome = 1,
        S_spawnPlayer = 2,
        S_playerPosition = 3,
        S_playerRotation = 4,
        S_playerShoot = 5,
        S_playerDisconnected = 6,
        S_playerHealth = 7,
        S_playerDead = 8,
        S_playerRespawned = 9,


        C_spawnPlayer = 126,
        C_welcomeReceived = 127,
        C_playerMovement = 128,
        C_playerShoot = 129,
        C_playerHit = 130,
    }
}

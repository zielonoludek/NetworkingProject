using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public enum PacketId
{
    S_Welcome = 1,
    S_SpawnPlayer = 2,
    S_PlayerDisconnected = 3,
    S_PlayerMoveDirection = 4,
    S_PlayerPosition = 5,

    C_WelcomeReceived = 126,
    C_Disconnect = 127,
    C_SpawnPlayer = 128,
    C_PlayerInput = 129
}

public class Packet
{
    private readonly List<byte> buffer;
    private readonly byte[] readableBuffer;
    private int readPos;

    /// <summary>Creats an empty packet</summary>
    public Packet()
    {
        buffer = new();
        readableBuffer = Array.Empty<byte>();
        readPos = 0;
    }

    /// <summary>Creats a packet with specifid id</summary>
    /// <param name="id">The id of the packet</param>
    public Packet(PacketId id)
    {
        buffer = new();
        readableBuffer = Array.Empty<byte>();
        readPos = 0;
        Write((byte)id);
    }

    /// <summary>Creats a packet with existing data</summary>
    /// <param name="data">The packet data</param>
    public Packet(byte[] data)
    {
        buffer = new();
        readableBuffer = data;
        readPos = 0;
    }

    /// <summary>Gets the length of the packet</summary>
    public int Length { get { return buffer.Count; } }


    /// <summary>Converts the packet to a byte array</summary>
    /// <returns>The packet as a byte[]</returns>
    public byte[] ToArray()
    {
        return buffer.ToArray();
    }

    /// <summary>Inserts the given id at the start of the buffer.</summary>
    /// <param name="clientId">The id to insert.</param>
    public void InsertInt(int clientId)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(clientId)); // Insert the int at the start of the buffer
    }


    /// <summary>Adds a byte to the packet</summary>
    /// <param name="value">The byte to add</param>
    public void Write(byte value)
    {
        buffer.Add(value);
    }

    /// <summary>Adds a byte array to the packet the length of the array will be writen first</summary>
    /// <param name="value">The byte[] to add</param>
    public void Write(byte[] value)
    {
        Write(value.Length);
        buffer.AddRange(value);
    }

    /// <summary>Adds a int to the packet</summary>
    /// <param name="value">The int to add</param>
    public void Write(int value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    /// <summary>Adds a float to the packet</summary>
    /// <param name="value">The float to add</param>
    public void Write(float value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    /// <summary>Adds a bool to the packet</summary>
    /// <param name="value">The bool to add</param>
    public void Write(bool value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    /// <summary>Adds a char to the packet</summary>
    /// <param name="value">The char to add</param>
    public void Write(char value)
    {
        buffer.Add(Convert.ToByte(value));
    }

    /// <summary>Adds a string to the packet the length of the string will be writen first</summary>
    /// <param name="value">The string to add</param>
    public void Write(string value)
    {
        Write(value.Length);
        buffer.AddRange(Encoding.ASCII.GetBytes(value));
    }

    /// <summary>Adds a Vector2 to the packet</summary>
    /// <param name="value">The Vector2 to add</param>
    public void Write(Vector2 value)
    {
        Write(value.x);
        Write(value.y);
    }

    /// <summary>Adds a Vector3 to the packet</summary>
    /// <param name="value">The Vector3 to add</param>
    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    /// <summary>Adds a Quaternion to the packet</summary>
    /// <param name="value">The Quaternion to add</param>
    public void Write(Quaternion value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }

    /// <summary>Gets a byte from the packet and moves the readPos by 1</summary>
    /// <returns>The byte read</returns>
    public byte GetByte()
    {
        if (readableBuffer.Length > readPos)
        {
            byte value = readableBuffer[readPos];
            readPos += 1;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    /// <summary>Gets a byte array from the packet and moves the readPos by the length of the byte array</summary>
    /// <returns>The byte[] read</returns>
    public byte[] GetBytes()
    {
        if (readableBuffer.Length > readPos)
        {
            int length = GetInt();
            byte[] value = new byte[length];
            Array.Copy(readableBuffer, readPos, value, 0, length);
            readPos += length;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    /// <summary>Gets a int from the packet and moves the readPos by 4</summary>
    /// <returns>The int read</returns>
    public int GetInt()
    {
        if (readableBuffer.Length > readPos)
        {
            int value = BitConverter.ToInt32(readableBuffer, readPos);
            readPos += 4;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    /// <summary>Gets a float from the packet and moves the readPos by 4</summary>
    /// <returns>The float read</returns>
    public float GetFloat()
    {
        if (readableBuffer.Length > readPos)
        {
            float value = BitConverter.ToSingle(readableBuffer, readPos);
            readPos += 4;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    /// <summary>Gets a bool from the packet and moves the readPos by 1</summary>
    /// <returns>The bool read</returns>
    public bool GetBool()
    {
        if (readableBuffer.Length > readPos)
        {
            bool value = BitConverter.ToBoolean(readableBuffer, readPos);
            readPos += 1;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    /// <summary>Gets a char from the packet and moves the readPos by 1</summary>
    /// <returns>The char read</returns>
    public char GetChar()
    {
        if (readableBuffer.Length > readPos)
        {
            char value = BitConverter.ToChar(readableBuffer, readPos);
            readPos += 1;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'char'!");
        }
    }

    /// <summary>Gets a string from the packet and moves the readPos by the length of the string</summary>
    /// <returns>The string read</returns>
    public string GetString()
    {
        if (readableBuffer.Length > readPos)
        {
            int length = GetInt();
            string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
            readPos += length;
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    /// <summary>Gets a Vector3 from the packet and moves the readPos by 12</summary>
    /// <returns>The Vector3 read</returns>
    public Vector3 GetVector3()
    {
        return new Vector3(GetFloat(), GetFloat(), GetFloat());
    }

    /// <summary>Gets a Quaternion from the packet and moves the readPos by 16</summary>
    /// <returns>The Quaternion read</returns>
    public Quaternion GetQuaternion()
    {
        return new Quaternion(GetFloat(), GetFloat(), GetFloat(), GetFloat());
    }
}


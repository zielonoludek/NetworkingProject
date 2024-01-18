using UnityEngine;
using static Packet;

public class Client : MonoBehaviour
{
    public ClientTCP tcp = new();
    public ClientUDP udp = new();
    public static Client instance;

    public string playerName = "Player1";

    public string ip = "127.0.0.1";
    public int port = 25565;
    public int Id = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }
    public void JoinGame()
    {
        Packet packet = new Packet();
        packet.Add((byte)Packet.PacketID.C_spawnPlayer);
        packet.Add(playerName);
        tcp.SendData(packet);
    }
    public void SetName(string name) => playerName = name;
    public void Connect() => tcp.Connect(ip, port);
    public void Disconnect()
    {
        tcp.Disconnect();
        if (GameManager.instance.PlayerList.TryGetValue(Id, out Player player))
        {
            GameManager.instance.PlayerList.Remove(Id);
            Destroy(player.gameObject);
        }
    }
}
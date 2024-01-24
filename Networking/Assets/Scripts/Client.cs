using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;

    public int Id => id;

    private int id = 0;

    [SerializeField] private string ip = "127.0.0.1";
    [SerializeField] private int port = 25565;

    public ClientTCP tcp;
    public ClientUDP udp;

    private bool isConnected = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying myself!");
            Destroy(this);
        }
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        tcp = new();
        udp = new(ip, port);

        isConnected = true;
        tcp.Connect(ip, port); // Connect tcp, udp gets connected once tcp is done
    }
    public void SetId(int id)
    {
        this.id = id;
        udp.Connect();
    }
    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
            GameManger.instance.DisconnectPlayer(Id);
        }
    }


    private void OnApplicationQuit()
    {
        Disconnect(); // Disconnect when the game is closed
    }
}

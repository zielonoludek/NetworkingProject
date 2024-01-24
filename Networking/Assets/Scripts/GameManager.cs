using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject localPlayerPrefab;

    private readonly Dictionary<int, Player> PlayerList = new();

    private string userName = "Player";
    public void SetUserName(string userName) => this.userName = userName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

    }

    public void JoinGame()
    {
        Packet packet = new(PacketId.C_SpawnPlayer);
        packet.Write(userName);
        Client.instance.tcp.SendData(packet);
    }


    public void SpawnPlayer(int id, string name, Vector3 pos, Quaternion rot)
    {
        GameObject prefabToSpawn = id == Client.instance.Id ? localPlayerPrefab : playerPrefab;
        Player newPlayer = Instantiate(prefabToSpawn, pos, rot).GetComponent<Player>();
        newPlayer.name = name;
        PlayerList.Add(id, newPlayer);
    }
    public void DisconnectPlayer(int id)
    {
        if (PlayerList.TryGetValue(id, out Player player))
        {
            PlayerList.Remove(id);
            Destroy(player.gameObject);
        }
    }
    public void SetPlayerMoveDirection(Vector3 direction)
    {
        if (PlayerList.TryGetValue(Client.instance.Id, out Player player))
        {
            player.targetPosition = direction;
        }
    }
    public void SetPlayerPosition(int id, Vector3 position, Quaternion rotation)
    {
        if (PlayerList.TryGetValue(id, out Player player))
        {
            player.transform.SetPositionAndRotation(position, rotation);
        }
    }
}

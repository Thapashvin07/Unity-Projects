using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class Menu : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;
    public static List<Card> InitSpawncards;
    public static List<Card> DeckCards;
    [SerializeField] GameObject _player;
    void Awake()
    {
        CreateCardDB();
    }
    async void Start()
    {
        // ConnectToRoom();
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
        runner = this.gameObject.GetComponent<NetworkRunner>();        
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "CardGame",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            Scene = SceneRef.FromIndex(1),
        });
    }
    public void CreateCardDB()
    {
        InitSpawncards = new List<Card>();
        InitSpawncards.Add(new Card(1, "Pawn", 1, new Ability((int)Player.CardAbilityType.GainPts, 1)));
        InitSpawncards.Add(new Card(2, "Ace", 1, new Ability((int)Player.CardAbilityType.GainPts, 2)));
        InitSpawncards.Add(new Card(3, "Knight", 1, new Ability((int)Player.CardAbilityType.GainPts, 2)));
        InitSpawncards.Add(new Card(4, "Unicorn", 2, new Ability((int)Player.CardAbilityType.GainPts, 3)));
        InitSpawncards.Add(new Card(5, "DarkHorse", 2, new Ability((int)Player.CardAbilityType.GainPts, 4)));
        InitSpawncards.Add(new Card(6, "Bishop", 2, new Ability((int)Player.CardAbilityType.GainPts, 3)));
        InitSpawncards.Add(new Card(7, "BigBishop", 3, new Ability((int)Player.CardAbilityType.GainPts, 4)));
        InitSpawncards.Add(new Card(8, "Rook", 3, new Ability((int)Player.CardAbilityType.GainPts, 5)));
        InitSpawncards.Add(new Card(9, "Rookie", 3, new Ability((int)Player.CardAbilityType.GainPts, 5)));

        DeckCards = new List<Card>(InitSpawncards);
        DeckCards.Add(new Card(10, "BigBishop", 3, new Ability((int)Player.CardAbilityType.GainPts, 4)));
        DeckCards.Add(new Card(11, "Rook", 3, new Ability((int)Player.CardAbilityType.GainPts, 5)));
        DeckCards.Add(new Card(12, "Queen", 4, new Ability((int)Player.CardAbilityType.DoublePts, 4)));
        DeckCards.Add(new Card(13, "King", 4,new Ability((int)Player.CardAbilityType.TriplePts, 3)));
        DeckCards.Add(new Card(14, "Jack", 5,new Ability((int)Player.CardAbilityType.GainPts, 5)));
        DeckCards.Add(new Card(15, "Joker", 5,new Ability((int)Player.CardAbilityType.MakeOppMinusTwo, 4)));
        DeckCards.Add(new Card(16, "Heart", 6,new Ability((int)Player.CardAbilityType.GainPts, 5)));
        DeckCards.Add(new Card(17, "Spade", 6,new Ability((int)Player.CardAbilityType.DoublePts, 3)));
        
    }
    public void LeaveRoom()
    {
        // runner.LeaveRoom();
    }
    public async void ConnectToRoom()
    {
        // if(runner != null) return;
        // runner = gameObject.AddComponent<NetworkRunner>();
        runner = this.gameObject.GetComponent<NetworkRunner>();        
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "CardGame",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
        });
    }
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject  obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject  obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner Runner, PlayerRef player)
    {
        Debug.Log("[OnPlayerJoined]");
        if(player == Runner.LocalPlayer)Runner.Spawn(_player);
    }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player){ }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input){ }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server"+(runner.IsSharedModeMasterClient));
        if(runner.ActivePlayers.Count() >2)
        {
            Debug.Log("Both players joined, loading game scene...");
            return;
        }
        Debug.Log("Players Joined:" + runner.ActivePlayers.Count());
    }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Disconnected from server: " + reason.ToString());
    }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,byte[] token)  { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){ }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){ }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner,PlayerRef player,ReliableKey key,float progress){}
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner,PlayerRef player,ReliableKey key,ArraySegment<byte> data){ }

}

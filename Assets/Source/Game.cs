using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    //Prefabs
    public GameObject onlinePlayer;
    public GameObject offlinePlayer;

    //References
    public Camera playerCamera;
    [System.NonSerialized]
    public CPlayer controlledPlayer;
    public Spawn activeSpawn;
    public List<Spawn> spawns;


    void Awake()
    {
    }

    public enum GameState {
        PLAYING,
        PAUSE,
        STOP
    }

    private static Game game;

    public static Game Get(){
        if(game){
            return game;
        }
        else{
            game = FindObjectOfType<Game>();
            return game;
        }
    }

    void Start() {
        StartGame();
        spawns = new List<Spawn>(FindObjectsOfType<Spawn>());

        //Set Default spawn if neccesary
        if (!activeSpawn)
        {
            activeSpawn = spawns[0];
        }
        activeSpawn.SetActiveSpawn();
    }

    void Update() {
        if (Input.GetKeyUp("escape")) {
            StopGame();
        }
    }


    //Static Game
    public static GameState state = GameState.STOP;


    public static void StartGame(){
        state = GameState.PLAYING;
        //GUIManager.Get().Hide("Menu");
        GUIManager.Get().Hide("PauseMenu");
        GUIManager.Get().Show("UI");
    }

    public static void PauseGame() {
        state = GameState.PAUSE;
        GUIManager.Get().Hide("UI");
        GUIManager.Get().Show("PauseMenu");
    }

    public static void StopGame() {
        state = GameState.STOP;
        GUIManager.Get().Hide("UI");
        GUIManager.Get().Hide("PauseMenu");
        //GUIManager.Get().Show("Menu");
        if(TNManager.isConnected)
            TNManager.Disconnect();
        else
            Application.LoadLevel("menu");
    }

    public static bool IsMobile(){
        return (Application.platform == RuntimePlatform.Android
             || Application.platform == RuntimePlatform.IPhonePlayer);
    }
}

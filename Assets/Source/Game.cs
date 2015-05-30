using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    //Prefabs
    public GameObject onlinePlayer;
    public GameObject offlinePlayer;

    //References
    public Camera playerCamera;
    public Transform playerSpawn;
    public CPlayer controlledPlayer;


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

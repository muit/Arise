using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {

    void Start() {
    }

    public void StartSession(InputField gameName) {
        NetworkManager.StartSession(gameName.text);
    }

    public void Connect(InputField ip) {
        NetworkManager.Connect(ip.text);
    }

    public void playOffline() {
        Application.LoadLevel("lobby");
    }

    public void Quit() {
        Application.Quit();
    }

}

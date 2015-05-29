using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollButton : MonoBehaviour
{
    public System.Guid id;
    public Button button;
    public Text name;
    public Text ip;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Join);
    }

    public void Join() {
        //NetworkManager.Connect(id);
    }
}
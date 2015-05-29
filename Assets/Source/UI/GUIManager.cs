using UnityEngine;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    private static GUIManager gui;

    public static GUIManager Get()
    {
        if (gui)
        {
            return gui;
        }
        else
        {
            gui = FindObjectOfType<GUIManager>();
            return gui;
        }
    }

    public List<Canvas> uis;

	void Start () {
        //Get Uis
        transform.GetComponentsInChildren<Canvas>(uis);
	}
	
	void Update () {
        if (Input.GetKeyDown("escape")) {
            Game.StopGame();
        }
	}

    public void Show(string name) {
        Transform ui = transform.FindChild(name);
        if(ui)
            ui.gameObject.SetActive(true);
    }

    public void Hide(string name){
        Transform ui = transform.FindChild(name);
        if (ui)
            ui.gameObject.SetActive(false);
    }

    public Canvas Find(string name) {
        Transform canvasTrans = transform.FindChild(name);
        if (canvasTrans)
            return canvasTrans.GetComponent<Canvas>();
        else return null;
    }

    public void OpenMenu() {
        
    }
}

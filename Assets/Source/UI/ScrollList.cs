using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScrollItem
{
    public System.Guid id;
    public string name;
    public string fullName;
    public string ip;
    public Sprite icon;
    public Button.ButtonClickedEvent onClick;
}

public class ScrollList : MonoBehaviour
{

    public NetworkManager network;
    public GameObject scrollButton;
    public List<ScrollItem> itemList;

    public Transform contentPanel;

    void Start()
    {
        PopulateList();
    }

    public void PopulateList()
    {
        //Reset List
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        foreach (ScrollItem item in itemList)
        {
            GameObject newButton = Instantiate(scrollButton) as GameObject;
            ScrollButton button = newButton.GetComponent<ScrollButton>();
            button.id = item.id;
            button.gameObject.name = item.name;
            button.name.text = item.fullName;
            button.ip.text = item.ip;
            newButton.transform.SetParent(contentPanel);
        }
    }

    public void OnClick()
    {
        Debug.Log("I done did something!");
    }

    public void SomethingElseToDo(GameObject item)
    {
        Debug.Log(item.name);
    }
}
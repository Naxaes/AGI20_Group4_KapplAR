using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{

    private Level level;
    private GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        GameObject interaction = GameObject.Find("Interaction");
        level = interaction.GetComponent<Level>();
        canvas = GameObject.Find("Canvas");

        GameObject button = (GameObject) Resources.Load("Prefabs/UI/InventoryItemButton");

        int buttonCount = 0;
        foreach (var item in level.inventory.inventory)
        {
            GameObject newButton = Instantiate(button);
            newButton.transform.SetParent(canvas.transform);
            newButton.transform.position += new Vector3(190 * buttonCount++, 0, 0);
            newButton.transform.GetChild(0).GetComponent<Text>().text = item.Key.label;
            newButton.transform.GetChild(1).GetComponent<Text>().text = item.Value.ToString();
            newButton.GetComponent<Button>().onClick.AddListener(() => { changeSelectedItem(item.Key, newButton); });
            if (item.Key.blocId == level.inventory.currentItem.blocId)
            {
                newButton.transform.GetComponent<Image>().color = new Color( 38.0f/255, 224.0f/255, 85.0f/255); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void changeSelectedItem(Bloc item, GameObject selectedButton)
    {
        level.inventory.SelectItem(item);
        foreach(var i in GameObject.FindGameObjectsWithTag("InventoryItem"))
        {
            i.GetComponent<Image>().color = Color.white;
        }
        selectedButton.GetComponent<Image>().color = new Color(38.0f / 255, 224.0f / 255, 85.0f / 255);

    }

    public void Quit()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}

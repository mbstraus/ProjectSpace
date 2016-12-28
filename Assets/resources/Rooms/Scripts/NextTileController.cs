using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTileController : MonoBehaviour {

    public Sprite roomNS;
    public Sprite roomNW;
    public Sprite roomNWS;
    public Sprite roomNWSE;
    public Image nextTileImage;

    // Use this for initialization
    void Start() {
        getNewTile();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.N))
        {
            getNewTile();
        }
    }

    public void getNewTile()
    {
        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            nextTileImage.sprite = roomNS;
        }
        else if (rand == 1)
        {
            nextTileImage.sprite = roomNW;
        }
        else if (rand == 2)
        {
            nextTileImage.sprite = roomNWS;
        }
        else if (rand == 3)
        {
            nextTileImage.sprite = roomNWSE;
        }
        else
        {
            Debug.LogError("Unsupported tile code " + rand);
        }
    }
}

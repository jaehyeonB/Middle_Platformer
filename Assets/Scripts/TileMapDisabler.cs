using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapDisabler : MonoBehaviour
{

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void Awake()
    {
        GetComponent<TilemapRenderer>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExcavationProcesser : MonoBehaviour
{
    private GenerationTilemap generator;
    [SerializeField] private Tilemap dirtMap;
    [SerializeField] private Tilemap stoneMap;
    // Start is called before the first frame update
    void Start()
    {
        generator=FindObjectOfType<GenerationTilemap>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

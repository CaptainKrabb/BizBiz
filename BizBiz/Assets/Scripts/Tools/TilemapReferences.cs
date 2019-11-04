using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilemapReferences : MonoBehaviour 
{
    int timer = 0;
    private GameObject grid;
    private List<GameObject> ground;
    [SerializeField]
    private GridLayout gridLayout;
    public GridLayout GridLayout {get{return gridLayout;}}
    [SerializeField]
    private List<Tilemap> groundTilemaps = new List<Tilemap>();
    public List<Tilemap> GroundTilemaps {get{return groundTilemaps;}}
    private List<Transform> decorationsTransform;
    [SerializeField]
    private List<Vector3Int> decorationPositions = new List<Vector3Int>();
    public List<Vector3Int> DecorationPositions {get{return decorationPositions;}set{decorationPositions=value;}}

    void Awake() {
        grid = GameObject.Find("Grid - Level");
        ground = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Base")).ToList();
        decorationsTransform = GameObject.Find("Decorations").GetComponentsInChildren<Transform>().ToList();
        gridLayout = grid.GetComponent<GridLayout>();

        foreach (GameObject level in ground){
            groundTilemaps.Add(level.GetComponent<Tilemap>());
        }
        foreach (Transform decoration in decorationsTransform) {
            decorationPositions.Add(gridLayout.WorldToCell(decoration.position) - new Vector3Int(1, 1, 0));
        }
    }
 }
 
    

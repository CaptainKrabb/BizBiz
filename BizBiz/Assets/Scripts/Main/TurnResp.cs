using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnResp : MonoBehaviour
{
    private List<GameObject> entities;
    private List<Character> characters = new List<Character>();
    public List<Character> Characters {get{return characters;}set{characters=value;}}
    private List<Vector3Int> entitiesPos = new List<Vector3Int>();
    public List<Vector3Int> EntitiesPos {get{return entitiesPos;}set{entitiesPos=value;}}
    private int indexTurn = 0;
    public int IndexTurn {get{return indexTurn;}set{indexTurn=value;}}

    void Start()
    {
        entities = new List<GameObject>();
        entities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        entities.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach( GameObject entity in  entities) {
            Character chara = entity.GetComponent<Character>() as Character;
            characters.Add(chara);
            entitiesPos.Add(chara.curCellPos);
        }
        characters[0].Turn = true;
        foreach(Character chara in characters.GetRange(1, characters.Count -1)) {
            chara.Turn = false;
        }
    }

    public void Next() {
        Renew();
        for (int i = 0; i < characters.Count; i++) {
            if (i == indexTurn) {
                characters[i].Movementpoint = 3;
                characters[i].Powerpoint = 3;
                characters[i].IsUsingSpell = -1;
            }
            if (i == (indexTurn + 1) % characters.Count)
                characters[i].Turn = true;
            else 
                characters[i].Turn = false;
        }
        indexTurn = (indexTurn + 1) % characters.Count;
    }

    public void Renew() {
        entities = new List<GameObject>();
        entities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        entities.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        characters = new List<Character>();
        entitiesPos = new List<Vector3Int>();
        foreach( GameObject entity in  entities) {
            Character chara = entity.GetComponent<Character>() as Character;
            characters.Add(chara);
            entitiesPos.Add(chara.curCellPos);
        }
    }
}

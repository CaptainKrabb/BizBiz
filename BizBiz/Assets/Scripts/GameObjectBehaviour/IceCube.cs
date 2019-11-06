using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IceCube : MonoBehaviour
{
    private Sprite blocSprite;
    private SpriteRenderer sprtRend;
    private SortingGroup sortingGroup;
    private int indexTurn;
    private TurnResp turnResp;
    private int duration;
    public int Duration {get{return duration;} set{duration=value;}}
    private bool turnPassed = false;
    
    void Start() {
        sprtRend = gameObject.AddComponent<SpriteRenderer>();
        sortingGroup = gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Foreground";
        blocSprite = (Sprite) Resources.Load("Sprite/decoration_sword_in_the_stone", typeof(Sprite));
        sprtRend.sprite = blocSprite;
        sprtRend.sortingOrder= 2;
        turnResp = GameObject.Find("TurnResp").GetComponent<TurnResp>() as TurnResp;
        indexTurn = turnResp.IndexTurn;
    }

    void Update()
    {
        if (turnResp.IndexTurn != indexTurn) {
            turnPassed = true;
        }
        if (turnPassed && indexTurn == turnResp.IndexTurn) {
            turnPassed = false;
            duration--;
        }
        if(duration == 0) {
            Destroy(gameObject);
        }
    }
}

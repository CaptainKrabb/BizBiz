using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchingSpells : MonoBehaviour
{
    private TurnResp turnResp;
    void Start() {
        turnResp = GameObject.Find("TurnResp").GetComponent<TurnResp>() as TurnResp;
    }
    public void spell1() {
        Character chara = turnResp.Characters[turnResp.IndexTurn];
        if (chara.gameObject.tag == "Player") {
            chara.IsUsingSpell = 0;
        }
    }
    public void spell2() {
        Character chara = turnResp.Characters[turnResp.IndexTurn];
        if (chara.gameObject.tag == "Player") {
            chara.IsUsingSpell = 1;
        }
    }
    public void spell3() {
        Character chara = turnResp.Characters[turnResp.IndexTurn];
        if (chara.gameObject.tag == "Player") {
            chara.IsUsingSpell = 2;
        }
    }
    public void spell4() {
        Character chara = turnResp.Characters[turnResp.IndexTurn];
        if (chara.gameObject.tag == "Player") {
            chara.IsUsingSpell = 3;
        }
    }
}

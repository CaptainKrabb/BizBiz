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
        turnResp.Characters[turnResp.IndexTurn].isUsingSpell = 1;
    }
}

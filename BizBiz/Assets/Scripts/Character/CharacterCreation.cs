using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    
    public Character Creation(int life, int pp, int mp, int armur,string class_nam,Vector3Int coord){
       GameObject perso = new GameObject(class_nam);

       Character Chara = perso.AddComponent<Character>() as Character;

       Chara.Lifepoint = life;
       Chara.Powerpoint= pp;
       Chara.Movementpoint= mp;
       Chara.Armor = armur;

       return Chara;
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spells : MonoBehaviour
{
    private TilemapReferences grid;
    private GridLayout gridLayout;
    public enum AllSpells {Verglas,Iceberker,Stalackmite,Stalacktite};
    private AllSpells thisSpell;
    public AllSpells ThisSpell {get{return thisSpell;}set{thisSpell=value;}}

    private string spell_name;
    public string Spell_Name {get{return spell_name;}set{spell_name=value;}}
    private string spell_description;
    public string Spell_Description {get{return spell_description;}set{spell_description=value;}}
    private int spellCost;
    public int SpellCost {get{return spellCost;}}
    private GridShape rangeShape;
    public GridShape RangeShape {get{return rangeShape;}set{rangeShape=value;}}
    private GridShape impactShape = new GridShape();
    public GridShape ImpactShape {get{return impactShape;}set{impactShape=value;}}
    private List<Action> actions = new List<Action>();
    private Vector3Int target;
    private int _upgrade;
    public int _Upgrade {get{return _upgrade;}set{_upgrade=value;}}

    public Spells(AllSpells spell, int upgrade) {
        grid = GameObject.Find("Grid - Level").GetComponent<TilemapReferences>() as TilemapReferences;
        gridLayout = grid.GridLayout;
        thisSpell = spell;
        this._upgrade = upgrade;
        switch(spell) {
            case AllSpells.Verglas :
                spellCost = 3;
                this.spell_name= "Verglas";
                this.spell_description= "";
                rangeShape = new GridShape(1, 3 + upgrade, GridShape.Shape.Cross);
                impactShape = new GridShape(0, 2 + upgrade, GridShape.Shape.T);
                actions.Add(() => setLifepoint(-4 - upgrade));
                actions.Add(() => setMP(-1 - ((int) upgrade/3)));
                break;

            case AllSpells.Iceberker:
                spellCost = 3;
                impactShape = new GridShape(0, 1, GridShape.Shape.Line);
                rangeShape = new GridShape(1, 2, GridShape.Shape.Cross);
                this.spell_description = " soit applique dommage soit crée blocks";
                this.spell_name  = "Icerberker";
                
                actions.Add(() => setLifepoint(Randomizer(-(30 + _upgrade * 20))));
                actions.Add(setBlocks);
                break;
            case AllSpells.Stalackmite:
                impactShape = new GridShape(0,2,GridShape.Shape.Cross);
                rangeShape = new GridShape(1, 2,GridShape.Shape.Circle);
                this.spell_description = " 10% de chance d'enelver tout les mp, 80% d'enelver des mp ";
                this.spell_name  = "Stalackmite";
                actions.Add(() => BizBiz(0, 1, 3, 0.8));
                actions.Add(() => setLifepoint(Randomizer(20+_upgrade*20)));
                break;
            case AllSpells.Stalacktite :
                break;
        }
    }

    
    public void Use( Vector3Int cell ) {
        this.target = cell;
        foreach (Action action in actions) {
            action();
        }
    }

    private void BizBiz(int action, int firstArg, int secondArg, double chances) {
        switch (action) {
            case 0 :
                if ((double) UnityEngine.Random.Range(0F, 1F) < chances) {
                    setMP(firstArg);
                }
                else {
                    setMP(secondArg);
                }
                break;
        }
    }

    private List<Character> occupied(Vector3Int cell) {
        List<Character> occupied = new List<Character>();
        List<GameObject> entities = new List<GameObject>();
        entities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        entities.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (Vector3Int v in impactShape.vectors) {
            foreach (GameObject entity in entities){
                Character chara = entity.GetComponent<Character>() as Character;
                if (chara.curCellPos == v + cell) {
                    occupied.Add(chara);
                }
            }
        }
        return occupied;
    }
   
    //EFFET
    private void setPP(int pp){ //changement statut PP.
        List<Character> targets = occupied(this.target);
        foreach ( Character target in  targets) {
            target.Powerpoint += pp ;
        }
    }
    private void setMP(int mp){ //changement statut MP.
        List<Character> targets = occupied(this.target);
        foreach ( Character target in  targets) {
            target.Movementpoint += mp;
        }
    }
    private void setLifepoint(int damage){//changement statut Lifepoint.
        List<Character> targets = occupied(this.target);
        foreach ( Character target in  targets) {
            target.Lifepoint += damage;
        }
    }
    private void setArmur(int armor){
        List<Character> targets = occupied(this.target);
        foreach ( Character target in  targets) {
            target.Armor += armor;
        }
    }

    private void setBlocks(){// cree un blocks 
        List<Character> targets = occupied(this.target);
        List<Vector3Int> targetsCoord = new List<Vector3Int>();
        foreach(Character c in targets) {
            if(c.curCellPos == impactShape.vectors[0] + this.target)
                return;
            targetsCoord.Add(c.curCellPos);
        }
        List<Vector3Int> impact = new List<Vector3Int>();
        foreach( Vector3Int vector in impactShape.vectors){// pour chaque vecteur possible dans la liste des vecteurs 
            Vector3Int blocks_position = vector + this.target;
            impact.Add(blocks_position);
            if (!(targetsCoord.Contains(blocks_position) || grid.DecorationPositions.Contains(blocks_position))){
                GameObject blocks = new GameObject("Blocs");
                IceCube block = blocks.AddComponent(typeof(IceCube)) as IceCube;
                block.Duration = 3;
                blocks.transform.position = gridLayout.CellToWorld(blocks_position + new Vector3Int(1, 1, 0)) + new Vector3(0, 0.4F, 0);  
                blocks.transform.parent = GameObject.Find("Decorations").transform;
            }
        }    
        List<Vector3Int> jj = grid.DecorationPositions;
        jj.AddRange(impact);
        grid.DecorationPositions = jj;
    }

    // private void setRange(){// changement statut Portée.
    //     List<Character> targets = occupied(this.target);
    //     foreach ( Character target in  targets){
    //           foreach(Spell spell in target.getSpell){
    //               spell.rangeMax_range+= reduceRange;        
    //           }
    //           if (spell.rangeMax_range<0){
    //                spell.rangeMax_range=0;
    //           }
    //     }
    // }

    private int criticalDammage(int nbr){
        int dommagecritic = Mathf.RoundToInt(nbr + nbr * UnityEngine.Random.Range(0F,nbr));
        return dommagecritic;   
    }
    private int Randomizer(int nbr){//randomise l'attaque pour la dynamiser
        int dommage = nbr + Mathf.RoundToInt(UnityEngine.Random.Range(0F, (float) (nbr+1-0.65*nbr))) ;
        return dommage;
    }

    public List<Vector3Int> rightImpact(Vector3Int direction) {
        if(direction.x != 0)
            direction.x = direction.x / Mathf.Abs(direction.x); 
        else
            direction.y = direction.y / Mathf.Abs(direction.y);

        if (this.impactShape.shape == GridShape.Shape.Line) {
            this.impactShape = new GridShape(Mathf.Abs(direction.x), this.impactShape.rangeMax, this.impactShape.shape);
        }
        if (this.impactShape.shape == GridShape.Shape.T) {
            int newRangeMin = GridShape.ways.IndexOf(direction);
            this.impactShape = new GridShape(newRangeMin, this.impactShape.rangeMax, this.impactShape.shape);
        }
        return this.impactShape.vectors;
    }

}




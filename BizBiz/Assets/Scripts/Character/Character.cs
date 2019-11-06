using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Character : MonoBehaviour
{
    private List<Tilemap> tilemaps;
    private GridLayout gridLayout;
    [SerializeField]
    private List<Vector3Int> decoPos;
    private Vector2 curPos;
    public Vector3Int curCellPos;
    private Vector3Int memCellPos;
    private IsometricCharacterRenderer isoRenderer;
    private Vector2 mousePos;
    private Vector3Int selectedCell;
    private Vector3Int memSelectedCellDisp;
    private Vector3Int memSelectedCellSpell;
    private List<Vector3Int> impact = new List<Vector3Int>();

    private Rigidbody2D rbody;

    private Spells.Marks[] marks = new Spells.Marks[1]


    [SerializeField]
    private int lifepoint;
    public int Lifepoint {get {return lifepoint;} set{lifepoint = value ;} }
    [SerializeField]
    private int powerpoint;
    public int Powerpoint{get{return powerpoint;}set{powerpoint= value;}}
    [SerializeField]
    private int movementpoint = 5;
    public int Movementpoint{get{return movementpoint;}set{movementpoint= value;}}
    private int armor;
    public int Armor{get{return movementpoint;}set{armor = value;}}
    private string description;
    public string Description{get{return description;}set{description=value;}}
    private string class_name;
    public string Class_name{get{return class_name;}set{class_name=value;}}
    private Spells[] spells = new Spells[4]; 
    [SerializeField]
    private bool turn;
    public bool Turn{get{return turn;}set{turn = value;}}

    private int isUsingSpell = -1;
    public int IsUsingSpell {set{isUsingSpell = value;}}
    private bool isWalking = false;
    private GridShape movPoss;
    private List<Vector3Int> targetPath = new List<Vector3Int>();
    private List<List<Vector3Int>> targetPaths = new List<List<Vector3Int>>();
    private Vector2 targetCoord;
    [SerializeField]
    private bool doubleClick = false;
    private bool wasUsingSpell = false;
    private TurnResp turnResp;

    void Awake() {
        rbody = GetComponent<Rigidbody2D>();
        isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
    }

    void Start() {
        turnResp = GameObject.Find("TurnResp").GetComponent<TurnResp>() as TurnResp;
        lifepoint = 100;
        this.spells[0] = new Spells(Spells.AllSpells.Iceberker, 0 );
        this.spells[1] = new Spells(Spells.AllSpells.Verglas, 0 );
        TilemapReferences grid = GameObject.Find("Grid - Level").GetComponent<TilemapReferences>() as TilemapReferences;
        tilemaps = grid.GroundTilemaps;
        gridLayout = grid.GridLayout;
        decoPos = grid.DecorationPositions;

        foreach (Tilemap groundTilemap in tilemaps) {
            for (int i = -100; i < 100; i++) {
                for (int j = -100; j < 100; j++) {
                    groundTilemap.SetTileFlags(new Vector3Int(i,j,0), TileFlags.None);
                }
            }
        }
        curPos = rbody.transform.position;
        curCellPos =  gridLayout.WorldToCell(curPos) - new Vector3Int(1, 1, 0);
        rbody.MovePosition(gridLayout.CellToWorld(curCellPos + new Vector3Int(1, 1, 0)) + new Vector3(0, 0.3F, 0));

        PathsGeneration();
    }

    public void upgradeSpell(int index) {
        Spells spell = this.spells[index];
        spell = new Spells(spell.ThisSpell,spell._Upgrade + 1);
    }

    public void setMovement(Vector2 coord,int mov){
    }

    public void useSpells(int index, Vector3Int cell){
        spells[index].Use(cell);
        this.powerpoint -= spells[index].SpellCost;
   }
 
   
    public void setSpell(int index,Spells sort){//index dans le tableau des sorts.
        this.spells[index] = sort;
    }
    public Spells getSpell(int index){
        return this.spells[index];
    }

    void Update(){
        if (lifepoint <= 0){
            Destroy(gameObject);
        }
        
        curPos = rbody.transform.position;
        curCellPos =  gridLayout.WorldToCell(curPos) - new Vector3Int(1, 1, 0);

        if (Turn) {
            foreach (Tilemap groundTilemap in tilemaps) {
                for (int i = -100; i < 100; i++) {
                    for (int j = -100; j < 100; j++) {
                        groundTilemap.SetColor(new Vector3Int(i,j,0), new Color(1F,1F,1F,1F));
                    }
                }
            }
            if(gameObject.tag == "Player") {
                if (isUsingSpell != -1 && powerpoint > 0) {
                    PlayerUsingSpell(isUsingSpell);
                }
                else {
                    PlayerDisplacement();
                }
                if(movementpoint == 0 && powerpoint == 0)
                    turnResp.Next();
            }
            else {
                if (movementpoint > 0) {
                    EnemyDisplacement();
                }
                else {
                    EnemyUsingSpell(0);
                }
            }
        }
    }

    private void PlayerDisplacement() {
        Vector2 mov = new Vector2();
        impact = new List<Vector3Int>();
        memSelectedCellSpell = new Vector3Int();

        if (!isWalking && this.movementpoint > 0){
            foreach (Tilemap groundTilemap in tilemaps) {
                foreach (Vector3Int v in movPoss.vectors)
                    groundTilemap.SetColor(curCellPos + v, Color.blue);
            }
        }

        if (Input.GetMouseButtonDown(0) && this.movementpoint > 0) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCell = gridLayout.WorldToCell(mousePos) - new Vector3Int(1, 1, 0);
            if (selectedCell == memSelectedCellDisp) {
                doubleClick = true;
            }
            else {
                memSelectedCellDisp = selectedCell;
                doubleClick = false;
                if (movPoss.vectors.Contains(selectedCell - curCellPos)) {
                    targetPath = targetPaths[movPoss.vectors.IndexOf(selectedCell - curCellPos)];
                }
            }
        }

        foreach (Tilemap groundTilemap in tilemaps) {
            foreach (Vector3Int v in targetPath) {
                groundTilemap.SetColor(v, Color.green);
            }
        }
        
        if (targetPath.Count > 0 && doubleClick) {
            isWalking = true;
            targetCoord = gridLayout.CellToWorld(targetPath[0] + new Vector3Int(1, 1, 0)) + new Vector3(0, 0.3F, 0);
            if (Mathf.Abs(targetCoord.x - curPos.x) + Mathf.Abs(targetCoord.y - curPos.y) >= 0.1F) {
                mov.x = (targetCoord.x - curPos.x) / Mathf.Abs(targetCoord.x - curPos.x);
                mov.y = ((targetCoord.y - curPos.y) / Mathf.Abs(targetCoord.y - curPos.y) * 0.5F);
            }
            else {
                targetPath.Remove(targetPath[0]);
                this.movementpoint--;
            }     
            Vector3 newPos = curPos + mov * Time.deltaTime;
            isoRenderer.SetDirection(mov);
            rbody.MovePosition(newPos);
        }
        
        if ( wasUsingSpell || (this.movementpoint > 0 && targetPath.Count == 0 &&  memCellPos != curCellPos)) {
            wasUsingSpell = false;
            isWalking = false;
            PathsGeneration();
            memCellPos = curCellPos;
        }
    }

    private void PlayerUsingSpell(int index) {
        wasUsingSpell = true;
        targetPath = new List<Vector3Int>();
        memSelectedCellDisp = new Vector3Int();

        Spells spell = this.spells[index];
        GridShape range = this.spells[index].RangeShape;
        List<Vector3Int> rangeVectors = new List<Vector3Int>(range.vectors);

        List<Vector3Int> toRemove = new List<Vector3Int>();
        List<Vector3Int> obstruct = new List<Vector3Int>();

        foreach (Vector3Int v in rangeVectors) {
            bool tileExist = tilemaps[0].HasTile(v + curCellPos);
            bool isFree = !(decoPos.Contains(v + curCellPos));
            if (tileExist && isFree) {
                if (Obstructed(curCellPos, v + curCellPos)) {
                    obstruct.Add(v);
                }
            }
            else {
                toRemove.Add(v);
            }
        }
        foreach(Vector3Int v in toRemove) {
            rangeVectors.Remove(v);
        }
        foreach(Vector3Int v in obstruct) {
            rangeVectors.Remove(v);
        }

        foreach (Tilemap groundTilemap in tilemaps) {
            foreach (Vector3Int v in rangeVectors)
                groundTilemap.SetColor(curCellPos + v, Color.blue);
            
            foreach (Vector3Int v in obstruct)
                groundTilemap.SetColor(curCellPos + v, Color.grey);
        }
        
        if (Input.GetMouseButtonDown(0)) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCell = gridLayout.WorldToCell(mousePos) - new Vector3Int(1, 1, 0);
            if (rangeVectors.Contains(selectedCell - curCellPos)) {
                Vector3Int direction = selectedCell - curCellPos;
                impact = spell.rightImpact(direction);
                if(memSelectedCellSpell == selectedCell) {
                    useSpells(index, selectedCell);
                }
                else {
                    memSelectedCellSpell = selectedCell;
                }
            }
            else {
                impact = new List<Vector3Int>();
                memSelectedCellSpell = new Vector3Int();
                if(!obstruct.Contains(selectedCell - curCellPos)) {
                    isUsingSpell = -1;
                }
            }
        }
        
        foreach (Tilemap groundTilemap in tilemaps) {
            foreach (Vector3Int v in impact) {
                groundTilemap.SetColor( selectedCell + v, Color.red);
            }
        }
    }

    private void EnemyDisplacement() {
        Vector2 mov = new Vector2();
        if(targetPath.Count == 0)
            targetPath = DisplacementMethods.EnemyPath(curCellPos, movementpoint, 2);
        if (targetPath.Count == 0) {
            movementpoint = 0;
            return;
        }
        targetCoord = gridLayout.CellToWorld(targetPath[0] + new Vector3Int(1, 1, 0)) + new Vector3(0, 0.3F, 0);
        if (Mathf.Abs(targetCoord.x - curPos.x) + Mathf.Abs(targetCoord.y - curPos.y) >= 0.1F) {
            mov.x = (targetCoord.x - curPos.x) / Mathf.Abs(targetCoord.x - curPos.x);
            mov.y = ((targetCoord.y - curPos.y) / Mathf.Abs(targetCoord.y - curPos.y) * 0.5F);
        }
        else {
            targetPath.Remove(targetPath[0]);
            this.movementpoint--;
        }   
        if(targetPath.Count == 0) {
            targetPath = new List<Vector3Int>();
            movementpoint = 0;
        }
        Vector3 newPos = curPos + mov * Time.deltaTime;
        isoRenderer.SetDirection(mov);
        rbody.MovePosition(newPos);
    }

    private void EnemyUsingSpell(int index) {
        Spells spell = this.spells[index];
        GridShape range = this.spells[index].RangeShape;
        List<Vector3Int> rangeVectors = new List<Vector3Int>(range.vectors);
        
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        List<Vector3Int> peoplePos = new List<Vector3Int>();
        foreach (GameObject entity in players) {
            Character chara = entity.GetComponent<Character>() as Character;
            peoplePos.Add(chara.curCellPos);
        }

        foreach(Vector3Int v in rangeVectors) {
            if ( peoplePos.Contains(v + curCellPos)) {
                useSpells(index, v + curCellPos);
                break;
            }
        }
        turnResp.Next();
    }
    
    private void PathsGeneration() {
        targetPaths = new List<List<Vector3Int>>();
        movPoss = new GridShape(1, movementpoint, GridShape.Shape.Circle);
        List<GameObject> entities = new List<GameObject>();
        entities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        entities.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        List<Vector3Int> peoplePos = new List<Vector3Int>();
        foreach (GameObject entity in entities) {
            Character chara = entity.GetComponent<Character>() as Character;
            peoplePos.Add(chara.curCellPos);
        }

        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (Vector3Int v in movPoss.vectors) {
            Vector3Int actualMovPoss = v + curCellPos;
            bool tileExist = tilemaps[0].HasTile(actualMovPoss);
            bool isFree = !(decoPos.Contains(actualMovPoss) || peoplePos.Contains(actualMovPoss));
            if (tileExist && isFree) {
                List<Vector3Int> path = DisplacementMethods.CellPath(curCellPos, actualMovPoss);
                if (path.Count <= movementpoint) {
                    targetPaths.Add(path);
                }
                else {
                    toRemove.Add(v);
                }
            }
            else {
                toRemove.Add(v);
            }
        }
        foreach(Vector3Int v in toRemove) {
            movPoss.vectors.Remove(v);
        }
    }

    private bool Obstructed(Vector3Int start, Vector3Int end) {
        float coef = 0;
        int x1 = start.x;		int y1 = start.y;
	    int x2 = end.x;		    int y2 = end.y;
        int coord_1 = 0; int coord_2 = 0; float coord_3 = 0;
        if (Mathf.Abs(x1 - x2) >= Mathf.Abs(y1 - y2)) {
            coef = (y2 - y1) / (x2 - x1);
            if ((int) Mathf.Min(x1,x2) == x1) {
                coord_1 = x1;
                coord_2 = x2;
                coord_3 = y1;
            }
            else {
                coord_1 = x2;
                coord_2 = x1;
                coord_3 = y2;
            }
            for(int i = coord_1; i <= coord_2; i++) {
                if ( decoPos.Contains(new Vector3Int(i, (int) coord_3, 0))) {
                    return true;
                }
                coord_3 += coef;
            }
        }
        else {
            coef = (x2 - x1) / (y2 - y1);
            if ((int) Mathf.Min(y1,y2) == y1) {
                coord_1 = y1;
                coord_2 = y2;
                coord_3 = x1;
            }
            else {
                coord_1 = y2;
                coord_2 = y1;
                coord_3 = x2;
            }
            for(int i = coord_1; i <= coord_2; i++) {
                if ( decoPos.Contains(new Vector3Int((int) coord_3, i, 0))) {
                    return true;
                }
                coord_3 += coef;
            }
        }
        return false;
    }
}

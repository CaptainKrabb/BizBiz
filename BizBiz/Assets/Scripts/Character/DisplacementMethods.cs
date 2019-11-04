using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class DisplacementMethods 
{
    /*Renvoie la liste de coordonées du chemin optimal entre "position" et "goal" */
    public static List<Vector3Int> CellPath(Vector3Int position, Vector3Int goal) {
        TilemapReferences grid = GameObject.Find("Grid - Level").GetComponent<TilemapReferences>() as TilemapReferences;
        List<Tilemap> groundTilemaps = grid.GroundTilemaps;
        GridLayout gridLayout = grid.GridLayout;
        List<Vector3Int> decorationPositions = grid.DecorationPositions;
        TurnResp turnResp = GameObject.Find("TurnResp").GetComponent<TurnResp>() as TurnResp;
        List<Vector3Int> entitiesPos = new List<Vector3Int>(turnResp.EntitiesPos);
        entitiesPos.Remove(goal);

        List<Vector3Int> ways = new List<Vector3Int>{new Vector3Int(0,1,0), new Vector3Int(0,-1,0), new Vector3Int(1,0,0), new Vector3Int(-1,0,0)};
        List<List<Vector3Int>> paths = new List<List<Vector3Int>>();
        paths.Add(new List<Vector3Int>{position});
        List<Vector3Int> surround = new List<Vector3Int>{position};
        for(int i = 0; i < 100; i++) {
            List<Vector3Int> considered = surround.Where(cell => cell.z == surround.Min(surr => surr.z)).ToList();
            foreach (Vector3Int c in considered) {
                foreach ( Vector3Int way in ways) {
                    Vector3Int nextCell = c + way - new Vector3Int(0, 0, c.z);
                    int dist = (int) Vector3Int.Distance(nextCell, goal);
                    nextCell += new Vector3Int(0, 0, dist);
                    foreach (Tilemap groundTilemap in groundTilemaps) {
                        Vector3Int actualNextCell = nextCell - new Vector3Int(0, 0, dist);
                        bool tileExist = groundTilemap.HasTile(actualNextCell);
                        bool isFree = !(decorationPositions.Contains(actualNextCell) || entitiesPos.Contains(actualNextCell));
                        bool isNew = !(surround.Contains(nextCell));
                        if (tileExist && isFree && isNew) {
                            
                            List<List<Vector3Int>> CPaths = paths.Where(path => path[path.Count -1 ] == c - new Vector3Int(0, 0, c.z)).ToList();
                            List<Vector3Int> shorterCPath = new List<Vector3Int>(CPaths.Where(path => path.Count == CPaths.Min(cpath => cpath.Count)).First());
                            shorterCPath.Add(nextCell - new Vector3Int(0, 0, nextCell.z));
                            paths.Add(shorterCPath);

                            if (nextCell - new Vector3Int(0, 0, nextCell.z) == goal) 
                                return shorterCPath.GetRange(1,shorterCPath.Count-1);

                            surround.Add(nextCell);
                            break;
                        }
                    }
                }
                surround.Remove(c);
            }
        }
        return paths[paths.Count - 1];
    }
    
    /*Detecte les joueurs sur la scène et choisi le chemin le plus court en respectant un distance nécessaire ("necDist") et le nombre donné
    de "MP" à partir de la positin "ePos" de l'ennemi.
    Si l'ennnemi se trouve à une distance du joueur inférieure à "necDist" alors il cherche à s'éloigner */
    public static List<Vector3Int> EnemyPath(Vector3Int ePos, int MP, int necDist) {
        TilemapReferences grid = GameObject.Find("Grid - Level").GetComponent<TilemapReferences>() as TilemapReferences;
        List<Tilemap> groundTilemaps = grid.GroundTilemaps;
        GridLayout gridLayout = grid.GridLayout;
        List<Vector3Int> decorationPositions = grid.DecorationPositions;
        TurnResp turnResp = GameObject.Find("TurnResp").GetComponent<TurnResp>() as TurnResp;

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        List<List<Vector3Int>> pathChoices = new List<List<Vector3Int>>();

        foreach (GameObject player in players) {
            Character chara = player.GetComponent<Character>() as Character;
            Vector3Int pPos = chara.curCellPos;
            pathChoices.Add(CellPath(ePos,pPos));
        }

        List<Vector3Int> pathChoice = pathChoices.Where(path => path.Count == pathChoices.Min(pat => pat.Count)).First();

        Vector3Int goal = pathChoice[pathChoice.Count - 1];

        if((Mathf.Abs(ePos.x - goal.x) + Mathf.Abs(ePos.y - goal.y)) < necDist) {
            GridShape circle = new GridShape(1, necDist, GridShape.Shape.Circle);
            List<Vector3Int> vectors = circle.vectors.GetRange(circle.vectors.Count - (4*necDist), 4*necDist);
            foreach (Vector3Int v in  vectors) {
                Vector3Int cell = v + goal;
                bool tileExist = groundTilemaps[0].HasTile(cell);
                bool isFree = !(decorationPositions.Contains(cell) || turnResp.EntitiesPos.Contains(cell));
                if (tileExist && isFree) {
                    pathChoice = CellPath(ePos, cell);
                    if(pathChoice.Count > MP) 
                        pathChoice = pathChoice.GetRange(0,MP);
                    return pathChoice;
                }
            }
        }

        pathChoice = pathChoice.GetRange(0, pathChoice.Count - necDist);
        if(pathChoice.Count > MP) 
            pathChoice = pathChoice.GetRange(0,MP);

        return pathChoice;
    }
}

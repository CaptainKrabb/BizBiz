using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridShape
{
    public static List<Vector3Int> ways = new List<Vector3Int>{new Vector3Int(0,1,0), new Vector3Int(0,-1,0), new Vector3Int(1,0,0), new Vector3Int(-1,0,0)};
    public enum Shape {Circle,Cross,Line,T,X};

    public List<Vector3Int> vectors = new List<Vector3Int>();

    public int rangeMin;
    public int rangeMax;
    public Shape shape;

    public GridShape() {
        this.vectors.Add(new Vector3Int());
    }

    public GridShape(int rangeMin, int rangeMax, Shape shape) {
        this.rangeMax = rangeMax;
        this.rangeMin = rangeMin;
        this.shape = shape;

        Vector3Int v1;
        Vector3Int v2;
        Vector3Int v3;
        Vector3Int v4;

        switch(shape) {
            case Shape.Circle :
                vectors.Add(new Vector3Int());
                for (int i = 0; i < rangeMax; i++) {
                    int max = vectors.Count;
                    for (int v = 0; v < max; v++) {
                        foreach (Vector3Int w in ways) {
                            vectors.Add(vectors[v] + w);
                        }
                    }
                }
                RemoveDuplicates(ref vectors);

                if (rangeMin >= 1) {
                    vectors = vectors.GetRange(1, vectors.Count - 1);
                    for (int i = 1; i < rangeMin ; i++) {
                        vectors = vectors.GetRange(i*4, vectors.Count - (i*4));
                    }
                }
                break;

            case Shape.Cross :
                for (int i = rangeMin; i <= rangeMax; i++) {
                    v1 = new Vector3Int(0,i,0);
                    v2 = new Vector3Int(0,-i,0);
                    v3 = new Vector3Int(i,0,0);
                    v4 = new Vector3Int(-i,0,0);
                    vectors.AddRange(new List<Vector3Int>{v1,v2,v3,v4});
                }
                break;
            
            case Shape.Line : 
                for (int i = 0; i <= rangeMax; i++) {
                    v1 = new Vector3Int((rangeMin - 1) * i,rangeMin * i,0);
                    v2 = new Vector3Int((rangeMin - 1) * -i,rangeMin * -i,0);
                    vectors.AddRange(new List<Vector3Int>{v1,v2});
                }
                break;
            
            case Shape.T :
                float coef = 1.5F;
                for (int i = 0; i < rangeMax; i++) {
                    switch (rangeMin) {
                        case 0 :
                            v1 = new Vector3Int(0,i,0); 
                            v2 = new Vector3Int(Mathf.RoundToInt(i/coef),rangeMax-1,0); 
                            v3 = new Vector3Int(Mathf.RoundToInt(-i/coef),rangeMax-1,0); 
                            vectors.AddRange(new List<Vector3Int>{v1,v2,v3});
                            break;
                        case 1 :
                            v1 = new Vector3Int(0,-i,0); 
                            v2 = new Vector3Int(Mathf.RoundToInt(i/coef),-rangeMax+1,0); 
                            v3 = new Vector3Int(Mathf.RoundToInt(-i/coef),-rangeMax+1,0); 
                            vectors.AddRange(new List<Vector3Int>{v1,v2,v3});
                            break;
                        case 2 :
                            v1 = new Vector3Int(i,0,0); 
                            v2 = new Vector3Int(rangeMax-1,Mathf.RoundToInt(i/coef),0); 
                            v3 = new Vector3Int(rangeMax-1,Mathf.RoundToInt(-i/coef),0); 
                            vectors.AddRange(new List<Vector3Int>{v1,v2,v3});
                            break;
                        case 3 : 
                            v1 = new Vector3Int(-i,0,0);
                            v2 = new Vector3Int(-rangeMax+1,Mathf.RoundToInt(i/coef),0); 
                            v3 = new Vector3Int(-rangeMax+1,Mathf.RoundToInt(-i/coef),0); 
                            vectors.AddRange(new List<Vector3Int>{v1,v2,v3});
                            break;
                    }
                    
                }
                break;

            case Shape.X :
                for (int i = rangeMin; i <= rangeMax; i++) {
                    v1 = new Vector3Int(i,i,0);
                    v2 = new Vector3Int(-i,i,0); 
                    v3 = new Vector3Int(i,-i,0);
                    v4 = new Vector3Int(-i,-i,0);  
                    vectors.AddRange(new List<Vector3Int>{v1,v2,v3,v4});
                }
                break;
            
        }
        RemoveDuplicates(ref vectors);
    }
    public static void RemoveDuplicates(ref List<Vector3Int> list) {
        int max = list.Count;
        int wereRemoved = 0;
        for (int i = 0; i < max-wereRemoved; i++) {
            while (list.IndexOf(list[i]) != list.LastIndexOf(list[i])) {
                list.RemoveAt(list.LastIndexOf(list[i]));
                wereRemoved++;
            }
        }
    }
}

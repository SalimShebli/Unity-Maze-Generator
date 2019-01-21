using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeJenerator : MonoBehaviour {

   //public static int[,] MazePlane = { { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,  }, 
   //                                   { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1,  },
   //                                   { 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1,  },
   //                                   { 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1,  },
   //                                   { 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 1,  },
   //                                   { 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1,  },
   //                                   { 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1,  },
   //                                   { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,  },
   //                                   { 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1,  },
   //                                   { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  },
   //                    };
    public static int[,] MazePlane;
   public static Vector3[,] MazeWorldPositions;
   public static int[,] MazeWithBoots;
   public MainController mainController;
   public int MazeSize = 11;
   public GameObject endPlane;
   public static List<Pair> EmptySells;
#region dfs&bfs
   bool[,] visited;
    int[] Xmove={2,-2,0,0};
    int[] Ymove={0,0,2,-2};
    int[,] distance;
#endregion
 //  public static List<Pair>[,] VisibleBlocks; 
    void Start() {
       
	}
    void SetMap()
    {
        MazePlane = new int[MazeSize, MazeSize];
        EmptySells = new List<Pair>();
        for (int i = 0; i < MazePlane.GetLength(0); i++)
                for (int j = 0; j < MazePlane.GetLength(1); j++)
                       MazePlane[i, j] = 1;
        visited = new bool[MazeSize, MazeSize];
        Dfs(1, 1, 1, 1, MazeSize - 2, MazeSize-2);
        

    }

    public void StartEveryThing()
    {
        MazeSize = MainController.CurrentLevelInfo.mapSize;
        print("MazeSize  " + MazeSize);
        SetMap();
        SetMazeCubesCount();
     
        MazeWorldPositions = new Vector3[MazePlane.GetLength(0), MazePlane.GetLength(1)];
        //  VisibleBlocks = new List<Pair>[MazePlane.GetLength(0), MazePlane.GetLength(1)];
        SetPlaneToGround();
        MazeWithBoots = MazePlane;
        SetWorldPostion();
        SetEndOfMap();
        mainController.GenerateAllBoots();
    }
    // Update is called once per frame
    void Update()
    {
		
	}
    void SetWorldPostion()
    {
        for (int i = 0; i < MazePlane.GetLength(0) && i < gameObject.transform.childCount; i++)
        {
            for (int j = 0; j < MazePlane.GetLength(1) && j < gameObject.transform.GetChild(i).childCount; j++)
            {
                //SetVisibleMapsFromCurrentBlock(i, j);
                gameObject.transform.GetChild(i).GetChild(j).GetComponent<MapCubeController>().SetMapPos(i, j);
                MazeWorldPositions[i, j] = gameObject.transform.GetChild(i).GetChild(j).position;
                MazeWorldPositions[i, j].y=6f;
            }
        }
    }
    void SetPlaneToGround()
    {
        for (int i = 0; i < MazePlane.GetLength(0) && i<gameObject.transform.childCount; i++)
        {
            for (int j = 0; j < MazePlane.GetLength(1) && j < gameObject.transform.GetChild(i).childCount; j++)
            {
                if(MazePlane[i,j]==0)
                {
                    gameObject.transform.GetChild(i).GetChild(j).gameObject.GetComponent<MeshRenderer>().enabled=false;
                    //for (int k = 0; k < gameObject.transform.GetChild(i).GetChild(j).childCount;k++ )
                    //{
                    //    gameObject.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                    //}
                        gameObject.transform.GetChild(i).GetChild(j).gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
            }
        }
    }
    public static Vector3 GetWorldPositionFromMatrixPosition(int x,int y)
    {
        return MazeWorldPositions[x, y];
    }

    
    void Dfs(int i,int j,int stx,int sty,int enx,int eny)
    {
       
        print("dfs " + i + "  " + j);
        visited[i, j] = true;
        MazePlane[i, j] = 0;
        EmptySells.Add(new Pair(i, j));
        for (int k = 0; k < 10; k++)
        {
            
            int rand = Random.Range(0, 4);
           
            rand %= 4;
            int nextI = i + Xmove[rand];
            int nextJ = j + Ymove[rand];
            int walI = i + (Xmove[rand] / 2);
            int walJ = j + (Ymove[rand] / 2);
            if (nextI >= stx && nextJ >= sty && nextI <= enx && nextJ <= eny && !visited[nextI, nextJ])
            {
                MazePlane[walI, walJ] = 0;
                Dfs(nextI, nextJ, stx, sty, enx, eny);
               // break;
            }
        }
    }
    public void SetMazeCubesCount()
    {
        GameObject Cube = transform.GetChild(0).GetChild(0).gameObject;
        
        for(int i=1;i<MazeSize;i++)
        {
            GameObject newCube = Instantiate(Cube, transform.GetChild(0));
            Vector3 newPosition = Cube.transform.localPosition;
            newPosition.z += 1;
            newCube.transform.localPosition = newPosition;
            Cube = newCube;
        }
        GameObject Line = transform.GetChild(0).gameObject;
        for(int i=1;i<MazeSize;i++)
        {
            GameObject newLine = Instantiate(Line, transform);
            Vector3 newPosition = Line.transform.localPosition;
            newPosition.x += 1;
            newLine.transform.localPosition = newPosition;
            Line = newLine;
        }
        //Vector3 planePos = transform.GetChild(transform.childCount / 2).
        //    GetChild((transform.GetChild(transform.childCount / 2).childCount)/2).position;
        //Plane.transform.position = new Vector3(planePos.x, Plane.transform.position.y, planePos.x);
        //Plane.transform.localScale = (MazeSize * Plane.transform.localScale);
    }
    public void SetEndOfMap()
    {
        visited = new bool[MazeSize, MazeSize];
        distance = new int[MazeSize, MazeSize];
        Pair end = Bfs();
        endPlane.transform.position = GetWorldPositionFromMatrixPosition(end.first, end.second);
        endPlane.GetComponent<EndLevelPanelController>().mainController = mainController;
    }
    public Pair Bfs()
    {
        Queue<Pair> q=new Queue<Pair>();
        q.Enqueue(new Pair(1, 1));
        Pair sol = new Pair(1, 1);
        while(q.Count!=0)
        {
            Pair t = q.Dequeue();
            sol.first = t.first;
            sol.second = t.second;
            print("bfs  " + t.first + "  " + t.second + "  " + distance[t.first, t.second]);
            for(int i=0;i<4;i++)
            {
                int nextX = t.first + (Xmove[i]/2);
                int nextY = t.second + (Ymove[i]/2);
                if(nextX>=0&&nextX<MazeSize&&nextY>=0&&nextY<MazeSize&&MazePlane[nextX,nextY]==0&&!visited[nextX,nextY])
                {
                   
                    visited[nextX, nextY] = true;
                    distance[nextX, nextY] = distance[t.first,t.second]+1;
                    q.Enqueue(new Pair(nextX, nextY));
                }
            }
        }
        return sol;
    }
    //public void SetVisibleMapsFromCurrentBlock(int x,int y)
    //{
    //    VisibleBlocks[x,y] = new List<Pair>();
    //    if (MazePlane[x, y] != 0) return;
    //    for(int i=x; i>=0 && i>x-5 ; i--)
    //    {
    //        if (MazePlane[i, y] == 1) break;
    //        VisibleBlocks[x, y].Add(new Pair(i, y));
    //    }
    //    for(int i=x+1;i<MazePlane.GetLength(0)&&i<x+5;i++)
    //    {
    //        if (MazePlane[i, y] == 1) break;
    //        VisibleBlocks[x, y].Add(new Pair(i, y));
    //    }
    //    for (int i = y-1; i >= 0 && i > y - 5; i--)
    //    {
    //        if (MazePlane[x, i] == 1) break;
    //        VisibleBlocks[x, y].Add(new Pair(x, i));
    //    }
    //    for (int i = y + 1; i < MazePlane.GetLength(1) && i < y + 5; i++)
    //    {
    //        if (MazePlane[x, i] == 1) break;
    //        VisibleBlocks[x, y].Add(new Pair(x, i));
    //    }
    //}
}
public class Pair
{
    public int first;
    public int second;
    public Pair(int f,int s)
    {
        first = f;
        second = s;

    }
}

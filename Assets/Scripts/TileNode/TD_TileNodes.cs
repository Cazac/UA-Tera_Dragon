using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

///////////////
/// <summary>
///     
/// TD_TileNodes is used to spawn and keep track of all nodes attached to the tilemap sprites
/// 
/// </summary>
///////////////

public class TD_TileNodes : MonoBehaviour
{
    [Header("Main Grid / Tilemap")]
    public Grid gridBase;
    public Tilemap uniqueTilemap;

    [Header("Monster Prefabs")]
    public GameObject enemyPrefab;

    [Header("Tile Node Prefabs")]
    public GameObject[] TileNodes;

    //  TO DO   // - This is not the best?
    [Header("Tile Sprites For Layers")]
    public Tile[] WalkableTiles;
    public Tile[] UnwalkableTiles;
    public Tile[] SpawnTiles;

    //  TO DO   // - Legacy?
    [Header("Selected Nodes")]
    [SerializeField]
    private List<GameObject> selectedNode = new List<GameObject>();
    public List<GameObject> SelectedNode { get => selectedNode; set => selectedNode = value; }

    //Sorted 2D array of nodes
    public GameObject[,] nodes;

    //List of nodes before they are sorted
    private List<GameObject> unsortedNodes;
    public List<WorldTile> permanentSpawnPoints;

    //Auto set to size of the tilemap tiles
    private float mapConstant;

    public PathsData pathData;

    float timer = 0;
    bool testBool = false;

    //////////////////////////////////////////////////////////

    private void Awake()
    {
        unsortedNodes = new List<GameObject>();
        mapConstant = gridBase.cellSize.x;

        generateNodes();
        pathData = PathFinding.GetPaths(nodes, permanentSpawnPoints);
    }

    private void Start()
    {

        //Start it all
        Debug.Log("permanentSpawnPoints: " + permanentSpawnPoints.Count);

        Debug.Log("Paths numbers: " + pathData.paths.Count);
        Debug.Log("Nodes length:" + nodes.GetLength(0) + " " + nodes.GetLength(1));

        ///// for testing //////
        //foreach (WorldTile wt in pathData.PathsByStart.Keys)
        //{
        //    GameObject go = Instantiate(enemyPrefab, wt.transform.position, new Quaternion());
        //    EnemyScript enemy = go.GetComponent<EnemyScript>();
        //    enemy.waypoints = pathData.PathsByStart[wt][0];
        //}
        /////////////////////////////////////////////////
    }

    private void Update()
    {
        ///////////  for testing  //////////////////////
        //timer += Time.deltaTime;
        //if (timer > 1f && !testBool)
        //{
        //    foreach (WorldTile wt2 in pathData.PathsByEnd.Keys)
        //    {

        //        GameObject go = Instantiate(enemyPrefab, pathData.PathsByEnd[wt2][0][0].transform.position, new Quaternion());
        //        EnemyScript enemy = go.GetComponent<EnemyScript>();
        //        enemy.waypoints = pathData.PathsByEnd[wt2][0];

        //    }
        //    testBool = true;
        //}
        /////////////////////////////////////////////////
    }

    ///////////////
    /// <summary>
    /// Creates the nodes, places them in a table and sets each nodes neighbours
    /// </summary>
    ///////////////
    private void generateNodes()
    {
        uniqueTilemap.CompressBounds();
        BoundsInt bounds = uniqueTilemap.cellBounds;
        int tableX = uniqueTilemap.cellBounds.size.x;
        int tableY = uniqueTilemap.cellBounds.size.y;
        
        nodes = new GameObject[tableX, tableY];

        LoopThroughTileset();
        FillNodeTable();
        SetNeigbours();
    }


    ///////////////
    /// <summary>
    /// Scans tileset for tiles and places the corresponding tile node when it enconters one.
    /// </summary>
    ///////////////
    private void LoopThroughTileset()
    {
        WorldTile wt; GameObject node;
        GameObject[] parentNodes = new GameObject[TileNodes.Length];
        parentNodes[0] = new GameObject("Parent_WalkableTiles");
        parentNodes[1] = new GameObject("Parent_UnwalkableTiles");
        
        int GridX = 0; int GirdY = 0;
        for (int x = -(nodes.GetLength(0)) - 1; x < nodes.GetLength(0) + 1; x++)
        {
            for (int y = -(nodes.GetLength(1)) - 1; y < nodes.GetLength(1) + 1; y++)
            {
                TileBase tb = uniqueTilemap.GetTile(new Vector3Int(x, y, 0)); //check if we have a floor tile at that world coords

                if (tb != null)
                {
                    Vector3 nodePosition = new Vector3(mapConstant / 2 + ((x + gridBase.transform.position.x) * mapConstant), ((y + 0.5f + gridBase.transform.position.y) * mapConstant), 0);


                    node = null;
                    string name = uniqueTilemap.GetTile(uniqueTilemap.WorldToCell(nodePosition)).name;

                    foreach (Tile tile in WalkableTiles)
                    {
                        if (name == tile.name)
                        {
                            node = Instantiate(TileNodes[0], nodePosition, Quaternion.identity, parentNodes[0].transform);
                            foreach(Tile spTile in SpawnTiles)
                            {
                                if (name == spTile.name)
                                {
                                    permanentSpawnPoints.Add(node.GetComponent<WorldTile>());
                                }
                            }
                        }
                    }
                    foreach (Tile tile in UnwalkableTiles)
                    {
                        if (name == tile.name)
                        {
                            node = Instantiate(TileNodes[1], nodePosition, Quaternion.identity, parentNodes[1].transform);
                        }
                    }

                    if (node == null)
                    {
                        Debug.LogError(name + " is not registered.");
                    }
                    else
                    {
                        unsortedNodes.Add(node);
                        wt = node.GetComponent<WorldTile>();
                        wt.gridX = GridX;
                        wt.gridY = GirdY;
                    }
                    
                }
                GirdY++;
            }
            GirdY = 0; ;
            GridX++;
        }
    }


    ///////////////
    /// <summary>
    /// Checks tilemap for size of node array then places existing nodes in their corresponding place in the table.
    /// </summary>
    ///////////////
    private void FillNodeTable()
    {
        int minX = nodes.GetLength(0);
        int minY = nodes.GetLength(1);
        WorldTile wt;

        // makes sure grid is correctly alligned by finding
        // the lowest value for x and y and making sure it is zero
        foreach (GameObject g in unsortedNodes)
        {
            wt = g.GetComponent<WorldTile>();
            if (wt.gridX < minX)
                minX = wt.gridX;
            if (wt.gridY < minY)
                minY = wt.gridY;
        }
        foreach (GameObject g in unsortedNodes)
        {
            wt = g.GetComponent<WorldTile>();
            wt.gridX -= minX;
            wt.gridY -= minY;
            wt.name = "NODE " + wt.gridX.ToString() + " : " + wt.gridY.ToString();
            nodes[wt.gridX, wt.gridY] = g;
        }

        unsortedNodes.Clear();
    }


    ///////////////
    /// <summary>
    /// For each tile in nodes[] checks the 4 surrounding tiles for neighbours
    /// </summary>
    //////////////////
    private void SetNeigbours()
    {
        WorldTile wt;
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[x, y] != null)
                {
                    wt = nodes[x, y].GetComponent<WorldTile>();
                    wt.myNeighbours = SetNeigbour(x, y, nodes.GetLength(0), nodes.GetLength(1), wt.walkable);
                }
            }
        }
    }


    ///////////////
    /// <summary>
    /// Grab 4 surrounding tiles from all tilemaps and checks if they are neigbours
    /// </summary>
    ///////////////
    private List<WorldTile> SetNeigbour(int x, int y, int width, int height, bool walkable)
    {
        List<WorldTile> myNeighbours = new List<WorldTile>();
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return myNeighbours;
        }

        if (x > 0)
        {
            AddNodeToList(myNeighbours, x - 1, y, walkable);
        }
        if (x < width - 1)
        {
            AddNodeToList(myNeighbours, x + 1, y, walkable);
        }
        if (y > 0)
        {
            AddNodeToList(myNeighbours, x, y - 1, walkable);
        }
        if (y < height - 1)
        {
            AddNodeToList(myNeighbours, x, y + 1, walkable);
        }

        return myNeighbours;
    }


    ///////////////
    /// <summary>
    /// Error check each node and walkable status before adding to the WorldTile neighbours list
    /// </summary>
    ///////////////
    private void AddNodeToList(List<WorldTile> list, int x, int y, bool currentWalkableState)
    {
        if (nodes[x, y] != null)
        {
            WorldTile wt = nodes[x, y].GetComponent<WorldTile>();
            if (wt != null && wt.walkable == currentWalkableState)
            {
                list.Add(wt);
            }
        }
    }

    
}

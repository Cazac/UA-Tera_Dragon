using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

///////////////
/// <summary>
///     
/// TD_TileNodes is used to spawn and keep track of all nodes attached to the tilemap sprites
/// 
/// </summary>
///////////////

public class TileNodes : MonoBehaviour
{
    [Header("Main Grid / Tilemap")]
    public Grid gridBase;
    public Tilemap uniqueTilemap;

    [Header("Monster Prefabs")]
    public GameObject enemyPrefab;

    [Header("Tile Node Prefabs")]
    public GameObject[] TileNodesPrefabs;

    //  TO DO   // - This is not the best?
    [Header("Tile Sprites For Layers")]
    public Tile[] WalkableTiles;
    public Tile[] UnwalkableTiles;
    public Tile[] SpawnTiles;

    //  TO DO   // - Used for 
    [Header("Selected Nodes")]
    [SerializeField]
    private List<GameObject> selectedNode = new List<GameObject>();
    public List<GameObject> SelectedNode { get => selectedNode; set => selectedNode = value; }

    // Sorted 2D array of nodes
    public GameObject[,] nodes;

    // List of nodes before they are sorted
    private List<GameObject> unsortedNodes;
    private List<WorldTile> permanentSpawnPoints;

    // Auto set to size of the tilemap tiles
    private float mapConstant;

    [SerializeField] // necessary to have the nodes saved in and out of play
    GameObject[] parentNodes = new GameObject[0];

    [Header("Editor variables")]
    // variables used for gizmo draw and 
    public int SelectedPath = 0;
    public List<WorldTile> selectedList;
    public PathsData pathData;

    //////////////////////////////////////////////////////////

    private void Awake() { BuildTable(); }
    private void Start() { }
    private void Update() { }

    public void BuildTable()
    {
        //   listWapper = new ListWapper();
        permanentSpawnPoints = new List<WorldTile>();
        //  
        for (int i = 0; i < parentNodes.Length; i++)
        {
            if (parentNodes[i] != null)
            {
                DestroyImmediate(parentNodes[i]);
            }
        }

        unsortedNodes = new List<GameObject>();
        mapConstant = gridBase.cellSize.x;

        generateNodes();
        pathData = PathFinding.GetPaths(nodes, permanentSpawnPoints, maxGridX);

    }


    ///////////////
    /// <summary>
    /// Allows you to see the list that is currently selected
    /// </summary>
    ///////////////
    public void Editor_SelectList()
    {
        if (pathData != null && pathData.paths != null)
        {
            if (SelectedPath >= 0 && SelectedPath < pathData.paths.Count)
            {
                selectedList = pathData.paths[SelectedPath];
            }
        }
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

        // create nodes
        LoopThroughTileset();
        // places these nodes in a table
        FillNodeTable();
        // give each node their neighbours
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
        parentNodes = new GameObject[TileNodesPrefabs.Length];

        parentNodes[0] = new GameObject("Parent_WalkableTiles");
        parentNodes[0].transform.SetParent(transform);

        parentNodes[1] = new GameObject("Parent_UnwalkableTiles");
        parentNodes[1].transform.SetParent(transform);

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

                    if (name == "Temp Base Tile")
                    {
                        node = Instantiate(TileNodesPrefabs[2], nodePosition, Quaternion.identity, parentNodes[0].transform);

                    }
                    else
                    {
                        // checks if tile is found in walkable
                        foreach (Tile tile in WalkableTiles)
                        {
                            if (name == tile.name)
                            {
                                node = Instantiate(TileNodesPrefabs[0], nodePosition, Quaternion.identity, parentNodes[0].transform);
                            }
                        }// checks if walkable tile is a spawning tile
                        foreach (Tile spTile in SpawnTiles)
                        {
                            if (name == spTile.name)
                            {
                                node = Instantiate(TileNodesPrefabs[0], nodePosition, Quaternion.identity, parentNodes[0].transform);
                                permanentSpawnPoints.Add(node.GetComponent<WorldTile>());
                            }
                        }
                        // checks if tile is found in unwalkable
                        foreach (Tile tile in UnwalkableTiles)
                        {
                            if (name == tile.name)
                            {
                                node = Instantiate(TileNodesPrefabs[1], nodePosition, Quaternion.identity, parentNodes[1].transform);
                            }
                        }

                    }

                    if (node == null)
                    {
                        //    Debug.LogError(name + " is not registered.");
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

        // temp variable

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
            if (wt.gridX > maxGridX)
                maxGridX = wt.gridX;
        }
        Debug.Log("Max Grid:" + maxGridX);
        unsortedNodes.Clear();
    }
    int maxGridX = 0;


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



    private void OnDrawGizmos()
    {
        // draws the lines that represent the path currently selected
        Gizmos.color = Color.blue;
        if (pathData != null && pathData.paths != null)
        {
            if (SelectedPath >= 0 && SelectedPath < pathData.paths.Count)
            {
                for (int i = 0; i < pathData.paths[SelectedPath].Count - 1; i++)
                {
                    Gizmos.DrawLine(pathData.paths[SelectedPath][i].transform.position, pathData.paths[SelectedPath][i + 1].transform.position);
                }
            }
            if (SelectedPath < 0)
                SelectedPath = 0;
            else if (SelectedPath >= (pathData.paths.Count))
                SelectedPath = pathData.paths.Count - 1;
        }
    }


}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class spawntiles : MonoBehaviour
{
    public int gridSize = 10;
    public int roadNum = 5;
    public Grid[,] gridMap;
    public GameObject diagonalObject;
    public GameObject halfcurveRightObject;
    public GameObject halfcurveLeftObject;
    public GameObject straightObject;
    public GameObject fullcurveObject;
    public GameObject starterObject;
    public GameObject pavementObject;
    public GameObject extraedgeObject;
    public GameObject centreObject;
    public GameObject straightConnectorObject;
    public GameObject halfcurveRightConnectorObject;
    public GameObject halfcurveLeftConnectorObject;
    public GameObject fullcurveConnectorObject;
    public GameObject startConnectorLeftObject;
    public GameObject startConnectorRightObject;
    // Start is called before the first frame update
    void Start()
    {
        createGrid(gridSize);

    }

    // Update is called once per frame
    void Update()
    {


    }

    void createGrid(int gridSize)
    {
        gridMap = new Grid[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                gridMap[x, y] = new Grid();
            }
        }
        createCentre(gridSize);
        spawnroads(roadNum);
        fillInBlanks();
    }

    void createCentre(int gridSize)
    {
        int centreStartX;
        int centreStartY;
        int centreSize = 5;

        centreStartX = Random.Range(gridSize / 4, gridSize - gridSize / 4);
        centreStartY = Random.Range(gridSize / 4, gridSize - gridSize / 4);

        for (int x = centreStartX - centreSize; x <= centreStartX + centreSize; x++)
        {
            for (int y = centreStartY - centreSize; y <= centreStartY + centreSize; y++)
            {
                if (x == centreStartX - centreSize && y == centreStartY - centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.fullcurve, 1);
                }
                else if (x == centreStartX + centreSize && y == centreStartY + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.fullcurve, 3);
                }
                else if (x == centreStartX - centreSize && y == centreStartY + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.fullcurve, 2);
                }
                else if (x == centreStartX + centreSize && y == centreStartY - centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.fullcurve, 0);
                }
                else if ((x == centreStartX - centreSize || x == centreStartX + centreSize) && y != centreStartY - centreSize && y != centreStartY + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straight, 0);
                }
                else if ((y == centreStartY - centreSize || y == centreStartY + centreSize) && x != centreStartX - centreSize && x != centreStartX + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straight, 1);
                }
                else
                {
                    gridMap[x, y].tile = new Tile(piece.centre, 0);
                }
            }
        }
    }

    // Fischer Yates Shuffle
    /*static int[] generateRandomArray(int roadnum)
    {
        int[] array = new int[roadnum];

        for (int b = 0; b < roadnum; b++)
        {
            array[b] = b;
        }

        System.Random random = new System.Random();
        for (int b = roadnum - 1; b >= 0; b--)
        {
            int j = random.Next(0, b + 1);
            int temp = array[b];
            array[b] = array[j];
            array[j] = temp;
        }

        return array;

    }*/

    void spawnroads(int roadNumber)
    {
        int startingpointX = 0;
        int startingpointY = 0;


        for (int a = 0; a < roadNumber; a++)
        {
            bool started = false;
            int spawnPos = a;

            if (spawnPos > 3)
            {
                spawnPos -= 4;
            }

            if (spawnPos == 2)
            {
                while (started == false)
                {
                    startingpointX = Random.Range(0, gridSize);
                    startingpointY = gridSize - 1;
                    if (gridMap[startingpointX, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 2);
                        gridMap[startingpointX, startingpointY].filled = true;
                        started = true;
                    }

                }

            }

            else if (spawnPos == 3)
            {
                while (started == false)
                {
                    startingpointX = gridSize - 1;
                    startingpointY = Random.Range(0, gridSize);
                    if (gridMap[startingpointX, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 3);
                        gridMap[startingpointX, startingpointY].filled = true;
                        started = true;
                    }

                }
            }

            else if (spawnPos == 0)
            {
                while (started == false)
                {
                    startingpointX = Random.Range(0, gridSize);
                    startingpointY = 0;
                    if (gridMap[startingpointX, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 0);
                        gridMap[startingpointX, startingpointY].filled = true;
                        started = true;
                    }

                }
            }

            else if  (spawnPos == 1)
            {
                while (started == false)
                {
                    startingpointX = 0;
                    startingpointY = Random.Range(0, gridSize);
                    if (gridMap[startingpointX, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 1);
                        gridMap[startingpointX, startingpointY].filled = true;
                        started = true;
                    }

                }
            }
            createPath(startingpointX, startingpointY);
        }


    }

    void createPath(int startX, int startY)
    {
        int PosX = startX;
        int PosY = startY;
        int edgePlaceX;
        int edgePlaceY;
        piece[] nextPieces;
        piece[] nextDiagPieces;
        Grid currentPiece = gridMap[PosX, PosY];
        piece nextpiece;
        int curveCounter = 0;

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            curveCounter += 1;
            if (curveCounter % 3 != 0 || curveCounter < 7)
            {
                nextDiagPieces = new piece[] { piece.diagonal };
                nextPieces = new piece[] { piece.straight };
            }
            else
            {
                nextPieces = new piece[] { piece.straight, piece.halfcurveright, piece.halfcurveleft, piece.fullcurve, piece.straight };
                nextDiagPieces = new piece[] { piece.diagonal, piece.halfcurveright, piece.halfcurveleft };
            }


            if (currentPiece.tile.north == connectorType.straight)
            {
                PosY++;
                if (PosY < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", connectorType.straight, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.south = connectorType.no;
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        currentPiece = gridMap[PosX, PosY];
                        setConnectorType("South", currentPiece.tile, PosX, PosY);
                        currentPiece.tile.south = connectorType.no;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.north == connectorType.right)
            {
                PosX++;
                PosY++;
                if (PosX < gridSize && PosY < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", connectorType.right, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.south = connectorType.no;
                        currentPiece.tile.west = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                        }
                        catch (System.Exception)
                        {

                        }
                       
                        edgePlaceX = PosX - 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX--;
                        PosY--;

                        if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                              
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 2);
                               
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 2);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                    }
                }
                else
                {
                    PosX--;
                    PosY--;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType()== piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 2);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = connectorType.no;
                    currentPiece.tile.west = connectorType.no;
                   
                }

            }
            else if (currentPiece.tile.north == connectorType.left)
            {
                PosX--;
                PosY++;
                if (PosX >= 0 && PosY < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", connectorType.left, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.south = connectorType.no;
                        currentPiece.tile.east = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX + 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX++;
                        PosY--;

                        if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 2);
                              
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 1);
                               
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                                
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                              
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                                
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                               
                            }
                        }
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                    }
                }
                else
                {
                    PosX++;
                    PosY--;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 2);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    currentPiece.tile.south = connectorType.no;

                }
            }
            else if (currentPiece.tile.south == connectorType.straight)
            {
                PosY--;
                if (PosY >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", connectorType.straight, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.north = connectorType.no;
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        currentPiece = gridMap[PosX, PosY];
                        setConnectorType("North", currentPiece.tile, PosX, PosY);
                        currentPiece.tile.north = connectorType.no;
                        break;
                    }
                }
                else
                {
                    break;
                }


            }
            else if (currentPiece.tile.south == connectorType.right)
            {
                PosX--;
                PosY--;
                if (PosX >= 0 && PosY >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", connectorType.right, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.north = connectorType.no;
                        currentPiece.tile.east = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX + 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX++;
                        PosY++;
                   

                        if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 0);
                            }

                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 2);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                    }



                }
                else
                {
                    PosX++;
                    PosY++;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 1);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 0);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    currentPiece.tile.north = connectorType.no;

                }
            }
            else if (currentPiece.tile.south == connectorType.left)
            {
                PosX++;
                PosY--;
                if (PosX < gridSize && PosY >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", connectorType.left, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.north = connectorType.no;
                        currentPiece.tile.west = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX - 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                        }
                        catch (System.Exception) { }
                    }

                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX--;
                        PosY++;
                        if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 3);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        currentPiece.tile.north = connectorType.no;
                        currentPiece.tile.west = connectorType.no;
                    }


                }
                else
                {
                    PosX--;
                    PosY++;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 3);
                    }
                    else if (   gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = connectorType.no;
                    currentPiece.tile.west = connectorType.no;
                }
            }
            else if (currentPiece.tile.east == connectorType.straight)
            {
                PosX++;
                if (PosX < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.straight, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        currentPiece = gridMap[PosX, PosY];
                        setConnectorType("West", currentPiece.tile, PosX, PosY);
                        currentPiece.tile.west = connectorType.no;
                        break;

                    }

                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == connectorType.right)
            {
                PosX++;
                PosY--;
                if (PosX < gridSize && PosY >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.right, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                        }
                        catch (System.Exception) { }
                    
                        edgePlaceX = PosX - 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX--;
                        PosY++;
                        if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 3);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                    }
                }
                else
                {

                    PosX--;
                    PosY++;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                    }
                    else if (   gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                    }
                     currentPiece.tile.north = connectorType.no;
                     currentPiece.tile.west = connectorType.no;
                }

            }
            else if (currentPiece.tile.east == connectorType.left)
            {
                PosX++;
                PosY++;
                if (PosX < gridSize && PosY < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.left, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                        }
                        catch (System.Exception) { }
                        
                        edgePlaceX = PosX - 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX--;
                        PosY--;
                        if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 1);
                            }
                        }
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                    }
                }
                else
                {
                    PosX--;
                    PosY--;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 2);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = connectorType.no;
                    currentPiece.tile.south = connectorType.no;
                }
            }
            else if (currentPiece.tile.west == connectorType.straight)
            {
                PosX--;
                if (PosX >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.straight, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.east = connectorType.no;
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        currentPiece = gridMap[PosX, PosY];
                        setConnectorType("East", currentPiece.tile, PosX, PosY);
                        currentPiece.tile.east = connectorType.no;
                        break;

                    }
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.west == connectorType.right)
            {
                PosX--;
                PosY++;
                if (PosX >= 0 && PosY < gridSize)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.right, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                        }
                        catch (System.Exception) { }
                    
                        edgePlaceX = PosX + 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                        }
                        catch (System.Exception) { }
                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX++;
                        PosY--;
                        if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 2);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 1);
                            }
                        }
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.south = connectorType.no;

                    }


                }
                else
                {
                    PosX++;
                    PosY--;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 3);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    currentPiece.tile.south = connectorType.no;
                }
            }
            else if (currentPiece.tile.west == connectorType.left)
            {
                PosX--;
                PosY--;
                if (PosX >= 0 && PosY >= 0)
                {
                    if (gridMap[PosX, PosY].tile == null)
                    {
                        nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                        gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.left, nextpiece));
                        gridMap[PosX, PosY].filled = true;
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX + 1;
                        try
                        {
                            gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                        }
                        catch (System.Exception) { }

                    }
                    else if (gridMap[PosX, PosY].tile != null)
                    {
                        PosX++;
                        PosY++;
                        if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 0);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 0);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveleft)
                        {

                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 2);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                            }
                        }
                        else if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 1);
                            }
                            else
                            {
                                gridMap[PosX, PosY].tile = new Tile(piece.halfcurveright, 0);
                            }
                        }
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                    }


                }
                else
                {

                    PosX++;
                    PosY++;
                    if (gridMap[PosX, PosY].tile.getTileType() == piece.diagonal)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.halfcurveleft, 3);
                    }
                    else if (gridMap[PosX, PosY].tile.getTileType() == piece.halfcurveright)
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.fullcurve, 0);
                    }
                    else
                    {
                        gridMap[PosX, PosY].tile = new Tile(piece.straight, 1);
                    }
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    currentPiece.tile.north = connectorType.no;
                }
            

            }
        }
    }
    void setConnectorType(string sideToAlign, Tile collider, int posX, int posY)
    {
        Tile toSlot = collider;
        int rotation = toSlot.getRotateAmount();

        if (toSlot.getTileType() == piece.starter)
        {
            if (rotation == 0)
            {
                if (sideToAlign == "East")
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 0);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinleft, 0);
                }
            }
            else if (rotation == 90)
            {
                if (sideToAlign == "South")
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 1);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinleft, 1);
                }
            }
            else if (rotation == 180)
            {
                if (sideToAlign == "West")
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 2);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinleft, 2);
                }
            }
            else
            {
                if (sideToAlign == "North")
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 3);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinleft, 3);
                }
            }
        }

        else if (toSlot.getTileType() == piece.straight)
        {
            if (sideToAlign == "North")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 2);
            }
            else if (sideToAlign == "South")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 0);
            }
            else if (sideToAlign == "East")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 3);
            }
            else if (sideToAlign == "West")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 1);
            }
        }

        else if (toSlot.getTileType() == piece.fullcurve)
        {
            if (sideToAlign == "North")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 1);
            }
            else if (sideToAlign == "South")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 3);
            }
            else if (sideToAlign == "East")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 2);
            }
            else if (sideToAlign == "West")
            {
                gridMap[posX, posY].tile = new Tile(piece.straightjoin, 0);
            }
        }

        else if (toSlot.getTileType() == piece.halfcurveleft)
        {
            gridMap[posX, posY].tile = new Tile(piece.halfcurveleftjoin, (toSlot.getRotateAmount() / 90));
        }

        else if (toSlot.getTileType() == piece.halfcurveright)
        {
            gridMap[posX, posY].tile = new Tile(piece.halfcurverightjoin, (toSlot.getRotateAmount()/ 90));
        }

        else if (toSlot.getTileType() == piece.extraedge)
        {
            if (rotation == 0)
            {
                if (sideToAlign == "West")
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveright, 1);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveleft, 2);
                }
            }
            else if(rotation == 90)
            {
                if (sideToAlign == "East")
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveleft, 3);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveright, 2);
                }
            }
            else if (rotation == 180)
            {
                if (sideToAlign == "South")
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveleft, 0);
                }
                else 
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveright, 3);
                }
               
            }
            else
            {
                if (sideToAlign == "West")
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveleft, 1);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.halfcurveright, 0);
                }
            }
        }
    }


    int matchRotation(string direction, connectorType faceType, piece tile)
    {
        connectorType north = connectorType.no;
        connectorType south = connectorType.no;
        connectorType east = connectorType.no;
        connectorType west = connectorType.no;
        int rotation = 0;


        if (tile == piece.starter)
        {
            north = connectorType.straight;
            south = connectorType.no;
            east = connectorType.no;
            west = connectorType.no;
        }

        else if (tile == piece.diagonal)
        {
            north = connectorType.right;
            south = connectorType.right;
            east = connectorType.left;
            west = connectorType.left;
        }

        else if (tile == piece.pavement)
        {
            north = connectorType.no;
            south = connectorType.no;
            east = connectorType.no;
            west = connectorType.no;
        }

        else if (tile == piece.halfcurveright)
        {
            north = connectorType.right;
            south = connectorType.straight;
            east = connectorType.left;
            west = connectorType.no;
        }
        else if (tile == piece.halfcurveleft)
        {
            north = connectorType.left;
            south = connectorType.straight;
            east = connectorType.no;
            west = connectorType.right;
        }
        else if (tile == piece.fullcurve)
        {
            north = connectorType.straight;
            south = connectorType.no;
            east = connectorType.no;
            west = connectorType.straight;
        }
        else if (tile == piece.straight)
        {
            north = connectorType.straight;
            south = connectorType.straight;
            east = connectorType.no;
            west = connectorType.no;
        }
        else if (tile == piece.extraedge)
        {
            north = connectorType.no;
            south = connectorType.no;
            east = connectorType.no;
            west = connectorType.no;
        }

        if (direction == "East")
        {
            while (east != faceType)
            {
                connectorType placeholder = north;
                north = west;
                west = south;
                south = east;
                east = placeholder;
                rotation++;
            }
        }
        else if (direction == "South")
        {
            while (south != faceType)
            {
                connectorType placeholder = north;
                north = west;
                west = south;
                south = east;
                east = placeholder;
                rotation++;
            }
        }
        else if (direction == "North")
        {
            while (north != faceType)
            {
                connectorType placeholder = north;
                north = west;
                west = south;
                south = east;
                east = placeholder;
                rotation++;
            }

        }
        else if (direction == "West")
        {
            while (west != faceType)
            {
                connectorType placeholder = north;
                north = west;
                west = south;
                south = east;
                east = placeholder;
                rotation++;
            }

        }
        return rotation;
    }

    void fillInBlanks()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (gridMap[x, y].tile == null)
                {
                    gridMap[x, y].tile = new Tile(piece.empty, 0);
                    gridMap[x, y].filled = true;
                }

            }
        }
        spawnTiles();
    }

    void spawnTiles()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (gridMap[x, y].tile.getTileType() == piece.diagonal)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(diagonalObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.straight)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurveright)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveRightObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurveleft)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveLeftObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.starter)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(starterObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.fullcurve)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(fullcurveObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.extraedge)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(extraedgeObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.centre)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(centreObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.straightjoin)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightConnectorObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.fullcurvejoin)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(fullcurveConnectorObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurveleftjoin)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveLeftConnectorObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurverightjoin)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveRightConnectorObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.starterjoinleft)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(startConnectorLeftObject, position, gridMap[x, y].tile.getRotation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.starterjoinright)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(startConnectorRightObject, position, gridMap[x, y].tile.getRotation());
                }
            }
        }
        spawnPavement();
    }

    void spawnPavement()
    {
        for (int x = 0; x < gridSize/4; x++)
        {
            for (int y = 0; y < gridSize/4; y++)
            {
                
                    Vector3 position = new Vector3((25.6f * (float)x) +16f, 0.1f, (25.6f * (float)y) +3.2f);
                    Instantiate(pavementObject, position, Quaternion.identity);
            }
                
        }
            
        
    }

}
    public class Grid
    {
        public Tile tile;
        public bool filled = false;

        public Grid()
        {
            tile = null;
        }
    }

    public enum connectorType
    {
        straight,
        left,
        right,
        no
    }

    public enum piece
    {
        halfcurveleft,
        halfcurveright,
        straight,
        diagonal,
        fullcurve,
        pavement,
        extraedge,
        starter,
        centre,
        starterjoinleft,
        starterjoinright,
        straightjoin,
        halfcurveleftjoin,
        halfcurverightjoin,
        diagonaljoin,
        fullcurvejoin,
        empty
    }

    public class Tile
    {
        public connectorType north = connectorType.no;
        public connectorType south = connectorType.no;
        public connectorType east = connectorType.no;
        public connectorType west = connectorType.no;
        int rotation;
        piece tileType;
        Quaternion rotater;
        int RTQ = 0;
        public Tile(piece tile, int rotateamount)
        {
            tileType = tile;
            rotation = rotateamount;

            if (tileType == piece.starter)
            {
                north = connectorType.straight;
                south = connectorType.no;
                east = connectorType.no;
                west = connectorType.no;
            }

            else if (tileType == piece.diagonal)
            {
                north = connectorType.right;
                south = connectorType.right;
                east = connectorType.left;
                west = connectorType.left;
            }

            else if (tileType == piece.pavement)
            {
                north = connectorType.no;
                south = connectorType.no;
                east = connectorType.no;
                west = connectorType.no;
            }

            else if (tileType == piece.halfcurveright)
            {
                north = connectorType.right;
                south = connectorType.straight;
                east = connectorType.left;
                west = connectorType.no;
            }
            else if (tileType == piece.halfcurveleft)
            {
                north = connectorType.left;
                south = connectorType.straight;
                east = connectorType.no;
                west = connectorType.right;
            }
            else if (tileType == piece.fullcurve)
            {
                north = connectorType.straight;
                south = connectorType.no;
                east = connectorType.no;
                west = connectorType.straight;
            }
            else if (tileType == piece.straight)
            {
                north = connectorType.straight;
                south = connectorType.straight;
                east = connectorType.no;
                west = connectorType.no;
            }
            else if (tileType == piece.extraedge)
            {
                north = connectorType.no;
                south = connectorType.no;
                east = connectorType.no;
                west = connectorType.no;
            }
            calculateinitialRotation();

        }
        public void calculateinitialRotation()
        {

            for (int i = 0; i < rotation; i++)
            {
                connectorType placeholder = north;
                north = west;
                west = south;
                south = east;
                east = placeholder;

            }
            RTQ = rotation * 90;
            rotater = Quaternion.Euler(0, RTQ, 0);
        }
        public piece getTileType()
        {
            return tileType;
        }
        public Quaternion getRotation()
        {
            return rotater;
        }

        public int getRotateAmount()
        {
            return RTQ;
        }

    }









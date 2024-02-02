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
using UnityEngine.WSA;

public class spawntiles : MonoBehaviour
{
    public int gridSize = 10;
    public int roadNum = 5;
    public Grid[,] gridMap;
    public Grid[,] extraPieces;
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
    public GameObject straightIntersectionObject;
    public GameObject straightcrossingObject;
    public GameObject BstraightObject;
    public GameObject BbendObject;
    public GameObject BfullblockObject;
    public GameObject BintersectionObject;
    public GameObject BfullemptyObject;
    public GameObject BtshapeObject;
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
        extraPieces = new Grid[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                gridMap[x, y] = new Grid();
                extraPieces[x, y] = new Grid();
            }
        }
        createCentre(gridSize);
        spawnroads();
        fillInBlanks();
        setroadEdge();
        createBuildings();
        spawnRoadTiles();
        spawnBuildingTiles();
    }

    void createCentre(int gridSize)
    {
        int centreStartX;
        int centreStartY;
        int centreSize = 5;
        int startingpointX;
        int startingpointY;
        centreStartX = Random.Range(gridSize / 4, gridSize - gridSize / 4);
        centreStartY = Random.Range(gridSize / 4, gridSize - gridSize / 4);
        int crossingPointX1 = Random.Range(centreStartX - centreSize + 1, centreStartX + centreSize - 1);
        int crossingPointX2 = Random.Range(centreStartX - centreSize + 1, centreStartX + centreSize - 1);
        int crossingPointY1 = Random.Range(centreStartY - centreSize + 1, centreStartY + centreSize - 1);
        int crossingPointY2 = Random.Range(centreStartY - centreSize + 1, centreStartY + centreSize - 1);
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
                else if(x == crossingPointX1 && y == centreStartY + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straightcrossing, 1);
                }
                else if (x == crossingPointX2 && y == centreStartY - centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straightcrossing, 1);
                }
                else if (y == crossingPointY1 && x == centreStartX - centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straightcrossing, 0);
                }
                else if (y == crossingPointY2 && x == centreStartX + centreSize)
                {
                    gridMap[x, y].tile = new Tile(piece.straightcrossing, 0);
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
        startingpointX = Random.Range(centreStartX - centreSize + 1, centreStartX + centreSize - 1);
        startingpointY = centreStartY + centreSize;
        gridMap[startingpointX, startingpointY].tile = new Tile(piece.straightjoin, 2);
        gridMap[startingpointX, startingpointY + 1].tile = new Tile(piece.straight, 0);
        gridMap[startingpointX, startingpointY + 1].tile.south = connectorType.no;
        createPath(startingpointX, startingpointY + 1);

        startingpointX = centreStartX + centreSize;
        startingpointY = Random.Range(centreStartY - centreSize + 1, centreStartY + centreSize - 1);
        gridMap[startingpointX, startingpointY].tile = new Tile(piece.straightjoin, 3);
        gridMap[startingpointX + 1, startingpointY].tile = new Tile(piece.straight, 1);
        gridMap[startingpointX + 1, startingpointY].tile.west = connectorType.no;
        createPath(startingpointX + 1, startingpointY);

        startingpointX = Random.Range(centreStartX - centreSize + 1, centreStartX + centreSize - 1);
        startingpointY = centreStartY - centreSize;
        gridMap[startingpointX, startingpointY].tile = new Tile(piece.straightjoin, 0);
        gridMap[startingpointX, startingpointY - 1].tile = new Tile(piece.straight, 0);
        gridMap[startingpointX, startingpointY - 1].tile.north = connectorType.no;
        createPath(startingpointX, startingpointY - 1);

        startingpointX = centreStartX - centreSize;
        startingpointY = Random.Range(centreStartY - centreSize + 1, centreStartY + centreSize - 1);
        gridMap[startingpointX, startingpointY].tile = new Tile(piece.straightjoin, 1);
        gridMap[startingpointX - 1, startingpointY].tile = new Tile(piece.straight, 1);
        gridMap[startingpointX - 1, startingpointY].tile.east = connectorType.no;
        createPath(startingpointX - 1, startingpointY);
    }

    void spawnroads()
    {
        int startingpointX = 0;
        int startingpointY = 0;
        int roadNumber = roadNum;
        int spawnPos = 0;
        for (int a = 0; a < roadNumber; a++)
        {
            bool started = false;
            

            if (spawnPos == 4)
            {
                spawnPos = 0;
            }

            if (spawnPos == 2)
            {
                while (started == false)
                {
                    startingpointX = Random.Range(2, gridSize - 2);
                    startingpointY = gridSize - 1;
                    if (gridMap[startingpointX, startingpointY].tile == null && gridMap[startingpointX + 1, startingpointY].tile == null
                        && gridMap[startingpointX - 1, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 2);
                        started = true;
                    }

                }

            }

            else if (spawnPos == 3)
            {
                while (started == false)
                {
                    startingpointX = gridSize - 1;
                    startingpointY = Random.Range(2, gridSize - 2);
                    if (gridMap[startingpointX, startingpointY].tile == null &&
                        gridMap[startingpointX, startingpointY + 1].tile == null && gridMap[startingpointX, startingpointY - 1].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 3);
                        started = true;
                    }

                }
            }

            else if (spawnPos == 0)
            {
                while (started == false)
                {
                    startingpointX = Random.Range(2, gridSize - 2);
                    startingpointY = 0;
                    if (gridMap[startingpointX, startingpointY].tile == null && gridMap[startingpointX + 1, startingpointY].tile == null
                        && gridMap[startingpointX - 1, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 0);
                        started = true;
                    }

                }
            }

            else if  (spawnPos == 1)
            {
                while (started == false)
                {
                    startingpointX = 0;
                    startingpointY = Random.Range(2, gridSize - 2);
                    if (gridMap[startingpointX, startingpointY].tile == null &&
                        gridMap[startingpointX, startingpointY + 1].tile == null && gridMap[startingpointX, startingpointY - 1].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter, 1);
                        started = true;
                    }
                }
            }
            createPath(startingpointX, startingpointY);
            spawnPos++;
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
        int crossingCounter = 0;
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            curveCounter += 1;
            crossingCounter += 1;
            if (curveCounter % 4 != 0 || curveCounter < 6)
            {
                nextDiagPieces = new piece[] { piece.diagonal };
                if (crossingCounter % 4 != 0)
                {
                    nextPieces = new piece[] { piece.straight};
                }
                else
                {
                    nextPieces = new piece[] { piece.straight, piece.straightcrossing, piece.straight };
                }
            }
            else
            {
                nextPieces = new piece[] { piece.straight, piece.halfcurveright, piece.halfcurveleft, piece.fullcurve};
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.south = connectorType.no;
                        currentPiece.tile.west = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                            }
                            
                        }
                        catch (System.Exception)
                        {

                        }
                       
                        edgePlaceX = PosX - 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                            }
                                
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.south = connectorType.no;
                        currentPiece.tile.east = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                            }
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.north = connectorType.no;
                        currentPiece.tile.east = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                            }
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX + 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                            }
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.north = connectorType.no;
                        currentPiece.tile.west = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                            }
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX - 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                            }
                            
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                            }
                        
                        }
                        catch (System.Exception) { }
                    
                        edgePlaceX = PosX - 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                            }
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.west = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                            }
                        }
                        catch (System.Exception) { }
                        
                        edgePlaceX = PosX - 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                            }
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.south = connectorType.no;
                        edgePlaceY = PosY - 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                            }
                            
                        }
                        catch (System.Exception) { }
                    
                        edgePlaceX = PosX + 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                            }
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
                        currentPiece = gridMap[PosX, PosY];
                        currentPiece.tile.east = connectorType.no;
                        currentPiece.tile.north = connectorType.no;
                        edgePlaceY = PosY + 1;
                        try
                        {
                            if (gridMap[PosX, edgePlaceY].tile == null)
                            {
                                gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                            }
                            else
                            {
                                extraPieces[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                            }
                        }
                        catch (System.Exception) { }
                        edgePlaceX = PosX + 1;
                        try
                        {
                            if (gridMap[edgePlaceX, PosY].tile == null)
                            {
                                gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                            }
                            else
                            {
                                extraPieces[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                            }
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
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 2);
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
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinright, 0);
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
                    gridMap[posX, posY].tile = new Tile(piece.starterjoinleft, 1);
                }
            }
        }

        else if (toSlot.getTileType() == piece.straight || toSlot.getTileType() == piece.straightcrossing)
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
                if (toSlot.getRotateAmount() == 180)
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 3);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 1);
                }
            }
            else if (sideToAlign == "South")
            {
                if (toSlot.getRotateAmount() == 0)
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 1);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 3);
                }
            }
            else if (sideToAlign == "East")
            {
                if(toSlot.getRotateAmount() == 0)
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 2);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 0);
                }
            }
            else if (sideToAlign == "West")
            {
                if(toSlot.getRotateAmount() == 90)
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 2);
                }
                else
                {
                    gridMap[posX, posY].tile = new Tile(piece.straightjoin, 0);
                }
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
        else if (tile == piece.straightcrossing)
        {
            north = connectorType.straight;
            south = connectorType.straight;
            east = connectorType.no;
            west = connectorType.no;
        }
        else if (tile == piece.straight4connect)
        {
            north = connectorType.straight;
            south = connectorType.straight;
            east = connectorType.straight;
            west = connectorType.straight;
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
                    
                }
                if (extraPieces[x, y].tile == null)
                {
                    extraPieces[x, y].tile = new Tile(piece.empty, 0);
                }

            }
        }
        
    }
    void setroadEdge()
    {
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (gridMap[x, y].tile.getTileType() == piece.empty)
                {
                    if (y == 0)
                    {
                        if (x == 0)
                        {
                            if (gridMap[x + 1, y].tile.getTileType() != piece.empty || gridMap[x, y + 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                        else if (x == gridSize - 1)
                        {
                            if (gridMap[x - 1, y].tile.getTileType() != piece.empty || gridMap[x, y + 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                        else
                        {
                            if (gridMap[x + 1, y].tile.getTileType() != piece.empty || gridMap[x - 1, y].tile.getTileType() != piece.empty
                                || gridMap[x, y + 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                    }
                    else if (y == gridSize - 1)
                    {
                        if (x == gridSize - 1)
                        {
                            if (gridMap[x - 1, y].tile.getTileType() != piece.empty || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                        else if (x == 0)
                        {
                            if (gridMap[x + 1, y].tile.getTileType() != piece.empty || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                        else
                        {
                            if (gridMap[x + 1, y].tile.getTileType() != piece.empty || gridMap[x - 1, y].tile.getTileType() != piece.empty
                                    || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                            {
                                gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                            }
                        }
                    }
                    else if (x == 0)
                    {
                        if (gridMap[x + 1, y].tile.getTileType() != piece.empty
                                    || gridMap[x, y + 1].tile.getTileType() != piece.empty || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                        {
                            gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                        }
                    }
                    else if (x == gridSize - 1)
                    {
                        if (gridMap[x - 1, y].tile.getTileType() != piece.empty
                                || gridMap[x, y + 1].tile.getTileType() != piece.empty || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                        {
                            gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                        }
                    }
                    else
                    {
                        if (gridMap[x + 1, y].tile.getTileType() != piece.empty || gridMap[x - 1, y].tile.getTileType() != piece.empty
                                || gridMap[x, y + 1].tile.getTileType() != piece.empty || gridMap[x, y - 1].tile.getTileType() != piece.empty)
                        {
                            gridMap[x, y].building = new BuildingSquare(buildingPiece.fullempty, 0);
                        }
                    }
                }
                
            }
        }
                        
    }
    void createBuildings()
    {
        buildingPiece[] nextPiece;
        buildingPiece chosenPiece;
        buildingConnectorType westConnectorType;
        buildingConnectorType northConnectorType;
        int edge;
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
            {
                
                if (gridMap[x, y].tile.getTileType() == piece.empty && gridMap[x, y].building == null)
                {
                    
                    if (y == 0 && x == 0)
                    {
                        gridMap[x, y].building = new BuildingSquare(buildingPiece.bend, 1);
                    }
                    else if (y == 0 && x == gridSize -1)
                    {
                        gridMap[x, y].building = new BuildingSquare(buildingPiece.bend, 0);
                    }
                    else if (y == gridSize - 1 && x == 0)
                    {
                        gridMap[x, y].building = new BuildingSquare(buildingPiece.bend, 2);
                    }
                    else if (y == gridSize - 1 && x == gridSize - 1)
                    {
                        gridMap[x, y].building = new BuildingSquare(buildingPiece.bend, 3);
                    }
                    else if(x == gridSize -1)
                    {
                        edge = 1;
                        if (gridMap[x - 1, y].building.east == buildingConnectorType.road)
                        {
                            westConnectorType = buildingConnectorType.road;
                            if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                            {
                                northConnectorType = buildingConnectorType.road;
                                nextPiece = new buildingPiece[] {buildingPiece.bend,buildingPiece.tshape};
                            }
                            else
                            {
                                northConnectorType = buildingConnectorType.building;
                                nextPiece = new buildingPiece[] { buildingPiece.bend};
                            }
                        }
                        else
                        {
                            westConnectorType = buildingConnectorType.building;

                            if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                            {
                                northConnectorType = buildingConnectorType.road;
                                nextPiece = new buildingPiece[] {buildingPiece.straight };
                            }
                            else
                            {
                                northConnectorType = buildingConnectorType.building;
                                nextPiece = new buildingPiece[] { buildingPiece.fullblock }  ;
                            }
                        }
                           
                        chosenPiece = nextPiece[Random.Range(0, nextPiece.Length)];
                        gridMap[x, y].building = new BuildingSquare(chosenPiece, matchBuildingDoubleRotation(westConnectorType, northConnectorType, chosenPiece,edge));
                    }
                    else if(x == 0)
                    {
                        edge = 0;
                        westConnectorType = buildingConnectorType.building;
                        if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                        {
                            northConnectorType = buildingConnectorType.road;
                            nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape,buildingPiece.straight };
                        }
                        else
                        {
                            northConnectorType = buildingConnectorType.building;
                            nextPiece = new buildingPiece[] { buildingPiece.bend,buildingPiece.fullblock };
                        }
                        chosenPiece = nextPiece[Random.Range(0,nextPiece.Length)];
                        gridMap[x, y].building = new BuildingSquare(chosenPiece, matchBuildingDoubleRotation(westConnectorType, northConnectorType, chosenPiece, edge));
                    }
                    else if(y == gridSize - 1)
                    {
                        northConnectorType = buildingConnectorType.building;
                        edge = 0;
                        if (gridMap[x - 1, y].building.east == buildingConnectorType.road)
                        {
                            westConnectorType = buildingConnectorType.road;
                            nextPiece = new buildingPiece[] {buildingPiece.bend,buildingPiece.straight,buildingPiece.straight,
                                buildingPiece.tshape};
                        }
                        else
                        {
                            westConnectorType = buildingConnectorType.building;
                            nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.fullblock};
                        }

                        chosenPiece = nextPiece[Random.Range(0, nextPiece.Length)];
                        gridMap[x, y].building = new BuildingSquare(chosenPiece, matchBuildingDoubleRotation(westConnectorType, northConnectorType, chosenPiece, edge));
                    }
                    else if (y == 0)
                    {
                        edge = 2;

                        if (gridMap[x - 1, y].building.east == buildingConnectorType.road)
                        {
                            westConnectorType = buildingConnectorType.road;
                            if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                            {
                                northConnectorType = buildingConnectorType.road;
                                nextPiece = new buildingPiece[] {buildingPiece.bend,buildingPiece.tshape};
                            }
                            else
                            {
                                northConnectorType = buildingConnectorType.building;
                                nextPiece = new buildingPiece[] {buildingPiece.straight};

                            }
                        }
                        else
                        {
                            westConnectorType = buildingConnectorType.building;
                            if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                            {
                                northConnectorType = buildingConnectorType.road;
                                nextPiece = new buildingPiece[] { buildingPiece.bend };
                            }
                            else
                            {
                                northConnectorType = buildingConnectorType.building;
                                nextPiece = new buildingPiece[] { buildingPiece.fullblock };

                            }
                        }

                        chosenPiece = nextPiece[Random.Range(0, nextPiece.Length)];
                        gridMap[x, y].building = new BuildingSquare(chosenPiece, matchBuildingDoubleRotation(westConnectorType, northConnectorType, chosenPiece, edge));

                    }
                    else
                    {
                        edge = 0;
                        {
                            
                            
                            if (gridMap[x - 1, y].building.east == buildingConnectorType.road)
                            {
                                westConnectorType = buildingConnectorType.road;
                                if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                                {
                                    northConnectorType = buildingConnectorType.road;
                                    if (Random.Range(0, 4) == 3)
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.intersect, buildingPiece.fullempty };
                                    }
                                    else
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.intersect };
                                    }


                                }
                                else
                                {
                                    northConnectorType = buildingConnectorType.building;
                                    if (Random.Range(0, 4) == 3)
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.straight, buildingPiece.fullempty };

                                    }
                                    else
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.straight };

                                    }
                                }
                            }
                            else
                            {
                                westConnectorType = buildingConnectorType.building;
                                if (gridMap[x, y + 1].building.south == buildingConnectorType.road)
                                {
                                    northConnectorType = buildingConnectorType.road;
                                    if (Random.Range(0, 4) == 3)
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.straight, buildingPiece.fullempty };

                                    }
                                    else
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.tshape, buildingPiece.straight };

                                    }
                                }
                                else
                                {
                                    northConnectorType = buildingConnectorType.building;
                                    if (Random.Range(0, 4) == 3)
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.fullempty, buildingPiece.fullblock };

                                    }
                                    else
                                    {
                                        nextPiece = new buildingPiece[] { buildingPiece.bend, buildingPiece.fullblock };
                                    }
                                }

                            }
                            chosenPiece = nextPiece[Random.Range(0, nextPiece.Length)];
                            gridMap[x, y].building = new BuildingSquare(chosenPiece, matchBuildingDoubleRotation(westConnectorType, northConnectorType, chosenPiece, edge));
                            

                            
                        }
                        
                    }
                }
                
            }
        }
    }
    int matchBuildingDoubleRotation(buildingConnectorType westFaceType, buildingConnectorType northFaceType, buildingPiece buildingTile,int position)
    {
        buildingConnectorType north = buildingConnectorType.pavement;
        buildingConnectorType south = buildingConnectorType.pavement;
        buildingConnectorType east = buildingConnectorType.pavement;
        buildingConnectorType west = buildingConnectorType.pavement;
        int rotation = 0;
        buildingPiece building = buildingTile;
        if (building == buildingPiece.tshape)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.building;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.intersect)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.road;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.fullempty)
        {
            north = buildingConnectorType.pavement;
            south = buildingConnectorType.pavement;
            east = buildingConnectorType.pavement;
            west = buildingConnectorType.pavement;
        }

        else if (building == buildingPiece.bend)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.building;
            east = buildingConnectorType.building;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.fullblock)
        {
            north = buildingConnectorType.building;
            south = buildingConnectorType.building;
            east = buildingConnectorType.building;
            west = buildingConnectorType.building;
        }

        else if (building == buildingPiece.straight)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.building;
            west = buildingConnectorType.building;
        }
        if (position == 0)
        {
            if (building == buildingPiece.fullempty)
            {
                rotation = 0;
            }
            else
            {
                while ((west != westFaceType || north != northFaceType) && rotation < 4)
                {
                    buildingConnectorType placeholder = north;
                    north = west;
                    west = south;
                    south = east;
                    east = placeholder;
                    rotation++;
                }
            }
        }
        else if(position == 1)
        {
            if (building == buildingPiece.fullempty)
            {
                rotation = 0;
            }
            else
            {
                while ((west != westFaceType || north != northFaceType || east != buildingConnectorType.building) && rotation < 4)
                {
                    buildingConnectorType placeholder = north;
                    north = west;
                    west = south;
                    south = east;
                    east = placeholder;
                    rotation++;
                }
            }
        }
        else if(position == 2)
        {
            if (building == buildingPiece.fullempty)
            {
                rotation = 0;
            }
            else
            {
                while ((west != westFaceType || north != northFaceType || south != buildingConnectorType.building) && rotation < 4)
                {
                    buildingConnectorType placeholder = north;
                    north = west;
                    west = south;
                    south = east;
                    east = placeholder;
                    rotation++;
                }
            }
        }
        
        
        return rotation;
    }
    
    void spawnRoadTiles()
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
                else if (gridMap[x, y].tile.getTileType() == piece.starterjoinright)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(startConnectorRightObject, position, gridMap[x, y].tile.getRotation());
                }
                else if (gridMap[x, y].tile.getTileType() == piece.straightcrossing)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightcrossingObject, position, gridMap[x, y].tile.getRotation());
                }
                else if (gridMap[x, y].tile.getTileType() == piece.straight4connect)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightIntersectionObject, position, gridMap[x, y].tile.getRotation());
                }
                if (extraPieces[x,y].tile.getTileType() == piece.extraedge)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(extraedgeObject, position, extraPieces[x, y].tile.getRotation());
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
                
                    Vector3 position = new Vector3((25.6f * (float)x) +16f, 0.19f, (25.6f * (float)y) +3.2f);
                    Instantiate(pavementObject, position, Quaternion.identity);
            }
        }
    }

    void spawnBuildingTiles()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (gridMap[x, y].building != null)
                {
                    if (gridMap[x, y].building.getTileType() == buildingPiece.bend)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BbendObject, position, gridMap[x, y].building.getRotation());
                    }
                    else if (gridMap[x, y].building.getTileType() == buildingPiece.fullblock)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BfullblockObject, position, gridMap[x, y].building.getRotation());
                    }
                    else if (gridMap[x, y].building.getTileType() == buildingPiece.fullempty)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BfullemptyObject, position, gridMap[x, y].building.getRotation());
                    }
                    else if (gridMap[x, y].building.getTileType() == buildingPiece.intersect)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BintersectionObject, position, gridMap[x, y].building.getRotation());
                    }
                    else if (gridMap[x, y].building.getTileType() == buildingPiece.straight)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BstraightObject, position, gridMap[x, y].building.getRotation());
                    }
                    else if (gridMap[x, y].building.getTileType() == buildingPiece.tshape)
                    {
                        Vector3 position = new Vector3(6.4f * (float)x, 1.7f, 6.4f * (float)y);
                        Instantiate(BtshapeObject, position, gridMap[x, y].building.getRotation());
                    }
                }
                
            }
        }
    }
}
public class Grid
{
    public Tile tile;
    public BuildingSquare building;
    public Grid()
    {
        tile = null;
        building = null;
    }
}

public class BuildingSquare
{
    public int PosX;
    public int PosY;
    public string Direction;
    public buildingConnectorType north = buildingConnectorType.road; 
    public buildingConnectorType south = buildingConnectorType.road;
    public buildingConnectorType east = buildingConnectorType.road;
    public buildingConnectorType west = buildingConnectorType.road;
    int rotation;
    buildingPiece building;
    Quaternion rotater;
    int RTQ = 0;
    public BuildingSquare(buildingPiece buildingfeed, int rotateamount)
    {
        building = buildingfeed;
        rotation = rotateamount;

        if (building == buildingPiece.tshape)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.building;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.intersect)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.road;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.fullempty)
        {
            north = buildingConnectorType.pavement;
            south = buildingConnectorType.pavement;
            east = buildingConnectorType.pavement;
            west = buildingConnectorType.pavement;
        }

        else if (building == buildingPiece.bend)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.building;
            east = buildingConnectorType.building;
            west = buildingConnectorType.road;
        }

        else if (building == buildingPiece.fullblock)
        {
            north = buildingConnectorType.building;
            south = buildingConnectorType.building;
            east = buildingConnectorType.building;
            west = buildingConnectorType.building;
        }

        else if (building == buildingPiece.straight)
        {
            north = buildingConnectorType.road;
            south = buildingConnectorType.road;
            east = buildingConnectorType.building;  
            west = buildingConnectorType.building;
        }
        calculateinitialRotation();
    }
    public void calculateinitialRotation()
    {

        for (int i = 0; i < rotation; i++)
        {
            buildingConnectorType placeholder = north;
            north = west;
            west = south;
            south = east;
            east = placeholder;

        }
        RTQ = rotation * 90;
        rotater = Quaternion.Euler(0, RTQ, 0);
    }
    public buildingPiece getTileType()
    {
        return building;
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


public enum buildingConnectorType
{
    building,
    road,
    pavement
}

public enum buildingPiece
{
    tshape,
    straight,
    intersect,
    bend,
    fullblock,
    fullempty
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
    straight4connect,
    straightcrossing,
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
        else if (tileType == piece.straightcrossing)
        {
            north = connectorType.straight;
            south = connectorType.straight;
            east = connectorType.no;
            west = connectorType.no;
        }
        else if (tileType == piece.straight4connect)
        {
            north = connectorType.straight;
            south = connectorType.straight;
            east = connectorType.straight;
            west = connectorType.straight;
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









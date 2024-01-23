using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
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
       
    }

    void createCentre(int gridSize)
    {
        int centreStartX;
        int centreStartY;
        int centreSize = 7;
      
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
    static int[] generateRandomArray(int roadnum)
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

    }

    void spawnroads(int roadNumber)
    {
        int startingpointX = 0;
        int startingpointY = 0;
        
       
        for (int a  = 0; a < roadNumber; a++)
        {
            bool started = false;
            int[] possibleSpawns = generateRandomArray(roadNumber);
            int spawnPos = possibleSpawns[a];

            if (spawnPos > 3)
            {
                spawnPos -= 3;
            }

            if ( spawnPos == 2)
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

            if (spawnPos == 3)
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

            if (spawnPos == 0)
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

            if (spawnPos == 1)
            {
                while (started == false)
                {
                    startingpointX = 0;
                    startingpointY = Random.Range(0, gridSize);
                    if (gridMap[startingpointX, startingpointY].tile == null)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile(piece.starter,1);
                        gridMap[startingpointX, startingpointY].filled = true;
                        started = true;
                    }

                }
            }
            createPath(startingpointX, startingpointY);
        }
        
        fillInBlanks();
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

        for (int i = 0; i < 100;  i++)
        {
            curveCounter ++;
            piece pieceType = currentPiece.tile.getTileType();
            if (curveCounter % 5 != 0)
            {
                nextDiagPieces = new piece[] { piece.diagonal };
                nextPieces = new piece[] {piece.straight };     
            }
            else
            {
                nextPieces = new piece[] { piece.straight,piece.halfcurveright, piece.halfcurveleft, piece.fullcurve, piece.straight };
                nextDiagPieces = new piece[] { piece.diagonal, piece.halfcurveright,piece.halfcurveleft };
            }


            if (currentPiece.tile.north == connectorType.straight)
            {
                PosY++;

                if (PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", connectorType.straight, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = connectorType.no;
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

                if (PosX < gridSize && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South",connectorType.right, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = connectorType.no;
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                    }
                    catch (System.Exception )
                    {

                    }
                    currentPiece.tile.west = connectorType.no;
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                    }
                    catch (System.Exception){ }
                   
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.north == connectorType.left)
            {
                PosX--;
                PosY++;
                if (PosX >= 0 && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South",connectorType.left, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = connectorType.no;
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                    }
                    catch(System.Exception ) { }
                    currentPiece.tile.east = connectorType.no;
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                    }
                    catch (System.Exception ) { }
                }

                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.south == connectorType.straight)
            {
                PosY--;
                if (PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", connectorType.straight, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = connectorType.no;
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
                if (PosX >= 0 && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North",connectorType.right, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = connectorType.no;
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                    }
                    catch(System.Exception) { }
                    edgePlaceX = PosX + 1;
                    currentPiece.tile.east = connectorType.no;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.south == connectorType.left)
            {
                PosX++;
                PosY--;
                if (PosX < gridSize && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", connectorType.left, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = connectorType.no;
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.west = connectorType.no;
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == connectorType.straight)
            {
                PosX++;
                if (PosX < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.straight, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = connectorType.no;
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
                if (PosX < gridSize && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.right, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = connectorType.no;
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 1);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.north = connectorType.no;
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 3);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == connectorType.left)
            {
                PosX++;
                PosY++;
                if (PosX < gridSize && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", connectorType.left, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = connectorType.no;
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 2);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.south = connectorType.no;
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 0);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.west == connectorType.straight)
            {
                PosX--;

                if (PosX >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.straight, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
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

                if (PosX >= 0 && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.right, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 3);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.south = connectorType.no;
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 1);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.west == connectorType.left)
            {
                PosX--;
                PosY--;

                if (PosX >= 0 && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", connectorType.left, nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = connectorType.no;
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile(piece.extraedge, 0);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.north = connectorType.no;
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile(piece.extraedge, 2);
                    }
                    catch(System.Exception) { }
                   
                }
                else if (gridMap[PosX, PosY].tile != null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(getConnectorType(connectorType.left, gridMap[PosX, PosY].tile.getTileType()), matchRotation("East", connectorType.left, getConnectorType(connectorType.left, gridMap[PosX, PosY].tile.getTileType())));
                    break;
                }
                else { break; }
            }
        }
        
        
    }

     private piece getConnectorType(connectorType current, piece collider)
    {
        piece piecetoReturn = piece.pavement;
        connectorType currentMatch = current;
        piece toSlot = collider;

        if (toSlot == piece.straight)
        {
            if (current == connectorType.left)
            {
                piecetoReturn = piece.straightleft;
            }
            else if (current == connectorType.right)
            {
                piecetoReturn = piece.straightright;
            }
            else if (current == connectorType.straight)
            {
                piecetoReturn = piece.straightstraight;
            }
        }
        else if (toSlot == piece.fullcurve)
        {
            if (current == connectorType.left)
            {
                piecetoReturn = piece.curvedleft;
            }
            else if (current == connectorType.right)
            {
                piecetoReturn = piece.curvedright;
            }
            else if (current == connectorType.straight)
            {
                piecetoReturn = piece.curvedstraight;
            }
        }
        else if (toSlot == piece.halfcurveleft)
        {
            if (current == connectorType.left)
            {
                piecetoReturn = piece.halfleftleft;
            }
            else if (current == connectorType.right)
            {
                piecetoReturn = piece.halfleftright;
            }
            else if (current == connectorType.straight)
            {
                piecetoReturn = piece.halfleftstraight;
            }

        }
        else if (toSlot == piece.halfcurveright)
        {
            if (current == connectorType.left)
            {
                piecetoReturn = piece.halfrightleft;
            }
            else if (current == connectorType.right)
            {
                piecetoReturn = piece.halfrightright;
            }
            else if (current == connectorType.straight)
            {
                piecetoReturn = piece.halfrightstraight;
            }

        }
        else if (toSlot == piece.diagonal)
        {
            if (current == connectorType.left)
            {
                piecetoReturn = piece.diagleft;
            }
            else if (current == connectorType.right)
            {
                piecetoReturn = piece.diagright;
            }
            else if (current == connectorType.straight)
            {
                piecetoReturn = piece.diagstraight;
            }
        }
        else if (toSlot == piece.extraedge)
        {
            piecetoReturn = piece.diagonal;
        }
        else if (toSlot == piece.extraedge)
        {
            piecetoReturn = piece.diagonal;
        }

        return piecetoReturn;
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
            while(east != faceType)
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
                    gridMap[x, y].tile = new Tile(piece.pavement, 0);
                    gridMap[x,y].filled = true;
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
                    Instantiate(diagonalObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.straight)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurveright)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveRightObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.halfcurveleft)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveLeftObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == piece.starter)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(starterObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x, y].tile.getTileType() == piece.pavement)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(pavementObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x, y].tile.getTileType() == piece.fullcurve)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(fullcurveObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x, y].tile.getTileType() == piece.extraedge)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(extraedgeObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x,y].tile.getTileType() == piece.centre)
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(centreObject, position, gridMap[x, y].tile.getRoatation());
                }
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
    straightstraight,
    straightleft,
    straightright,
    curvedstraight,
    curvedright,
    curvedleft,
    diagstraight,
    diagleft,
    diagright,
    halfrightstraight,
    halfrightright,
    halfrightleft,
    halfleftstraight,
    halfleftright,
    halfleftleft
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
    public Quaternion getRoatation()
    {
        return rotater;
    }
}








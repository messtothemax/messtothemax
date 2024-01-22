using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        spawnroads(roadNum);
    }

    void spawnroads(int roadNumber)
    {
        int startingpointX = 0;
        int startingpointY = 0;
       
        for (int a  = 0; a < roadNumber; a++)
        {
            bool started = false;
            int spawnPos = Random.Range(0, 5);
        
            
            if ( spawnPos == 2)
            {
                while (started == false)
                {
                    startingpointX = Random.Range(0, gridSize);
                    startingpointY = gridSize - 1;
                    if (gridMap[startingpointX, startingpointY].filled == false)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile("Starter", 2);
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
                    if (gridMap[startingpointX, startingpointY].filled == false)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile("Starter", 3);
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
                    if (gridMap[startingpointX, startingpointY].filled == false)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile("Starter", 0);
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
                    if (gridMap[startingpointX, startingpointY].filled == false)
                    {
                        gridMap[startingpointX, startingpointY].tile = new Tile("Starter",1);
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
        string[] nextPieces;
        string[] nextDiagPieces;
        Grid currentPiece = gridMap[PosX, PosY];
        string nextpiece;

        for (int i = 0; i < 100;  i++)
        {
            string pieceType = currentPiece.tile.getTileType();
            if (pieceType == "HalfCurveRight" || pieceType == "HalfCurveLeft" || pieceType == "FullCurve")
            {
                nextDiagPieces = new string[] { "Diagonal" };
                nextPieces = new string[] { "Straight" };     
            }
            else
            {
                nextPieces = new string[] { "Straight", "HalfCurveRight", "HalfCurveLeft", "FullCurve","Straight","FullCurve" };
                nextDiagPieces = new string[] { "Diagonal", "HalfCurveRight", "HalfCurveLeft", "HalfCurveLeft","HalfCurveRight" };
            }


            if (currentPiece.tile.north == "Straight")
            {
                PosY++;

                if (PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", "Straight", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = "No";
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.north == "Right")
            {
                PosX++;
                PosY++;

                if (PosX < gridSize && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", "Right", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = "No";
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 2);
                    }
                    catch (System.Exception )
                    {

                    }
                    currentPiece.tile.west = "No";
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 0);
                    }
                    catch (System.Exception){ }
                   
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.north == "Left")
            {
                PosX--;
                PosY++;
                if (PosX >= 0 && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("South", "Left", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.south = "No";
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 3);
                    }
                    catch(System.Exception ) { }
                    currentPiece.tile.east = "No";
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 1);
                    }
                    catch (System.Exception ) { }
                }

                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.south == "Straight")
            {
                PosY--;
                if (PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", "Straight", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = "No";
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.south == "Right")
            {
                PosX--;
                PosY--;
                if (PosX >= 0 && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", "Right", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = "No";
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 0);
                    }
                    catch(System.Exception) { }
                    edgePlaceX = PosX + 1;
                    currentPiece.tile.east = "No";
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 2);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }

            }
            else if (currentPiece.tile.south == "Left")
            {
                PosX++;
                PosY--;
                if (PosX < gridSize && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("North", "Left", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.north = "No";
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 1);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.west = "No";
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 3);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == "Straight")
            {
                PosX++;
                if (PosX < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", "Straight", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = "No";
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == "Right")
            {
                PosX++;
                PosY--;
                if (PosX < gridSize && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", "Right", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = "No";
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 1);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.north = "No";
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 3);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.east == "Left")
            {
                PosX++;
                PosY++;
                if (PosX < gridSize && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("West", "Left", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.west = "No";
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 2);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.south = "No";
                    edgePlaceX = PosX - 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 0);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.west == "Straight")
            {
                PosX--;

                if (PosX >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextPieces[Random.Range(0, nextPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", "Straight", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = "No";
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.west == "Right")
            {
                PosX--;
                PosY++;

                if (PosX >= 0 && PosY < gridSize && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", "Right", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = "No";
                    edgePlaceY = PosY - 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 3);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.south = "No";
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 1);
                    }
                    catch (System.Exception) { }
                }
                else
                {
                    break;
                }
            }
            else if (currentPiece.tile.west == "Left")
            {
                PosX--;
                PosY--;

                if (PosX >= 0 && PosY >= 0 && gridMap[PosX, PosY].tile == null)
                {
                    nextpiece = nextDiagPieces[Random.Range(0, nextDiagPieces.Length)];
                    gridMap[PosX, PosY].tile = new Tile(nextpiece, matchRotation("East", "Left", nextpiece));
                    gridMap[PosX, PosY].filled = true;
                    currentPiece = gridMap[PosX, PosY];
                    currentPiece.tile.east = "No";
                    edgePlaceY = PosY + 1;
                    try
                    {
                        gridMap[PosX, edgePlaceY].tile = new Tile("Edge", 0);
                    }
                    catch(System.Exception) { }
                    currentPiece.tile.north = "No";
                    edgePlaceX = PosX + 1;
                    try
                    {
                        gridMap[edgePlaceX, PosY].tile = new Tile("Edge", 2);
                    }
                    catch(System.Exception) { }
                   
                }
                else
                {
                    break;
                }
            }
        }
        
        
    }

     int matchRotation(string direction, string faceType, string tileType)
    {
        string north = "";
        string south = "";
        string east = "";
        string west = "";
        int rotation = 0;


        if (tileType == "Starter")
        {
            north = "Straight";
            south = "No";
            east = "No";
            west = "No";
        }

        else if (tileType == "Diagonal")
        {
            north = "Right";
            south = "Right";
            east = "Left";
            west = "Left";
        }

        else if (tileType == "Pavement")
        {
            north = "No";
            south = "No";
            east = "No";
            west = "No";
        }

        else if (tileType == "HalfCurveRight")
        {
            north = "Right";
            south = "Straight";
            east = "Left";
            west = "No";
        }
        else if (tileType == "HalfCurveLeft")
        {
            north = "Left";
            south = "Straight";
            east = "No";
            west = "Right";
        }
        else if (tileType == "FullCurve")
        {
            north = "Straight";
            south = "No";
            east = "No";
            west = "Straight";
        }
        else if (tileType == "Straight")
        {
            north = "Straight";
            south = "Straight";
            east = "No";
            west = "No";
        }

        if (direction == "East")
        {
            while(east != faceType)
            {
                string placeholder = north;
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
                string placeholder = north;
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
                string placeholder = north;
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
                string placeholder = north;
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
                    gridMap[x, y].tile = new Tile("Pavement", 0);
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
                if (gridMap[x, y].tile.getTileType() == "Diagonal")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(diagonalObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "Straight")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(straightObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "HalfCurveRight")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveRightObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "HalfCurveLeft")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(halfcurveLeftObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "Starter")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(starterObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "ExtraEdge")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(extraedgeObject, position, gridMap[x, y].tile.getRoatation());
                }

                else if (gridMap[x, y].tile.getTileType() == "Pavement")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(pavementObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x, y].tile.getTileType() == "FullCurve")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(fullcurveObject, position, gridMap[x, y].tile.getRoatation());
                }
                else if (gridMap[x, y].tile.getTileType() == "Edge")
                {
                    Vector3 position = new Vector3(6.4f * (float)x, 0, 6.4f * (float)y);
                    Instantiate(extraedgeObject, position, gridMap[x, y].tile.getRoatation());
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

public class Tile
{
    public string north;
    public string south;
    public string east;
    public string west;
    int rotation;
    string tileType;
    Quaternion rotater;
    int RTQ = 0;
    public Tile(string tile, int rotateamount)
    {
        tileType = tile;
        rotation = rotateamount;

        if (tileType == "Starter")
        {
            north = "Straight";
            south = "No";
            east = "No";
            west = "No";
        }

        else if (tileType == "Diagonal")
        {
            north = "Right";
            south = "Right";
            east = "Left";
            west = "Left";
        }

        else if (tileType == "Pavement")
        {
            north = "No";
            south = "No";
            east = "No";
            west = "No";
        }

        else if (tileType == "HalfCurveRight")
        {
            north = "Right";
            south = "Straight";
            east = "Left";
            west = "No";
        }
        else if (tileType == "HalfCurveLeft")
        {
            north = "Left";
            south = "Straight";
            east = "No";
            west = "Right";
        }
        else if (tileType == "FullCurve")
        {
            north = "Straight";
            south = "No";
            east = "No";
            west = "Straight";
        }
        else if (tileType == "Straight")
        {
            north = "Straight";
            south = "Straight";
            east = "No";
            west = "No";
        }
        else if (tileType == "Edge")
        {
            north = "No";
            south = "No";
            east = "No";
            west = "No";
        }
        calculateinitialRotation();

    }
    public void calculateinitialRotation()
    {
        
        for (int i = 0; i < rotation; i++)
        {
            string placeholder = north;
            north = west;
            west = south;
            south = east;
            east = placeholder;
            
        }
        RTQ = rotation * 90;
        rotater = Quaternion.Euler(0, RTQ, 0);
    }
    public string getTileType()
    {
        return tileType;
    }
    public Quaternion getRoatation()
    {
        return rotater;
    }
}








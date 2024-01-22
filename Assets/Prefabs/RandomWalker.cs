using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : MonoBehaviour
{
    
    public VectorScript vectorField; 
    public int numWalkers = 5;
    public int maxSteps = 1000;

    public GameObject roadNodePrefab; 

    void Start()
    {
        TraceRoads();
    }

    void TraceRoads()
    {
        for (int i = 0; i < numWalkers; i++)
        {
            Vector2 startPos = new Vector2(Random.Range(0, vectorField.gridSizeX * vectorField.cellSize), Random.Range(0, vectorField.gridSizeY * vectorField.cellSize));
            WalkRoad(startPos);
        }
    }

    void WalkRoad(Vector2 startPos)
    {
        Vector2 currentPosition = startPos;

        for (int step = 0; step < maxSteps; step++)
        {

            Vector3 vector = vectorField.vectorField[Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y)];
            float compare = Vector3.Dot(vector.normalized, startPos.normalized);
            float compsin = Mathf.Acos(compare);
            if (step < 2)
            {
                currentPosition += new Vector2(vector.x, vector.z);
            }
            else if (compsin < -1.7 || compsin > 1.7)
            {
                startPos = currentPosition;
                vector = Quaternion.Euler(0, 180, 0) * vector;
                currentPosition += new Vector2(vector.x, vector.z);
            }
            else
            {
                startPos = currentPosition;
                currentPosition += new Vector2(vector.x, vector.z);
            }


            //currentPosition += new Vector2(vector.x, vector.z);

            currentPosition.x = Mathf.Clamp(currentPosition.x, 0f, vectorField.gridSizeX * vectorField.cellSize - 1);
            currentPosition.y = Mathf.Clamp(currentPosition.y, 0f, vectorField.gridSizeY * vectorField.cellSize - 1);

            CreateRoadNode(new Vector3(currentPosition.x, 0f, currentPosition.y)); 
        }
    }

    void CreateRoadNode(Vector3 position)
    {
      
        Instantiate(roadNodePrefab, position, Quaternion.identity);
    }


    void Update()
    {
        
    }
}

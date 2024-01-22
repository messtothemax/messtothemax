using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VectorScript : MonoBehaviour
{
    
    public int gridSizeX = 100;
    public int gridSizeY = 100;
    public float cellSize = 1f;
    public float arrowScale = 1f;
    public Vector3[,] vectorField;

    void Start()
    {
        GenerateVectorField();
    }

    void GenerateVectorField()
    {
        vectorField = new Vector3[gridSizeX * Mathf.FloorToInt(cellSize), gridSizeY * Mathf.FloorToInt(cellSize) ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
 
                Vector3 vector = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                vectorField[x, y] = vector;

                Vector3 cellPosition = new Vector3(x, 0f, y);
                CreateArrow(cellPosition, vector);
                
            }
        }
    }

    void CreateArrow(Vector3 position, Vector3 direction)
    {
        GameObject arrow = new GameObject("Arrow");
        arrow.transform.position = position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        arrow.transform.rotation = rotation;
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.parent = arrow.transform;
        cylinder.transform.localPosition = new Vector3(0f, 0f, 0.5f * arrowScale);
        cylinder.transform.localScale = new Vector3(0.1f, arrowScale, 0.1f);
        cylinder.GetComponent<Renderer>().material.color = Color.blue;
    }

        void Update()
    {
        
    }
}

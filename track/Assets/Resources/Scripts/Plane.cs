using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Plane : MonoBehaviour
{

    private float quadSize = 1f;

    private int planeSizeX = 100; // 10 quads on the x axis

    private int planeSizeZ = 100; // 10 quads on the z axis

    private int meshSize = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            Objective.x = 0;
            Track.Des();
            Track.Spawn();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RenderPlane();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RenderPlane(){
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();

        meshFilter.mesh = CreatePlane();

        meshRenderer.material =new Material(Shader.Find("Specular"));
    }

    private Mesh CreatePlane(){ 

        MeshGenerator meshGenerator = new MeshGenerator(meshSize);


        //points
        Vector3[,] quadPoints = new Vector3[planeSizeX, planeSizeZ];

        for(int x = 0; x < planeSizeX; x++){
            for(int z = 0; z < planeSizeZ; z++){
                quadPoints[x , z] = new Vector3(quadSize * x, 0, quadSize * z);
            }
        }

        //create quads
        for(int x = 0; x < planeSizeX - 1; x++){
            for(int z = 0; z < planeSizeZ - 1; z++){
                Vector3 point1 = quadPoints[x+1 , z];
                Vector3 point2 = quadPoints[x   , z];
                Vector3 point3 = quadPoints[x   , z+1];
                Vector3 point4 = quadPoints[x+1 , z+1];

                meshGenerator.CreateTriangle(point1, point2, point3, 0);
                meshGenerator.CreateTriangle(point1, point3, point4, 0);
            }
        }

        return meshGenerator.CreateMesh();
    }
}

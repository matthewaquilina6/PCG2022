using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureData
{
    public Texture2D terrainTexture;

    public Vector2 tileSize;

    public float min;

    public float max;

}

public class TreeData
{
    public GameObject treeMesh;

    public float min;

    public float max;

}
public class Code : MonoBehaviour
{

    private Terrain terrain;

    private TerrainData terrainData;
    private float perlinNoiseWidthScale = 0.01f;
    private float perlinNoiseHeightScale = 0.01f;
    private float min = 0f;
    private float max = 0.01f;
    public List<TerrainTextureData> terrainTextureData = new List<TerrainTextureData>();
    private float terrainTextureBlendOffset = 0.01f;
    public List<TreeData> treeData = new List<TreeData>();
    int maxTrees = 10000;
    int treeSpacing = 10;
    int terrainLayerIndex = 8;
    GameObject water;
    float waterHeight = 0.25f;
    GameObject cloud;
    float cloudHeight = 1f;
    GameObject fire;
    GameObject player;
    GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        if (terrain == null)
        { terrain = this.GetComponent<Terrain>(); }
        perlinNoiseWidthScale = Random.Range(0.01f, 0.02f);
        perlinNoiseHeightScale = Random.Range(0.01f, 0.02f);
        if (terrainData == null)
        { terrainData = Terrain.activeTerrain.terrainData; }
        GenerateHeights();
        AddTerrainTextures();
        AddTrees();
        Water();
        Cloud();
        Player();
    }
    void GenerateHeights()
    {
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        for (int width = 0; width < terrainData.heightmapResolution; width++)
        {
            for (int height = 0; height < terrainData.heightmapResolution; height++)
            {
                heightMap[width, height] = Random.Range(min, max);
                heightMap[width, height] += Mathf.PerlinNoise(width * perlinNoiseWidthScale, height * perlinNoiseHeightScale);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    void AddTerrainTextures()
    {
        GetTextures();
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainTextureData.Count];
        for (int i = 0; i < terrainTextureData.Count; i++)
        {
            terrainLayers[i] = new TerrainLayer();
            terrainLayers[i].diffuseTexture = terrainTextureData[i].terrainTexture;
            terrainLayers[i].tileSize = terrainTextureData[i].tileSize;
        }
        terrainData.terrainLayers = terrainLayers;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,,] alphamapList = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int height = 0; height < terrainData.alphamapHeight; height++)
        {
            for (int width = 0; width < terrainData.alphamapWidth; width++)
            {

                float[] alphamap = new float[terrainData.alphamapLayers];
                for (int i = 0; i < terrainTextureData.Count; i++)
                {

                    float heightBegin = terrainTextureData[i].min - terrainTextureBlendOffset;
                    float heightend = terrainTextureData[i].max + terrainTextureBlendOffset;

                    if ((heightMap[width, height] >= heightBegin) && (heightMap[width, height] <= heightend))
                    {
                        alphamap[i] = 1;
                    }
                }
                Blend(alphamap);
                for (int j = 0; j < terrainTextureData.Count; j++)
                {
                    alphamapList[width, height, j] = alphamap[j];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, alphamapList);
    }

    void Blend(float[] alphamap)
    {
        float total = 0;
        for (int i = 0; i < alphamap.Length; i++)
        {
            total += alphamap[i];
        }
        for (int i = 0; i < alphamap.Length; i++)
        {
            alphamap[i] = alphamap[i] / total;
        }
    }

    void GetTextures()
    {
        TerrainTextureData snow = new TerrainTextureData();
        snow.terrainTexture = Resources.Load("Textures/snow") as Texture2D;
        snow.tileSize = new Vector2(20, 20);
        snow.min = 0.8f;
        snow.max = 1f;
        terrainTextureData.Add(snow);
        TerrainTextureData dirt = new TerrainTextureData();
        dirt.terrainTexture = Resources.Load("Textures/dirt") as Texture2D;
        dirt.tileSize = new Vector2(20, 20);
        dirt.min = 0.6f;
        dirt.max = 0.8f;
        terrainTextureData.Add(dirt);
        TerrainTextureData grass = new TerrainTextureData();
        grass.terrainTexture = Resources.Load("Textures/grass") as Texture2D;
        grass.tileSize = new Vector2(20, 20);
        grass.min = 0.25f;
        grass.max = 0.6f;
        terrainTextureData.Add(grass);
        TerrainTextureData under = new TerrainTextureData();
        under.terrainTexture = Resources.Load("Textures/under") as Texture2D;
        under.tileSize = new Vector2(20, 20);
        under.min = 0f;
        under.max = 0.25f;
        terrainTextureData.Add(under);
    }

    private void AddTrees()
    {
        GetTree();
        TreePrototype[] trees = new TreePrototype[treeData.Count];

        for (int i = 0; i < treeData.Count; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = treeData[i].treeMesh;
        }

        terrainData.treePrototypes = trees;

        List<TreeInstance> treeInstanceList = new List<TreeInstance>();

        for (int z = 0; z < terrainData.size.z; z += treeSpacing)
        {
            for (int x = 0; x < terrainData.size.x; x += treeSpacing)
            {
                for (int treeIndex = 0; treeIndex < trees.Length; treeIndex++)
                {
                    if (treeInstanceList.Count < maxTrees)
                    {
                        float currentHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        if (currentHeight >= treeData[treeIndex].min && currentHeight <= treeData[treeIndex].max)
                        {
                            float randomX = (x + Random.Range(-5.0f, 5.0f)) / terrainData.size.x;
                            float randomZ = (z + Random.Range(-5.0f, 5.0f)) / terrainData.size.z;

                            Vector3 treePosition = new Vector3(randomX * terrainData.size.x, currentHeight * terrainData.size.y, randomZ * terrainData.size.z) + this.transform.position;
                            RaycastHit raycastHit;
                            int layerMask = 1 << terrainLayerIndex;
                            if (Physics.Raycast(treePosition, -Vector3.up, out raycastHit, 100, layerMask) ||
                                Physics.Raycast(treePosition, Vector3.up, out raycastHit, 100, layerMask))
                            {
                                float treeDistance = (raycastHit.point.y - this.transform.position.y) / terrainData.size.y;
                                TreeInstance treeInstance = new TreeInstance();
                                treeInstance.position = new Vector3(randomX, treeDistance, randomZ);
                                treeInstance.rotation = Random.Range(0, 360);
                                treeInstance.prototypeIndex = treeIndex;
                                treeInstance.color = Color.white;
                                treeInstance.lightmapColor = Color.white;
                                treeInstance.heightScale = 0.95f;
                                treeInstance.widthScale = 0.95f;
                                treeInstanceList.Add(treeInstance);
                            }
                        }
                    }
                }
            }
        }
        terrainData.treeInstances = treeInstanceList.ToArray();
    }

    void GetTree()
    {
        TreeData t1 = new TreeData();
        t1.treeMesh = Resources.Load("Prefabs/Tree9_2") as GameObject;
        t1.min = 0.7f;
        t1.max = 1f;
        treeData.Add(t1);
        TreeData t2 = new TreeData();
        t2.treeMesh = Resources.Load("Prefabs/Tree9_3") as GameObject;
        t2.min = 0.5f;
        t2.max = 0.7f;
        treeData.Add(t2);
        TreeData t3 = new TreeData();
        t3.treeMesh = Resources.Load("Prefabs/Tree9_4") as GameObject;
        t3.min = 0.4f;
        t3.max = 0.5f;
        treeData.Add(t3);
    }

    void Water()
    {
        water = Resources.Load("Prefabs/Water") as GameObject;
        GameObject w = Instantiate(water, this.transform.position, this.transform.rotation);
        w.name = "Water";
        w.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2, waterHeight * terrainData.size.y, terrainData.size.z / 2);
        w.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
    }
    void Cloud()
    {
        cloud = Resources.Load("Prefabs/Cloud") as GameObject;
        GameObject c = Instantiate(cloud, this.transform.position, this.transform.rotation);
        c.name = "Cloud";
        c.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2, cloudHeight * terrainData.size.y, terrainData.size.z / 2);
        c.transform.localScale = new Vector3(terrainData.size.x / 2, 1, terrainData.size.z / 2);
    }

    void Player()
    {
        player = Resources.Load("Prefabs/Player") as GameObject;
        camera = Resources.Load("Prefabs/Camera") as GameObject;
        GameObject c = Instantiate(camera, this.transform.position, this.transform.rotation);
        StartCoroutine(FireWorks());

    }
    IEnumerator FireWorks()
    {
        fire = Resources.Load("Prefabs/Fireworks") as GameObject;
        GameObject p = Instantiate(player, new Vector3(Random.Range(250, 750), 1f * terrainData.size.y, Random.Range(250, 750)), this.transform.rotation);
        while (true)
        {
            yield return new WaitForSeconds(10f);
            GameObject f = Instantiate(fire);
            f.transform.position =p.transform.position;
            yield return new WaitForSeconds(10f);
            Destroy(f);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    GameObject parent;
    public GameObject[] treeAssets;
    public GameObject[] houseAssets;
    List<Vector3> treeSizes = new();
    List<Vector3> houseSizes = new();
    float terrainWidth = 0;
    float terrainLength = 0;
    public string parentName = "trees";
    public int treeQuantity = 6000;
    public int houseQuantity = 2500;
    public float maxRadius = 400;
    public float centerX = 0;
    public float centerZ = 0;
    public float scale = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        this.SetParentGameObject(this.parentName);
        this.CalculateTerrainCenter();
        this.CalculateGameObjectSizes(this.treeAssets, this.treeSizes);
        this.CalculateGameObjectSizes(this.houseAssets, this.houseSizes);
    }

    void SetParentGameObject(string pName)
    {
        // Find existing parent GameObject by name
        this.parent = GameObject.Find(pName);
        //Debug.Log(parent.name);
        // If the parent doesn't exist, create it
        if (this.parent == null)
        {
            this.parent = new GameObject(pName);
        }
        else
        {
            // Clear existing children of the parent GameObject
            this.DestroyAllChildren();
        }
    }

    void CalculateGameObjectSizes(GameObject[] gameObjects, List<Vector3> array)
    {
        foreach(GameObject gameObject in gameObjects)
        {
            array.Add(this.GetAssetSize(gameObject));
        }
    }

    Vector3 GetAssetSize(GameObject gameObject)
    {
        GameObject tmpAsset = Instantiate(gameObject, new(0f, 0f, 0f), Quaternion.identity, this.parent.transform);
        Destroy(tmpAsset);
        return tmpAsset.GetComponent<Renderer>().bounds.size;
    }

    void CalculateTerrainCenter()
    {
        // Assuming the script is attached to the GameObject with the Terrain component
        GameObject go = GameObject.Find("Terrain");
        
        if (go.TryGetComponent<Terrain>(out var terrain))
        {
            TerrainData terrainData = terrain.terrainData;

            // Get the size of the terrain
            this.terrainWidth = terrainData.size.x;
            this.terrainLength = terrainData.size.z;

            // Calculate the center of the terrain
            this.centerX = this.terrainWidth / 2f;
            this.centerZ = this.terrainLength / 2f;
        }
        else
        {
            Debug.LogError("Terrain component not found!");
        }
    }

    public void PlaceAssetsInCartesian(string gameObjectName)
    {
        if (gameObjectName.Equals("trees"))
        {
            this.PlaceAssetsInCartesian(this.treeAssets, this.treeSizes, this.treeQuantity);
        }
        if (gameObjectName.Equals("houses"))
        {
            this.PlaceAssetsInCartesian(this.houseAssets, this.houseSizes, this.houseQuantity);
        }
    }

    public void PlaceAssetsInPolar(string gameObjectName)
    {
        if (gameObjectName.Equals("trees"))
        {
            this.PlaceAssetsInPolar(this.treeAssets, this.treeSizes, this.treeQuantity);
        }
        if (gameObjectName.Equals("houses"))
        {
            this.PlaceAssetsInPolar(this.houseAssets, this.houseSizes, this.houseQuantity);
        }
    }

    void PlaceAssetsInCartesian(GameObject[] gameObjects, List<Vector3> sizes, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            int objectIdx = Random.Range(0, gameObjects.Length - 1);
            float x = Random.Range(0, this.terrainWidth);
            float z = Random.Range(0, this.terrainLength);
            float scale = Random.Range(this.scale * 0.8f, this.scale * 1.2f);

            Vector3 position = new(x, (sizes[objectIdx].y) * scale, z);
            this.PlaceAsset(this.parent, gameObjects[objectIdx], position, this.scale);
        }
    }

    void PlaceAssetsInPolar(GameObject[] gameObjects, List<Vector3> sizes, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomRadius = Random.Range(8, this.maxRadius);

            // Convert polar coordinates to Cartesian coordinates
            float x = randomRadius * Mathf.Cos(Mathf.Deg2Rad * randomAngle) + this.centerX;
            float z = randomRadius * Mathf.Sin(Mathf.Deg2Rad * randomAngle) + this.centerZ;

            if (x < 0f || x > this.terrainLength || z < 0f || z > this.terrainWidth)
            {
                continue;
            }
            int objectIdx = Random.Range(0, gameObjects.Length - 1);
            Vector3 position;
            float localScale = Random.Range(0.5f * this.scale, 1f * this.scale);
            if (gameObjects[objectIdx].GetComponent<BoxCollider>() != null)
            {
                position = new(x, 0, z);
            }
            else
            {
                position = new(x, (sizes[objectIdx].y / 2) * localScale - 0.1f, z);
            }
            this.PlaceAsset(this.parent, gameObjects[objectIdx], position, localScale);
        }
    }

    public void DestroyAllChildren()
    {
        for (int i = this.parent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(this.parent.transform.GetChild(i).gameObject);
        }
    }

    void PlaceAsset(GameObject parent, GameObject asset, Vector3 position, float localScale)
    {
        // Check if the terrain exists
        if (Terrain.activeTerrain != null)
        {
            if (asset.GetComponent<BoxCollider>() != null)
            {
                BoxCollider boxCollider = asset.GetComponent<BoxCollider>();
                Ray ray = new(position + boxCollider.center, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    position.y = hit.point.y;
                }
            }
            // 0, 1, 2, or 3 (for 0, 90, 180, or 270 degrees)
            float rotationAngle = Random.Range(0, 4) * 90f + Random.Range(-10, 10);
            GameObject newAsset = Instantiate(asset, position, Quaternion.Euler(0f, rotationAngle, 0f), parent.transform);
            //Debug.Log(newAsset.transform.position);
            newAsset.transform.localScale = new(localScale, localScale, localScale);
        }
        else
        {
            Debug.LogError("Terrain not found in the scene!");
        }
    }

    public void Update()
    {
        // Custom action on '1' key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.DestroyAllChildren();
        }

        // Custom action on '1' key press
        if (Input.GetKeyDown(KeyCode.Alpha1) && this.treeAssets.Length > 0)
        {
            this.PlaceAssetsInCartesian(this.treeAssets, this.treeSizes, this.treeQuantity);
        }

        // Custom action on '2' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha2) && this.treeAssets.Length > 0)
        {
            this.PlaceAssetsInPolar(this.treeAssets, this.treeSizes, this.treeQuantity);
        }

        // Custom action on '3' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha3) && this.houseAssets.Length > 0)
        {
            this.PlaceAssetsInCartesian(this.houseAssets, this.houseSizes, this.houseQuantity);
        }

        // Custom action on '3' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha4) && this.houseAssets.Length > 0)
        {
            this.PlaceAssetsInPolar(this.houseAssets, this.houseSizes, this.houseQuantity);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

class ObjectPrefab {
    public Vector3 position;
    public float size;
    public float rotation;
    public GameObject asset;
    public string parentName;

    public ObjectPrefab(Vector3 position, float size, float rotation, GameObject asset, string parentName) { 
        this.parentName = parentName;
        this.position = position;
        this.size = size;
        this.rotation = rotation;
        this.asset = asset;
    }
}

public class PlaceObjects : MonoBehaviour
{
    GameObject parentTrees;
    GameObject parentHouses;
    public GameObject[] treeAssets;
    public GameObject[] houseAssets;
    List<Vector3> treeSizes = new();
    List<Vector3> houseSizes = new();
    float terrainWidth = 0;
    float terrainLength = 0;

    public string parentNameTrees = "trees";
    public string parentNameHouses = "houses";
    public int treeQuantity = 6000;
    public int houseQuantity = 2500;
    public float maxRadius = 400;
    public float maxScale;
    public float minScale;

    private float centerX = 0;
    private float centerZ = 0;

    public int installsPerFrame = 20;
    private List<ObjectPrefab> objectsToPlace = new();

    public bool shadowsOnly = false;

    // Start is called before the first frame update
    void Start()
    {
        this.parentTrees = GameObject.Find(this.parentNameTrees);
        this.parentTrees = this.SetParentGameObject(this.parentTrees, this.parentNameTrees);

        this.parentHouses = GameObject.Find(this.parentNameHouses);
        this.parentHouses = this.SetParentGameObject(this.parentHouses, this.parentNameHouses);

        this.CalculateTerrainCenter();
        this.CalculateGameObjectSizes(this.treeAssets, this.treeSizes);
        this.CalculateGameObjectSizes(this.houseAssets, this.houseSizes);
    }

    GameObject SetParentGameObject(GameObject parent, string name)
    {
        if (parent == null)
        {
            parent = new GameObject(name);
        }
        else
        {
            this.DestroyAllChildren(parent);
        }

        return parent;
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
        GameObject tmpAsset = Instantiate(gameObject, new(0f, 0f, 0f), Quaternion.identity, this.parentTrees.transform);
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

    public void PlaceAssetsInCartesian(string parentName, bool placeAsset)
    {
        if (parentName.Equals("trees"))
        {
            this.PlaceAssetsInCartesian(this.treeAssets, this.treeSizes, this.treeQuantity, placeAsset, parentName);
        }
        if (parentName.Equals("houses"))
        {
            this.PlaceAssetsInCartesian(this.houseAssets, this.houseSizes, this.houseQuantity, placeAsset, parentName);
        }
    }

    public void PlaceAssetsInPolar(string parentName, bool placeAsset)
    {
        if (parentName.Equals("trees"))
        {
            this.PlaceAssetsInPolar(this.treeAssets, this.treeSizes, this.treeQuantity, placeAsset, parentName);
        }
        if (parentName.Equals("houses"))
        {
            this.PlaceAssetsInPolar(this.houseAssets, this.houseSizes, this.houseQuantity, placeAsset, parentName);
        }
    }

    void PlaceAssetsInCartesian(GameObject[] gameObjects, List<Vector3> sizes, int quantity, bool placeAsset, string parentName)
    {
        for (int i = 0; i < quantity; i++)
        {
            int objectIdx = Random.Range(0, gameObjects.Length - 1);
            float x = Random.Range(0, this.terrainWidth);
            float z = Random.Range(0, this.terrainLength);
            float localScale = Random.Range(this.minScale, this.maxScale);

            Vector3 position = new(x, (sizes[objectIdx].y) * localScale, z);
            float rotation = Random.Range(0, 4) * 90f + Random.Range(-10, 10);
            if (placeAsset)
            {
                if (parentName == this.parentNameTrees)
                {
                    this.PlaceAsset(this.parentTrees, gameObjects[objectIdx], position, localScale, rotation);
                }
                else if (parentName == this.parentNameHouses)
                {
                    this.PlaceAsset(this.parentHouses, gameObjects[objectIdx], position, localScale, rotation);
                }
            }
            else
            {
                this.objectsToPlace.Add(new ObjectPrefab(position, localScale, rotation, gameObjects[objectIdx], parentName));
            }
        }
    }

    void PlaceAssetsInPolar(GameObject[] gameObjects, List<Vector3> sizes, int quantity, bool placeAsset, string parentName)
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
            float localScale = Random.Range(this.minScale, this.maxScale);
            if (gameObjects[objectIdx].GetComponent<BoxCollider>() != null)
            {
                position = new(x, 0, z);
            }
            else
            {
                position = new(x, (sizes[objectIdx].y / 2) * localScale - 0.1f, z);
            }
            float rotation = Random.Range(0, 4) * 90f + Random.Range(-10, 10);
            if (placeAsset)
            {
                if (parentName == this.parentNameTrees)
                {
                    this.PlaceAsset(this.parentTrees, gameObjects[objectIdx], position, localScale, rotation);
                }
                else if (parentName == this.parentNameHouses)
                {
                    this.PlaceAsset(this.parentHouses, gameObjects[objectIdx], position, localScale, rotation);
                }
            }
            else
            {
                this.objectsToPlace.Add(new ObjectPrefab(position, localScale, rotation, gameObjects[objectIdx], parentName));
            }
        }
    }

    private void DestroyAllChildren(GameObject parent)
    {
        //Debug.Log("Destroy " + this.parent.transform.childCount + " children");
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public void DestroyAllChildren(string parentName)
    {
        if (parentName == this.parentNameTrees)
        {
            //Debug.Log("Destroy " + this.parent.transform.childCount + " children");
            for (int i = this.parentTrees.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(this.parentTrees.transform.GetChild(i).gameObject);
            }
        }
        else if (parentName == this.parentNameHouses)
        {
            //Debug.Log("Destroy " + this.parent.transform.childCount + " children");
            for (int i = this.parentHouses.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(this.parentHouses.transform.GetChild(i).gameObject);
            }
        }
    }

    void PlaceAsset(GameObject parent, GameObject asset, Vector3 position, float localScale, float rotation)
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
            GameObject newAsset = Instantiate(asset, position, Quaternion.Euler(0f, rotation, 0f), parent.transform);
            //Debug.Log(newAsset.transform.position);
            newAsset.transform.localScale = new(localScale, localScale, localScale);
            MeshRenderer meshRenderer = newAsset.GetComponent<MeshRenderer>();
            if (meshRenderer != null && this.shadowsOnly)
            {
                meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
            // Change base color
            foreach (Material mat in newAsset.GetComponent<Renderer>().materials)
            {
                Color c = new(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f));
                mat.SetColor("_BaseColor", c);
            }
        }
        else
        {
            Debug.LogError("Terrain not found in the scene!");
        }
    }

    public void Update()
    {
        // Custom action on 'C' key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.DestroyAllChildren(this.parentHouses);
            this.DestroyAllChildren(this.parentTrees);
        }

        // Custom action on '1' key press
        if (Input.GetKeyDown(KeyCode.Alpha1) && this.treeAssets.Length > 0)
        {
            this.PlaceAssetsInCartesian(this.treeAssets, this.treeSizes, this.treeQuantity, true, this.parentNameTrees);
        }

        // Custom action on '2' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha2) && this.treeAssets.Length > 0)
        {
            this.PlaceAssetsInPolar(this.treeAssets, this.treeSizes, this.treeQuantity, true, this.parentNameTrees);
        }

        // Custom action on '3' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha3) && this.houseAssets.Length > 0)
        {
            this.PlaceAssetsInCartesian(this.houseAssets, this.houseSizes, this.houseQuantity, true, this.parentNameHouses);
        }

        // Custom action on '4' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha4) && this.houseAssets.Length > 0)
        {
            this.PlaceAssetsInPolar(this.houseAssets, this.houseSizes, this.houseQuantity, true, this.parentNameHouses);
        }

        // Custom action on '5' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha5) && this.houseAssets.Length > 0)
        {
            this.PlaceAssetsInPolar(this.treeAssets, this.treeSizes, this.treeQuantity, false, this.parentNameTrees);
        }

        if (this.objectsToPlace.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(this.installsPerFrame, this.objectsToPlace.Count); i++)
            {
                ObjectPrefab op = this.objectsToPlace[i];
                if (op.parentName == this.parentNameTrees)
                {
                    this.PlaceAsset(this.parentTrees, op.asset, op.position, op.size, op.rotation);
                }
                else if (op.parentName == this.parentNameHouses)
                {
                    this.PlaceAsset(this.parentHouses, op.asset, op.position, op.size, op.rotation);
                }

                this.objectsToPlace.RemoveAt(i);
                Debug.Log("Place object");
            }
        }
    }
}

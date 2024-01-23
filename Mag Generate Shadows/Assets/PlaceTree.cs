using UnityEngine;

public class PlaceTree : MonoBehaviour
{
    public GameObject[] assets;
    public int assetIdx = 0;
    public GameObject assetPrefab;
    public string parentName = "trees";
    public int objectQuantity = 3000;
    public float xLimitB = 130f;
    public float xLimitU = 150f;
    public float zLimitB = 130f;
    public float zLimitU = 150f;
    public float maxRadius = 400;
    public GameObject parent;
    public float assetWidth = 0;
    public float assetDepth = 0;
    public float centerX = 0;
    public float centerZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Find existing parent GameObject by name
        this.parent = GameObject.Find(parentName);
        //Debug.Log(parent.name);
        // If the parent doesn't exist, create it
        if (this.parent == null)
        {
            this.parent = new GameObject(parentName);
        }
        else
        {
            // Clear existing children of the parent GameObject
            foreach (Transform child in this.parent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        assetPrefab = assets[assetIdx];
        GameObject newAsset = Instantiate(assetPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, this.parent.transform);

        assetWidth = newAsset.GetComponent<Renderer>().bounds.size.x;
        assetDepth = newAsset.GetComponent<Renderer>().bounds.size.z;
        //Debug.Log("Asset size x: " + assetWidth + " z: " + assetDepth);

        this.CalculateTerrainCenter();

        Destroy(newAsset);

    }

    void CalculateTerrainCenter()
    {
        // Assuming the script is attached to the GameObject with the Terrain component
        GameObject go = GameObject.Find("Terrain");
        Terrain terrain = go.GetComponent<Terrain>();

        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;

            // Get the size of the terrain
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            // Calculate the center of the terrain
            this.centerX = terrainWidth / 2f;
            this.centerZ = terrainLength / 2f;
        }
        else
        {
            Debug.LogError("Terrain component not found!");
        }
    }

    void PlaceAssetsInCartesian(float assetWidth, float assetDepth)
    {
        this.DestroyAllChildren();
        for (float x = xLimitB; x < xLimitU; x += Random.Range(assetWidth * 0.05f, assetWidth * 0.25f))
        {
            for (float z = zLimitB; z < zLimitU; z += Random.Range(assetDepth * 0.05f, assetDepth * 0.25f))
            {
                float xtmp = Random.Range(assetWidth * -0.1f, assetWidth * 0.1f);
                //Debug.Log("Place an asset x: "+ xtmp + " z: "+z);
                this.PlaceAsset(this.parent, x + xtmp + this.centerX, z + this.centerZ);
            }
        }
    }

    public void PlaceAssetsInPolar()
    {
        for (int i = 0; i < objectQuantity; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomRadius = Random.Range(0, this.maxRadius);

            // Convert polar coordinates to Cartesian coordinates
            float x = randomRadius * Mathf.Cos(Mathf.Deg2Rad * randomAngle);
            float y = randomRadius * Mathf.Sin(Mathf.Deg2Rad * randomAngle);
            this.PlaceAsset(this.parent, x + this.centerX, y + this.centerZ);
        }
    }

    public void DestroyAllChildren()
    {
        foreach (Transform child in this.parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void PlaceAsset(GameObject parent, float xPosition, float zPosition)
    {
        // Check if the terrain exists
        if (Terrain.activeTerrain != null)
        {
            // Instantiate the asset as a child of the parent
            float yPosition = Terrain.activeTerrain.SampleHeight(new Vector3(xPosition, 0f, zPosition));
            GameObject newAsset = Instantiate(this.assets[Random.Range(0, this.assets.Length - 1)], new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent.transform);
            //Debug.Log(newAsset.transform.position);
            newAsset.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
        else
        {
            Debug.LogError("Terrain not found in the scene!");
        }
    }

    public void Update()
    {
        // Custom action on '1' key press
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.PlaceAssetsInCartesian(assetWidth, assetDepth);
        }

        // Custom action on '2' key presspress
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.PlaceAssetsInPolar();
        }
    }
}

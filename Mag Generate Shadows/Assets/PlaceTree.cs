using UnityEngine;

public class PlaceTree : MonoBehaviour
{
    public GameObject assetPrefab;
    public string parentName = "trees";
    public int objectQuantity = 100;
    public float xLimitB = 130f;
    public float xLimitU = 150f;
    public float zLimitB = 130f;
    public float zLimitU = 150f;

    // Start is called before the first frame update
    void Start()
    {
        // Find existing parent GameObject by name
        GameObject parent = GameObject.Find(parentName);
        //Debug.Log(parent.name);
        // If the parent doesn't exist, create it
        if (parent == null)
        {
            parent = new GameObject(parentName);
        }
        else
        {
            // Clear existing children of the parent GameObject
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        GameObject newAsset = Instantiate(assetPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, parent.transform);

        float assetWidth = newAsset.GetComponent<Renderer>().bounds.size.x;
        float assetDepth = newAsset.GetComponent<Renderer>().bounds.size.z;
        //Debug.Log("Asset size x: " + assetWidth + " z: " + assetDepth);

        Destroy(newAsset);

        for (float x = xLimitB; x < xLimitU; x += Random.Range(assetWidth * 0.05f, assetWidth * 0.25f))
        {
            for (float z = zLimitB; z < zLimitU; z += Random.Range(assetDepth * 0.05f, assetDepth * 0.25f))
            {
                float xtmp = Random.Range(assetWidth * -0.1f, assetWidth * 0.1f);
                //Debug.Log("Place an asset x: "+ xtmp + " z: "+z);
                this.PlaceAsset(parent, x + xtmp, z);
            }
        }
    }

    void PlaceAsset(GameObject parent, float xPosition, float zPosition)
    {
        // Check if the terrain exists
        if (Terrain.activeTerrain != null)
        {
            // Instantiate the asset as a child of the parent
            float yPosition = Terrain.activeTerrain.SampleHeight(new Vector3(xPosition, 0f, zPosition));
            GameObject newAsset = Instantiate(assetPrefab, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent.transform);
            //Debug.Log(newAsset.transform.position);
            newAsset.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
        else
        {
            Debug.LogError("Terrain not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

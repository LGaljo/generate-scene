using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public Texture2D[] textures;
    public int idx = 0;
    public Terrain terrain;
    public Material terrainMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain component not found!");
            terrainMaterial.mainTexture = this.textures[idx];
        }
        else
        {
            Debug.Log("Terrain found, continue...");
        }
    }

    public int ChangeTerrainMaterial()
    {
        int retval = this.idx + 1;
        idx = (idx + 1) % this.textures.Length;
        TerrainData terrainData = terrain.terrainData;

        // For currently used terrain material which has shader hdrp/lit
        if (terrainMaterial != null)
        {
            terrainMaterial.mainTexture = this.textures[idx];
        }

        // Check if there is at least one terrain layer
        if (terrainData.terrainLayers.Length > 0)
        {
            // Modify the texture of the first terrain layer
            terrainData.terrainLayers[0].diffuseTexture = this.textures[idx];

            // Refresh the terrain to apply changes
            terrain.Flush();
        }
        else
        {
            Debug.LogError("No terrain layers found!");
        }

        return retval;
    }

    public string GetTerrainLayerName(int layer)
    {
        // Check if there is at least one terrain layer
        if (terrain.terrainData.terrainLayers.Length > 0)
        {
            return terrain.terrainData.terrainLayers[layer].diffuseTexture.name;
        }

        return "";
    }

    // Update is called once per frame
    void Update()
    {
        // Custom action on '1' key press
        if (Input.GetKeyDown(KeyCode.T) && terrain != null)
        {
            Debug.Log("Change terrain texture");
            this.ChangeTerrainMaterial();
        }
    }
}

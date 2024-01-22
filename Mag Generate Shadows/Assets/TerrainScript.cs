using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainScript : MonoBehaviour
{
    public Texture2D[] textures;
    public int idx = 0;
    public Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain component not found!");
        }
        else
        {
            Debug.Log("Terrain found, continue...");
        }
    }

    public void ChangeTerrainMaterial()
    {
        idx = (idx + 1) % this.textures.Length;
        TerrainData terrainData = terrain.terrainData;

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

        //TerrainLayer terrainLayer = new TerrainLayer();
        //terrainLayer.diffuseTexture = this.textures[idx];
        //terrainLayer.tileSize = new Vector2(300f, 300f);
        //terrainLayer.tileOffset = new Vector2(550f, 550f);

        //terrain.terrainData.terrainLayers = new TerrainLayer[] { terrainLayer };
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

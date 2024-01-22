using UnityEngine;

public class ModifyShaders : MonoBehaviour
{
    private bool isUnlit = false;

    public void OnButtonClick()
    {
        GameObject models = GameObject.Find("trees");

        // Check if the object was found
        if (models != null)
        {
            // Iterate over object in folder models
            foreach (Transform gameObject in models.transform)
            {
                // Change shader of that object
                this.ChangeShaderObject(gameObject);

                // Check for any children objects in that oobject
                foreach (Transform child in gameObject.transform)
                {
                    // And change its shader too
                    this.ChangeShaderObject(child);
                }
            }
        }

        if (this.isUnlit)
        {
            RenderSettings.sun.shadows = LightShadows.Soft;
        }
        else
        {
            RenderSettings.sun.shadows = LightShadows.None;
        }

        this.isUnlit = !this.isUnlit;
    }

    private void ChangeShaderObject(Transform gameObject)
    {
        Shader unlitTransparentShader = Shader.Find("Unlit/Transparent");
        Shader unlitTextureShader = Shader.Find("Unlit/Texture");
        Shader litShader = Shader.Find("HDRP/Lit");


        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log("GameObject " + gameObject.name + " has shader: " + renderer.material.shader.name + " and material " + renderer.material.mainTexture.name);
            if (this.isUnlit)
            {
                renderer.material.shader = litShader;
            }
            else
            {
                if (gameObject.name.StartsWith("House"))
                {
                    renderer.material.shader = unlitTextureShader;
                }
                else
                {
                    renderer.material.shader = unlitTransparentShader;
                    
                }
            }
            
            // Change material shaders in game object
            foreach (Material mat in renderer.materials)
            {
                this.ChangeShaderMaterial(gameObject, mat);
            }
        }
    }

    private void ChangeShaderMaterial(Transform gameObject, Material mat)
    {
        Shader unlitTransparentShader = Shader.Find("Unlit/Transparent");
        Shader unlitTextureShader = Shader.Find("Unlit/Texture");
        Shader litShader = Shader.Find("HDRP/Lit");

        Debug.Log("Material " + mat.name + " has shader " + mat.shader.name);
 
        if (this.isUnlit)
        {
            mat.shader = litShader;
        }
        else
        {
            if (gameObject.name.StartsWith("House"))
            {
                mat.shader = unlitTextureShader;
            }
            else
            {
                mat.shader = unlitTransparentShader;
            }
        }
    }


    public void Update()
    {
        // Custom action on 'U' key press
        if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject gameObject = GameObject.Find("Button");
            ModifyShaders ms = gameObject.GetComponent<ModifyShaders>();
            ms.OnButtonClick();
        }
    }
}

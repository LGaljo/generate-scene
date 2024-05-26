using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePreviewModels : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

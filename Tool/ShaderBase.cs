using UnityEngine;
using UnityEngine.UI;

public class ShaderBase : MonoBehaviour {
    protected Material rendererMaterial {
        get {
            if (GetComponent<Renderer>() != null) {
                return GetComponent<Renderer>().sharedMaterial;
            }  
            if (GetComponent<Image>() != null) {
                return GetComponent<Image>().material;
            }

            return null;
        }
        set {
            if (GetComponent<Renderer>() != null) {
                GetComponent<Renderer>().sharedMaterial = value;
            } else if (GetComponent<Image>() != null) {
                GetComponent<Image>().material = value;
            }
        }
    }
}
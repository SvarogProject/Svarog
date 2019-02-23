using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class _moiling_2dxFX_BaseClass : MonoBehaviour
{
    protected Material rendererMaterial
    {
        get
        {
            if (this.GetComponent<Renderer>() != null)
            {
                return this.GetComponent<Renderer>().material;
            }
            else if (this.GetComponent<Image>() != null)
            {
                return GetComponent<Image>().material;
            }
            return null;
        }
        set
        {
            if (this.GetComponent<Renderer>() != null)
            {
                this.GetComponent<Renderer>().material = value;
            }
            else if (this.GetComponent<Image>() != null)
            {
                this.GetComponent<Image>().material = value;
            }
        }
    }

}



using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[ExecuteInEditMode]
[AddComponentMenu("MOILING/Sprites/ColorChange")]
[System.Serializable]
public class ChangeColor : ShaderBase {
    [HideInInspector] public Material ForceMaterial;
    [HideInInspector] public bool ActiveChange = true;

    private const string Shader = "Sprites/ColorChange";

    [HideInInspector] [Range(0, 1)] public float _Alpha = 1f;
    [HideInInspector] [Range(0, 1)] public float _Tolerance = 1f;
    [HideInInspector] [Range(0, 360)] public float _HueShift = 180f;
    [HideInInspector] [Range(-2, 2)] public float _Saturation = 1f;
    [HideInInspector] [Range(-2, 2)] public float _ValueBrightness = 1f;
    [HideInInspector] public Color _Color = new Color(0f, 1f, 1f, 1f);
    
    [HideInInspector] [Range(0, 1)] public float _Alpha2 = 1f;
    [HideInInspector] [Range(0, 1)] public float _Tolerance2 = 1f;
    [HideInInspector] [Range(0, 360)] public float _HueShift2 = 180f;
    [HideInInspector] [Range(-2, 2)] public float _Saturation2 = 1f;
    [HideInInspector] [Range(-2, 2)] public float _ValueBrightness2 = 1f;
    [HideInInspector] public Color _Color2 = new Color(0f, 1f, 1f, 1f);
    
    [HideInInspector] [Range(0, 1)] public float _Alpha3 = 1f;
    [HideInInspector] [Range(0, 1)] public float _Tolerance3 = 1f;
    [HideInInspector] [Range(0, 360)] public float _HueShift3 = 180f;
    [HideInInspector] [Range(-2, 2)] public float _Saturation3 = 1f;
    [HideInInspector] [Range(-2, 2)] public float _ValueBrightness3 = 1f;
    [HideInInspector] public Color _Color3 = new Color(0f, 1f, 1f, 1f);

    [HideInInspector] public int ShaderChange;

    Material tempMaterial;
    Material defaultMaterial;


    void Start() {
        ShaderChange = 0;
    }

    public void CallUpdate() {
        Update();
    }

    void Update() {
        if ((ShaderChange == 0) && (ForceMaterial != null)) {
            ShaderChange = 1;
            if (tempMaterial != null) DestroyImmediate(tempMaterial);
            rendererMaterial = ForceMaterial;
            ForceMaterial.hideFlags = HideFlags.None;
            ForceMaterial.shader = UnityEngine.Shader.Find(Shader);


        }

        if ((ForceMaterial == null) && (ShaderChange == 1)) {
            if (tempMaterial != null) DestroyImmediate(tempMaterial);
            tempMaterial = new Material(UnityEngine.Shader.Find(Shader));
            tempMaterial.hideFlags = HideFlags.None;
            rendererMaterial = tempMaterial;
            ShaderChange = 0;
        }

#if UNITY_EDITOR
        if (rendererMaterial.shader.name == "Sprites/Default") {
            if (ForceMaterial != null) {
                ForceMaterial.shader = UnityEngine.Shader.Find(Shader);
                ForceMaterial.hideFlags = HideFlags.None;
                rendererMaterial = ForceMaterial;
            }

        }
#endif
        if (ActiveChange) {
            // Color 1
            rendererMaterial.SetFloat("_Alpha", 1 - _Alpha);
            rendererMaterial.SetColor("_ColorX", _Color);
            rendererMaterial.SetFloat("_Tolerance", _Tolerance);
            rendererMaterial.SetFloat("_HueShift", _HueShift);
            rendererMaterial.SetFloat("_Sat", _Saturation);
            rendererMaterial.SetFloat("_Val", _ValueBrightness);

            // Color 2
            rendererMaterial.SetFloat("_Alpha2", 1 - _Alpha2);
            rendererMaterial.SetColor("_ColorX2", _Color2);
            rendererMaterial.SetFloat("_Tolerance2", _Tolerance2);
            rendererMaterial.SetFloat("_HueShift2", _HueShift2);
            rendererMaterial.SetFloat("_Sat2", _Saturation2);
            rendererMaterial.SetFloat("_Val2", _ValueBrightness2);
            
            // Color 3
            rendererMaterial.SetFloat("_Alpha3", 1 - _Alpha3);
            rendererMaterial.SetColor("_ColorX3", _Color3);
            rendererMaterial.SetFloat("_Tolerance3", _Tolerance3);
            rendererMaterial.SetFloat("_HueShift3", _HueShift3);
            rendererMaterial.SetFloat("_Sat3", _Saturation3);
            rendererMaterial.SetFloat("_Val3", _ValueBrightness3);
        }

    }

    void OnDestroy() {
        if (Application.isPlaying == false && Application.isEditor) {

            if (tempMaterial != null) DestroyImmediate(tempMaterial);

            if (gameObject.activeSelf && defaultMaterial != null) {
                rendererMaterial = defaultMaterial;
                rendererMaterial.hideFlags = HideFlags.None;
            }
        }
    }

    void OnDisable() {
        if (gameObject.activeSelf && defaultMaterial != null) {
            rendererMaterial = defaultMaterial;
            rendererMaterial.hideFlags = HideFlags.None;
        }
    }

    void OnEnable() {
        if (defaultMaterial == null) {
            defaultMaterial = new Material(UnityEngine.Shader.Find("Sprites/Default"));


        }

        if (ForceMaterial == null) {
            ActiveChange = true;
            tempMaterial = new Material(UnityEngine.Shader.Find(Shader));
            tempMaterial.hideFlags = HideFlags.None;
            rendererMaterial = tempMaterial;
        } else {
            ForceMaterial.shader = UnityEngine.Shader.Find(Shader);
            ForceMaterial.hideFlags = HideFlags.None;
            rendererMaterial = ForceMaterial;
        }

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ChangeColor)), CanEditMultipleObjects]
public class ChangeColor_Editor : Editor {
    private SerializedObject m_object;

    public void OnEnable() {
        m_object = new SerializedObject(targets);
    }

    public override void OnInspectorGUI() {
        m_object.Update();
        DrawDefaultInspector();

        ChangeColor _2dxScript = (ChangeColor) target;

//        Texture2D icon = Resources.Load("2dxfxinspector") as Texture2D;
//
//        if (icon) {
//            Rect r;
//            float ih = icon.height;
//            float iw = icon.width;
//            float result = ih / iw;
//            float w = Screen.width;
//            result = result * w;
//            r = GUILayoutUtility.GetRect(ih, result);
//            EditorGUI.DrawTextureTransparent(r, icon);
//        }

        EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"),
            new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));

        if (_2dxScript.ForceMaterial == null) {
            _2dxScript.ActiveChange = true;
        } else {
            if (GUILayout.Button("Remove Shared Material")) {
                _2dxScript.ForceMaterial = null;
                _2dxScript.ShaderChange = 1;
                _2dxScript.ActiveChange = true;
                _2dxScript.CallUpdate();
            }

            EditorGUILayout.PropertyField(m_object.FindProperty("ActiveChange"),
                new GUIContent("Change Material Property", "Change The Material Property"));
        }

        if (_2dxScript.ActiveChange) {

            EditorGUILayout.BeginVertical("Box");


            Texture2D icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_HueShift"),
                new GUIContent("Change Hue", icone, "Change the color from the Selected Color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Color"),
                new GUIContent("Selected Color", icone, "Pick up a color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Tolerance"),
                new GUIContent("Tolerance", icone, "the tolerance of the selected color"));

            icone = Resources.Load("2dxfx-icon-contrast") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Saturation"),
                new GUIContent("Color Saturation", icone, "Change the saturation"));

            icone = Resources.Load("2dxfx-icon-brightness") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_ValueBrightness"),
                new GUIContent("Brighntess", icone, "Change the brightness"));


            EditorGUILayout.BeginVertical("Box");

            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"),
                new GUIContent("Fading", icone, "Fade from nothing to showing"));
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            // 2
            
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(m_object.FindProperty("_HueShift2"),
                new GUIContent("Change Hue 2", icone, "Change the color from the Selected Color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Color2"),
                new GUIContent("Selected Color 2", icone, "Pick up a color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Tolerance2"),
                new GUIContent("Tolerance 2", icone, "the tolerance of the selected color"));

            icone = Resources.Load("2dxfx-icon-contrast") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Saturation2"),
                new GUIContent("Color Saturation 2", icone, "Change the saturation"));

            icone = Resources.Load("2dxfx-icon-brightness") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_ValueBrightness2"),
                new GUIContent("Brighntess 2", icone, "Change the brightness"));


            EditorGUILayout.BeginVertical("Box");

            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha2"),
                new GUIContent("Fading 2", icone, "Fade from nothing to showing"));
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            
            // 3
            
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(m_object.FindProperty("_HueShift3"),
                new GUIContent("Change Hue 3", icone, "Change the color from the Selected Color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Color3"),
                new GUIContent("Selected Color 3", icone, "Pick up a color"));

            icone = Resources.Load("2dxfx-icon-color") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Tolerance3"),
                new GUIContent("Tolerance 3", icone, "the tolerance of the selected color"));

            icone = Resources.Load("2dxfx-icon-contrast") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Saturation3"),
                new GUIContent("Color Saturation 3", icone, "Change the saturation"));

            icone = Resources.Load("2dxfx-icon-brightness") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_ValueBrightness3"),
                new GUIContent("Brighntess 3", icone, "Change the brightness"));


            EditorGUILayout.BeginVertical("Box");

            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha3"),
                new GUIContent("Fading 3", icone, "Fade from nothing to showing"));
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();


        }

        m_object.ApplyModifiedProperties();

    }
}
#endif
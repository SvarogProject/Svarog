﻿//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2015 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/Twist")]
[System.Serializable]
public class _2dxFX_Twist : _moiling_2dxFX_BaseClass
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/Twist";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;
	
	[HideInInspector] [Range(-1, 1)] public float Distortion = 1.6f;
	[HideInInspector] [Range(-1, 2)] public float _PosX = 0.5f;
	[HideInInspector] [Range(-1, 2)] public float _PosY = 0.5f;
	[HideInInspector] public Color _ColorX = new Color (1f, 1f, 1f, 1f);

	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;
	
	Material defaultMaterial;

	void Start ()
	{ 
		ShaderChange = 0;
	}
	
	public void CallUpdate()
	{
		Update ();
	}
	
	void Update()
	{	
		if ((ShaderChange == 0) && (ForceMaterial != null)) 
		{
			ShaderChange=1;
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			rendererMaterial = ForceMaterial;
			ForceMaterial.hideFlags = HideFlags.None;
			ForceMaterial.shader=Shader.Find(shader);
		

		}
		if ((ForceMaterial == null) && (ShaderChange==1))
		{
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			rendererMaterial = tempMaterial;
			ShaderChange=0;
		}
		
		#if UNITY_EDITOR
		if (rendererMaterial.shader.name == "Sprites/Default")
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
		}
		#endif
		if (ActiveChange)
		{
			rendererMaterial.SetFloat("_Alpha", 1-_Alpha);
			
			rendererMaterial.SetFloat("_Distortion", Distortion);
			rendererMaterial.SetFloat("_PosX", _PosX);
			rendererMaterial.SetFloat("_PosY", _PosY);
			rendererMaterial.SetColor("_ColorX", _ColorX);

		}
		
	}
	
	void OnDestroy()
	{
		if ((Application.isPlaying == false) && (Application.isEditor == true)) {
			
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			
			if (gameObject.activeSelf && defaultMaterial!=null) {
				rendererMaterial = defaultMaterial;
				rendererMaterial.hideFlags = HideFlags.None;
			}
		}
	}
	void OnDisable()
	{ 
		if (gameObject.activeSelf && defaultMaterial!=null) {
			rendererMaterial = defaultMaterial;
			rendererMaterial.hideFlags = HideFlags.None;
		}		
	}
	
	void OnEnable()
	{
		if (defaultMaterial == null) {
			defaultMaterial = new Material(Shader.Find("Sprites/Default"));
			 
			
		}
		if (ForceMaterial==null)
		{
			ActiveChange=true;
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			rendererMaterial = tempMaterial;
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
		}
		
	}
}




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Twist)),CanEditMultipleObjects]
public class _2dxFX_Twist_Editor : Editor
{
	private SerializedObject m_object;
	
	public void OnEnable()
	{
		m_object = new SerializedObject(targets);
	}
	
	public override void OnInspectorGUI()
	{
		m_object.Update();
		DrawDefaultInspector();
		
		_2dxFX_Twist _2dxScript = (_2dxFX_Twist)target;
		
		Texture2D icon = Resources.Load ("2dxfxinspector") as Texture2D;
		if (icon)
		{
			Rect r;
			float ih=icon.height;
			float iw=icon.width;
			float result=ih/iw;
			float w=Screen.width;
			result=result*w;
			r = GUILayoutUtility.GetRect(ih, result);
			EditorGUI.DrawTextureTransparent(r,icon);
		}
		
		EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));
		
		if (_2dxScript.ForceMaterial == null)
		{
			_2dxScript.ActiveChange = true;
		}
		else
		{
			if(GUILayout.Button("Remove Shared Material"))
			{
				_2dxScript.ForceMaterial= null;
				_2dxScript.ShaderChange = 1;
				_2dxScript.ActiveChange = true;
				_2dxScript.CallUpdate();
			}
			
			EditorGUILayout.PropertyField (m_object.FindProperty ("ActiveChange"), new GUIContent ("Change Material Property", "Change The Material Property"));
		}
		
		if (_2dxScript.ActiveChange)
		{
			
			EditorGUILayout.BeginVertical("Box");
			
			Texture2D icone = Resources.Load ("2dxfx-icon-distortion") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Distortion"), new GUIContent("Distortion value", icone, "Change the distortion value"));
			icone = Resources.Load ("2dxfx-icon-size_x") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_PosX"), new GUIContent("Position X", icone, "Change the horizontal position"));
			icone = Resources.Load ("2dxfx-icon-size_y") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_PosY"), new GUIContent("Position Y", icone, "Change the vertical position"));
			icone = Resources.Load ("2dxfx-icon-color") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ColorX"), new GUIContent("Change the color", icone, "Change the color"));

			EditorGUILayout.BeginVertical("Box");
			
		
			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			
			
		}
		
		m_object.ApplyModifiedProperties();
		
	}
}
#endif
using UnityEngine;
using UnityEditor;

public class ChangeMaterialSetting : ScriptableObject
{	
	[MenuItem ("EditorScript/Material/Change Shader Type/Diffuse")]
    static void ChangeShaderType_Diffuse()
	{
		ChangeShaderType("Diffuse");
	}
	
	[MenuItem ("EditorScript/Material/Change Shader Type/Transparent Diffuse")]
    static void ChangeShaderType_Transparent_Diffuse()
	{
		ChangeShaderType("Transparent/Diffuse");
	}
	
	[MenuItem ("EditorScript/Material/Change Shader Type/Unlit Texture")]
    static void ChangeShaderType_Unlit_Texture()
	{
		ChangeShaderType("Unlit/Texture");
	}
	
	[MenuItem ("EditorScript/Material/Change Shader Type/Unlit Transparent")]
    static void ChangeShaderType_Unlit_Transparent()
	{
		ChangeShaderType("Unlit/Transparent");
	}
	
	// ----------------------------------------------------------------------------
	
	static void ChangeShaderType(string shaderName)
	{
		Object[] materials = GetSelectedMaterials();
		Selection.objects = new Object[0];
		foreach (Material material in materials)  {
			material.shader = Shader.Find(shaderName);
		}
	}
	
	static Object[] GetSelectedMaterials() 
	{ 
		return Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
	}
}

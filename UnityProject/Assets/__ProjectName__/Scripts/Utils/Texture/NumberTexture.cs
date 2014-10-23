using UnityEngine;
using System.Collections;
using System;

/*
 * 入力された0-9の数値からテクスチャを設定して切り替える
 */

public class NumberTexture : MonoBehaviour
{	
	//数字テクスチャ0-9
	public Texture2D[] numTextures = new Texture2D[10];
	
	//一の位のプレートから順に
	public GameObject[] numPlates;
	
	
	/// <summary>
	/// 桁合わせしてテクスチャをセットする
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void SetNumber(int num)
	{
		string numString = String.Format("{0:D" + numPlates.Length.ToString() + "}", num);
		
		for(int i=0; i<numString.Length; i++)
		{
			numPlates[numString.Length - 1 - i].renderer.material.mainTexture = numTextures[int.Parse(numString[i].ToString())];
		}
	}
}

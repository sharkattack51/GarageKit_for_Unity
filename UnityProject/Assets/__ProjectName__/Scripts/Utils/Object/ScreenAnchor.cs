using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Screen上の基準位置を固定する
/// </summary>

public class ScreenAnchor : MonoBehaviour
{
	//画面横方向位置設定
	public enum ANCHOR_POSITION_HORIZONTAL
	{
		LEFT = 0,
		MIDDLE,
		RIGHT
	}
	public ANCHOR_POSITION_HORIZONTAL anchorPosH;
	
	//画面縦方向位置設定
	public enum ANCHOR_POSITION_VERTICAL
	{
		TOP = 0,
		MIDDLE,
		BOTTOM
	}
	public ANCHOR_POSITION_VERTICAL anchorPosV;
	
	//固定処理をUpdate()で行う
	public bool useUpdateAnchor = false;
	
	
	void Awake()
	{

	}
	
	void Start()
	{
		Anchor();
	}
	
	void Update()
	{
		if(useUpdateAnchor)
			Anchor();
	}
	
	
	private void Anchor()
	{
		//横
		float horizontal = 0.0f;
		switch(anchorPosH)
		{
			case ANCHOR_POSITION_HORIZONTAL.LEFT: horizontal = 0.0f; break;
			case ANCHOR_POSITION_HORIZONTAL.MIDDLE: horizontal = 0.5f; break;
			case ANCHOR_POSITION_HORIZONTAL.RIGHT: horizontal = 1.0f; break;
			default: break;
		}
		
		//縦
		float vertical = 0.0f;
		switch(anchorPosV)
		{
			case ANCHOR_POSITION_VERTICAL.TOP: vertical = 1.0f; break;
			case ANCHOR_POSITION_VERTICAL.MIDDLE: vertical = 0.5f; break;
			case ANCHOR_POSITION_VERTICAL.BOTTOM: vertical = 0.0f; break;
			default: break;
		}
		
		Vector3 worldPos = Utils.CameraUtil.FindCameraForLayer(this.gameObject.layer).ViewportToWorldPoint(new Vector3(horizontal, vertical, 0.0f));
		this.gameObject.transform.position = new Vector3(worldPos.x, worldPos.y, this.gameObject.transform.position.z);
	}
}

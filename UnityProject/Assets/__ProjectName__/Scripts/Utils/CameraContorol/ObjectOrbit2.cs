using UnityEngine;
using System.Collections;

/*
 * ドラッグ操作でのカメラのオービット回転コントロールクラス2
 * マウス&タッチ対応
 */

public class ObjectOrbit2 : MonoBehaviour
{
	public static bool win7touch = false;
	
	//singleton
	private static ObjectOrbit2 instance;
	public static ObjectOrbit2 Instance { get{ return instance; } }
	
	public Camera renderCam;
	public float sensitivity = 1.0f;
	public float smoothTime = 0.1f;
	public bool invertRot = true;
	
	public bool collidRadiusFromBounds = true;
	public float collidRadius = 0.0f;
	private float scaledRadius;
	
	private bool inputLock;
	public bool IsInputLock { get{ return inputLock; } }
	private object lockObject;
	
	private Vector3 currentTouchScrPos = Vector3.zero;
	private Vector3 oldTouchScrPos = Vector3.zero;
	private Vector3 oldHitPointDir = Vector3.zero;
	private Vector3 targetRotAxis = Vector3.zero;
	private float targetRotDeg = 0.0f;
	
	private float velocitySmoothRot = 0.0f;
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		inputLock = false;
		
		//レンダリングするカメラを設定
		if(renderCam == null)
			renderCam = Camera.main;
		
		//コライダを設定する
		if(this.gameObject.collider == null)
		{
			this.gameObject.AddComponent<SphereCollider>();
		}
		else if(this.gameObject.collider.GetType() != typeof(SphereCollider))
		{
			Collider.Destroy(this.gameObject.collider);
			this.gameObject.AddComponent<SphereCollider>();
		}
		
		float scale =  Mathf.Max(
			new float[]{
				this.gameObject.transform.localScale.x,
				this.gameObject.transform.localScale.y,
				this.gameObject.transform.localScale.z	
			});
		
		if(collidRadiusFromBounds)
		{
			collidRadius = Mathf.Max(
				Vector3.Distance(this.gameObject.transform.position, GetRenderBoundsChildren(this.gameObject).max),
				Vector3.Distance(this.gameObject.transform.position, GetRenderBoundsChildren(this.gameObject).min)
			);
		}
		scaledRadius = collidRadius / scale;
		(this.gameObject.collider as SphereCollider).radius = scaledRadius;
		
		//カメラ位置の警告
		if(Vector3.Distance(renderCam.transform.position, this.gameObject.transform.position) < (this.gameObject.collider as SphereCollider).radius)
			Debug.LogWarning("ObjectOrbit2 :: RenderCam position is inside of the collision.");
	}
	
	void Update() 
	{
		if(!inputLock && ButtonObjectBase.PressBtnsTotal == 0)
			GetInput();
		else
			ResetInput();
		
		HitTest();
		UpdateOrbit();
	}
	
	private void ResetInput()
	{
		currentTouchScrPos = Vector3.zero;
		oldTouchScrPos = Vector3.zero;
		oldHitPointDir = Vector3.zero;
	}
	
	private void GetInput()
	{
		//for Touch
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			if(Input.touchCount > 0)
				currentTouchScrPos = Input.GetTouch(0).position;
			else
				ResetInput();
		}

#if UNITY_STANDALONE_WIN
		else if(Application.platform == RuntimePlatform.WindowsPlayer && win7touch)
		{
			if(W7TouchManager.GetTouchCount() > 0)
				currentTouchScrPos = W7TouchManager.GetTouch(0).Position;
			else
				ResetInput();
		}
#endif
		
		//for Mouse
		else
		{
			if(Input.GetMouseButton(0))
				currentTouchScrPos = Input.mousePosition;
			else
				ResetInput();
		}
	}
	
	/// <summary>
	/// Input更新のLock
	/// </summary>
	public void LockInput(object sender)
	{
		if(!inputLock)
		{
			lockObject = sender;
			inputLock = true;
		}
	}
	
	/// <summary>
	/// Input更新のUnLock
	/// </summary>
	public void UnlockInput(object sender)
	{
		if(inputLock && lockObject == sender)
			inputLock = false;
	}
	
	/// <summary>
	/// Hit判定
	/// </summary>
	private void HitTest()
	{
		if(oldTouchScrPos != Vector3.zero)
		{
			//ドラッグ量が小さい場合は無視
			if(Vector3.Distance(oldTouchScrPos, currentTouchScrPos) < 1.5f)
			{
				ResetInput();
				return;
			}
			
			Ray ray = renderCam.ScreenPointToRay(currentTouchScrPos);
			RaycastHit hitInfo;
			if(this.gameObject.collider.Raycast(ray, out hitInfo, float.PositiveInfinity))
			{
				//Hit座標をローカル座標変換する
				Vector3 localMatPt = this.gameObject.transform.worldToLocalMatrix.MultiplyPoint(hitInfo.point);
				
				//方向に変換
				Vector3 newHitPointDir = localMatPt - this.gameObject.transform.position;
				
				//ドラッグを回転に変換
				if(oldHitPointDir != Vector3.zero)
				{
					//回転軸
					targetRotAxis = Vector3.Cross(newHitPointDir, oldHitPointDir);
					
					//回転量
					targetRotDeg = Vector3.Angle(newHitPointDir, oldHitPointDir) * sensitivity;
					if(invertRot)
						targetRotDeg *= -1.0f;
				}
				
				//ドラッグ検出用に方向を保存
				oldHitPointDir = newHitPointDir;
			}
		}
		
		//ドラッグ閾値判定用にタッチ位置を保存
		oldTouchScrPos = currentTouchScrPos;
	}
	
	/// <summary>
	/// 回転の更新
	/// </summary>
	private void UpdateOrbit()
	{
		//回転量をDump
		targetRotDeg = Mathf.SmoothDamp(targetRotDeg, 0.0f, ref velocitySmoothRot, smoothTime);
		
		//更新
		this.gameObject.transform.rotation *= Quaternion.AngleAxis(targetRotDeg, targetRotAxis);
	}
	
	/// <summary>
	/// ギズモ表示
	/// </summary>
	void OnDrawGizmos()
	{
	    Gizmos.color = Color.yellow;
	    Gizmos.DrawWireSphere(this.gameObject.transform.position, collidRadius);
	}
	
	/// <summary>
	/// 階層のRenderBoundsを取得する
	/// </summary>
	/// <param name="root">
	/// A <see cref="GameObject"/>
	/// </param>
	private Bounds GetRenderBoundsChildren(GameObject root)
	{
		Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
	
		//rendererが無い場合は無効
		if(renderers.Length == 0)
			return new Bounds(Vector3.zero, Vector3.zero);
		
		Vector3 vertexPosMax = renderers[0].bounds.max;
		Vector3 vertexPosMin = renderers[0].bounds.min;
		
		foreach(Renderer rndr in renderers)
		{
			if(rndr.enabled)
			{
				//maxを比較
				vertexPosMax = Vector3.Max(vertexPosMax, rndr.bounds.max);
				
				//minを比較
				vertexPosMin = Vector3.Min(vertexPosMin, rndr.bounds.min);
			}
		}
		
		Vector3 center = (vertexPosMax + vertexPosMin) * 0.5f;
		Vector3 size = vertexPosMax - vertexPosMin;
		
		return new Bounds(center, size);
	}
}

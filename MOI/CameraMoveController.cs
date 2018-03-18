using System;
using UnityEngine;
using System.Collections;

public class CameraMoveController : MonoBehaviour
{

	public const float SMALL_MODE_MAX_X = 10f;
	public const float SMALL_MODE_MIN_X = -10f;
	public const float SMALL_MODE_MAX_Y = 5f;
	public const float SMALL_MODE_MIN_Y = 0f;
	public const float LARGE_MODE_MAX_X = 6.67f;
	public const float LARGE_MODE_MIN_X = -6.47f;
	public const float LARGE_MODE_MAX_Y = 2.11f;
	public const float LARGE_MODE_MIN_Y = 9.42f;
		
	public GameObject Player;
	public GameObject Player2;
	public float FollowSpeed;

	private float _maxX = SMALL_MODE_MAX_X;
	private float _minX = SMALL_MODE_MIN_X;
	private float _maxY = SMALL_MODE_MAX_Y;
	private float _minY = SMALL_MODE_MIN_Y;
	private Camera _camera;
	private CameraSizeType _type = CameraSizeType.SMALL;

	private void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	void FixedUpdate ()
	{
		var distance2PlayersX = Player.transform.position.x - Player2.transform.position.x;
		var distance2PlayersY = Player.transform.position.y - Player2.transform.position.y;
		ChangeCameraSizeType((Player.transform.position - Player2.transform.position).magnitude);
		
		if (_type == CameraSizeType.LARGE && _camera.orthographicSize < 7)
		{
			_camera.orthographicSize += 0.15f;
			//_deltaPositon += Vector2.up * 0.1f;
		}
		if (_type == CameraSizeType.SMALL && _camera.orthographicSize > 5)
		{
			_camera.orthographicSize -= 0.1f;
			//_deltaPositon -= Vector2.up * 0.1f;
		}
				
				
		var destination = new Vector3
		(
			Mathf.Clamp((Player.transform.position.x + Player2.transform.position.x) / 2, _minX, _maxX),
			Mathf.Clamp((Player.transform.position.y + Player2.transform.position.y) / 2, _minY, _maxY),
			transform.position.z
		);

		//平滑移动(线性插值)
		var positionNow = Vector3.Lerp(transform.position, destination, FollowSpeed * Time.deltaTime);

		transform.position = new Vector3(positionNow.x, positionNow.y, transform.position.z);
	
	}

	private void ChangeCameraSizeType(float distance)
	{
		if (distance > 10 && _type == CameraSizeType.SMALL)
		{
			// TODO 应该是跟随着角色的距离慢慢变动
			_type = CameraSizeType.LARGE;
			_maxX = LARGE_MODE_MAX_X;
			_minX = LARGE_MODE_MIN_X;
			_minY = LARGE_MODE_MAX_Y;
			_maxY = LARGE_MODE_MIN_Y;
		}
		else if (distance < 8 && _type == CameraSizeType.LARGE)
		{	
			// TODO 有个延时，如果几秒都在这个范围才缩小
			_type = CameraSizeType.SMALL;
			_maxX = SMALL_MODE_MAX_X;
			_minX = SMALL_MODE_MIN_X;
			_minY = SMALL_MODE_MAX_Y;
			_maxY = SMALL_MODE_MIN_Y;
		}
	}
}

public enum CameraSizeType
{
	SMALL,
	LARGE
}

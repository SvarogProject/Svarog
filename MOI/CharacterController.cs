using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{

	public List<string> AnimNameList = new List<string>();
	private Animator _animator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// TODO 我想要取消掉延迟，但想了下这个方法不可行
	public void ControlAnimBool(string animName)
	{
		foreach (var name in AnimNameList)
		{
			if (name == animName)
			{
				_animator.SetBool(name, true);
			}
			else
			{
				_animator.SetBool(name, false);
			}
		}
	}
}

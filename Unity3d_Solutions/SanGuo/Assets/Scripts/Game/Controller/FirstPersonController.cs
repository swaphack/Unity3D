﻿using System;
using UnityEngine;

namespace Game.Controller
{
	/// <summary>
	/// 第一人称
	/// 挂在物体上
	/// </summary>
	public class FirstPersonController : MonoBehaviour
	{
		public FirstPersonController ()
		{
		}

		void Start()
		{
			Camera mainCamera = Camera.main;
			mainCamera.transform.SetParent(null);
			mainCamera.transform.SetParent (transform);
			mainCamera.transform.localPosition = Vector3.zero;
		}

		void Update()
		{
		}
	}
}

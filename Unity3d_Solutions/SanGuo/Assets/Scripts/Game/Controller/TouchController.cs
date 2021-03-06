﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Game.Platform;
using Game.Helper;

namespace Game.Controller
{
	/// <summary>
	/// 触摸监听
	/// </summary>
	public class TouchController : MonoBehaviour 
	{
		/// <summary>
		/// 点击协议
		/// </summary>
		private TouchProtocol _TouchProtocol;

		public TouchController()
		{
			_TouchProtocol = new TouchProtocol ();
		}

		void Start () 
		{
			_TouchProtocol.Target = this.GetComponent<Collider>();
			_TouchProtocol.IsTouchEnable = true;
			_TouchProtocol.OnTouchBegan = OnTouchBegan;
			_TouchProtocol.OnTouchMoved = OnTouchMoved;
			_TouchProtocol.OnTouchEnded = OnTouchEnded;
		}

		void Update () 
		{

		}

		protected void OnTouchBegan(Vector2 vector)
		{
			Log.Info ("touch me");
		}

		protected void OnTouchMoved(Vector2 vector)
		{
			Log.Info ("move touch");
		}

		protected void OnTouchEnded(Vector2 vector)
		{
			Log.Info ("leave touch");
		}

		void OnDestory()
		{
			_TouchProtocol.IsTouchEnable = false;
		}
	}
}


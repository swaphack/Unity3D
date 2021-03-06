﻿using System;
using UnityEngine;

namespace Game.Platform
{
	/// <summary>
	/// 点击派发器
	/// </summary>
	public interface ITouchDispatcher
	{
		/// <summary>
		/// 点击对象
		/// </summary>
		/// <value>The target.</value>
		Collider Target { get; set; }

		/// <summary>
		/// 点击生效
		/// </summary>
		/// <param name="state">点击状态</param>
		/// <param name="vector">点击点</param>
		void OnDispatchTouch (TouchPhase state, Vector2 vector);
	}

	/// <summary>
	/// 点击回调
	/// </summary>
	public delegate void OnTouchCallBack(Vector2 vector);
}


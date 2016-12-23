﻿using System;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// 点击监听
	/// </summary>
	public abstract class TouchListener :  MonoBehaviour, ITouchDispatcher
	{
		/// <summary>
		/// 是否可点击
		/// </summary>
		private bool _TouchEnabled = false;
		/// <summary>
		/// 目标
		/// </summary>
		protected Collider _Target = null;

		/// <summary>
		/// 获取对象
		/// </summary>
		/// <value>The target.</value>
		public Collider Target {
			set { 
				_Target = value; 
			}
			get { 
				return _Target; 
			}
		}

		public TouchListener ()
		{
		}

		/// <summary>
		/// 是否可点击
		/// </summary>
		/// <value><c>true</c> if this instance is touch enable; otherwise, <c>false</c>.</value>
		public bool IsTouchEnable {
			set {
				if (value == _TouchEnabled) {
					return;
				}
				_TouchEnabled = value;
				if (_TouchEnabled == true) {
					GameInstance.GetInstance ().Touch.AddDispatcher (this);
				} else {
					GameInstance.GetInstance ().Touch.RemoveDispatcher (this);
				}

			}
			get { 
				return _TouchEnabled;
			}
		}

		/// <summary>
		/// 点击生效
		/// </summary>
		/// <param name="state">点击状态</param>
		/// <param name="vector">点击点</param>
		public void OnDispatchTouch (TouchPhase state, Vector2 vector)
		{
			if (state == TouchPhase.Began) {
				OnTouchBegan (vector);
			} else if (state == TouchPhase.Moved) {
				OnTouchMoved (vector);
			} else if (state == TouchPhase.Ended) {
				OnTouchEnded (vector);
			}
		}

		/// <summary>
		/// 点击开始
		/// </summary>
		/// <param name="vector">Vector.</param>
		protected abstract void OnTouchBegan(Vector2 vector);

		/// <summary>
		/// 滑动
		/// </summary>
		/// <param name="vector">Vector.</param>
		protected abstract void OnTouchMoved(Vector2 vector);

		/// <summary>
		/// 点击离开
		/// </summary>
		/// <param name="vector">Vector.</param>
		protected abstract void OnTouchEnded(Vector2 vector);
	}
}

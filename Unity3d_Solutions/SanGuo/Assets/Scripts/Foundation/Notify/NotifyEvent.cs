﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Notify
{
	/// <summary>
	/// 推送监听器
	/// </summary>
	public class NotifyEvent
	{
		/// <summary>
		/// 无参数监听事件
		/// </summary>
		private List<NotifyHandler> _NotifyEvent;
		/// <summary>
		/// 有参数监听事件
		/// </summary>
		private List<NotifyHandlerWithParameter> _NotifyEventWithParameter;

		public NotifyEvent()
		{
			_NotifyEvent = new List<NotifyHandler> ();
			_NotifyEventWithParameter = new List<NotifyHandlerWithParameter> ();
		}

		/// <summary>
		/// 添加推送处理
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void AddNotify(NotifyHandler handler)
		{
			if (handler != null) {
				_NotifyEvent.Add (handler);
			}
		}

		/// <summary>
		/// 移除推送处理
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void RemoveNotify(NotifyHandler handler)
		{
			if (handler != null) {
				_NotifyEvent.Remove (handler);
			}
		}

		/// <summary>
		/// 派送
		/// </summary>
		public void Dispatch()
		{
			int count = _NotifyEvent.Count;
			for (int i = 0; i < count; i++) {
				_NotifyEvent [i] ();
			}
		}

		/// <summary>
		/// 添加推送处理
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void AddNotify(NotifyHandlerWithParameter handler)
		{
			if (handler != null) {
				_NotifyEventWithParameter.Add (handler);
			}
		}

		/// <summary>
		/// 移除推送处理
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void RemoveNotify(NotifyHandlerWithParameter handler)
		{
			if (handler != null) {
				_NotifyEventWithParameter.Remove (handler);
			}
		}

		/// <summary>
		/// 派送
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void Dispatch(object parameter)
		{
			int count = _NotifyEventWithParameter.Count;
			for (int i = 0; i < count; i++) {
				_NotifyEventWithParameter[i] (parameter);
			}
		}
	}
}


﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Helper;

namespace Controller.Battle
{
	/// <summary>
	/// 地图
	/// </summary>
	public class Map
	{  
		/// <summary>
		/// 根节点
		/// </summary>
		private Transform _Root;
		/// <summary>
		/// 资源包
		/// </summary>
		private List<string> _AssetBundles;
		/// <summary>
		/// 已加载资源包数量
		/// </summary>
		private int _LoadAssetBundleCount;
		/// <summary>
		/// 加载资源包索引
		/// </summary>
		private int _LoadAssetBundleCursor;

		/// <summary>
		/// 根节点
		/// </summary>
		/// <value>The root.</value>
		public Transform Root {
			get { 
				return _Root;
			}
			set { 
				_Root = value;
			}
		}

		public Map ()
		{
			_AssetBundles = new List<string> ();
		}

		/// <summary>
		/// 添加要加载的资源包
		/// </summary>
		/// <param name="filepath">Filepath.</param>
		public void AddAssetBundle(string filepath)
		{
			if (string.IsNullOrEmpty (filepath) == true) {
				return;
			}

			_AssetBundles.Add (filepath);
		}

		/// <summary>
		/// 是否正在加载资源包
		/// </summary>
		/// <returns><c>true</c>, if asset bundle was loaded, <c>false</c> otherwise.</returns>
		public bool LoadAssetBundle()
		{
			if (_LoadAssetBundleCount >= _AssetBundles.Count) {
				return false;
			}

			if (_LoadAssetBundleCursor >= _AssetBundles.Count) {
				return true;
			}

			string filePath = _AssetBundles [_LoadAssetBundleCursor];

			FileDataHelp.LoadAssetBundle(filePath, (bool value)=> {
				_LoadAssetBundleCount++;
			});
			_LoadAssetBundleCursor++;

			return true;
		}

		/// <summary>
		/// 添加对象
		/// </summary>
		/// <param name="transform">Transform.</param>
		public void AddTransform(Transform transform)
		{
			if (transform == null || _Root == null) {
				return;
			}

			transform.SetParent (_Root);
		}


		/// <summary>
		/// 移除对象
		/// </summary>
		/// <param name="transform">Transform.</param>
		public void RemoveTransform(Transform transform)
		{
			if (transform == null) {
				return;
			}

			transform.SetParent (null);

			Utility.Destory (transform.gameObject);
		}

		/// <summary>
		/// 销毁地图
		/// </summary>
		public void Dispose()
		{
			_AssetBundles.Clear ();
			if (_Root == null) {
				return;
			}

			_Root.SetParent (null);
			Utility.Destory (_Root.gameObject);
			_Root = null;
		}
	}
}


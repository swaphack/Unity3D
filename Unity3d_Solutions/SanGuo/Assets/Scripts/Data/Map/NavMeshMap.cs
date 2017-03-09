﻿using System;
using System.Xml;
using System.Collections.Generic;
using Game.Helper;

namespace Data.Map
{
	/// <summary>
	/// 导航网格地图
	/// </summary>
	public class NavMeshMap
	{
		/// <summary>
		/// 点
		/// </summary>
		public struct Point
		{
			/// <summary>
			/// x轴值
			/// </summary>
			public int X;
			/// <summary>
			/// y轴值
			/// </summary>
			public int Y;
		}

		/// <summary>
		/// 网格
		/// </summary>
		public struct Mesh
		{
			/// <summary>
			/// 顶点
			/// </summary>
			public List<int> Vertices;
		}

		/// <summary>
		/// 关联
		/// </summary>
		public struct Link
		{
			/// <summary>
			/// 第一点
			/// </summary>
			public int Point1;
			/// <summary>
			/// 第二点
			/// </summary>
			public int Point2;
			/// <summary>
			/// 距离
			/// </summary>
			public float Distance;
		}

		/// <summary>
		/// 网格
		/// </summary>
		private Dictionary<int, Point> _Points;
		/// <summary>
		/// 关联
		/// </summary>
		private Dictionary<int, Mesh> _Meshes;

		/// <summary>
		/// 关联
		/// </summary>
		private Dictionary<int, Link> _Links;


		/// <summary>
		/// 配置所在路径
		/// </summary>
		private string _ConfigPath;

		/// <summary>
		/// 点
		/// </summary>
		/// <value>The points.</value>
		public Dictionary<int, Point> Points {
			get { 
				return _Points;
			}
		}

		/// <summary>
		/// 关联
		/// </summary>
		/// <value>The links.</value>
		public Dictionary<int, Mesh> Meshes {
			get { 
				return _Meshes;
			}
		}

		/// <summary>
		/// 关联
		/// </summary>
		/// <value>The links.</value>
		public Dictionary<int, Link> Links {
			get { 
				return _Links;
			}
		}

		/// <summary>
		/// 配置所在路径
		/// </summary>
		public string ConfigPath { 
			get { 
				return _ConfigPath;
			}
		}

		public NavMeshMap (string configpath)
		{
			_ConfigPath = configpath;
			_Points = new Dictionary<int, Point> ();
			_Meshes = new Dictionary<int, Mesh> ();
			_Links = new Dictionary<int, Link> ();
		}

		/// <summary>
		/// 加载点
		/// </summary>
		/// <param name="node">Node.</param>
		private void LoadPoint(XmlNode node)
		{
			if (node == null) {
				return;
			}

			XmlElement element = (XmlElement)node;
			XmlNode itemNode = node.FirstChild;
			while (itemNode != null) {
				element = (XmlElement)itemNode;
				int id = Int32.Parse(element.GetAttribute ("ID"));
				Point item = new Point();
				item.X = Int32.Parse(element.GetAttribute ("X"));
				item.Y = Int32.Parse (element.GetAttribute ("Y"));
				_Points [id] = item;
				itemNode = itemNode.NextSibling;
			}
		}

		/// <summary>
		/// 加载网格
		/// </summary>
		/// <param name="node">Node.</param>
		private void LoadMesh(XmlNode node)
		{
			if (node == null) {
				return;
			}

			XmlElement element = (XmlElement)node;
			XmlNode itemNode = node.FirstChild;
			while (itemNode != null) {
				element = (XmlElement)itemNode;
				int id = Int32.Parse(element.GetAttribute ("ID"));
				Mesh item = new Mesh();
				item.Vertices = StringHelp.ConvertToIntArray(element.GetAttribute ("Vertices"));
				_Meshes [id] = item;
				itemNode = itemNode.NextSibling;
			}
		}

		/// <summary>
		/// 加载关联
		/// </summary>
		/// <param name="node">Node.</param>
		private void LoadLink(XmlNode node)
		{
			if (node == null) {
				return;
			}

			XmlElement element = (XmlElement)node;
			XmlNode itemNode = node.FirstChild;
			while (itemNode != null) {
				element = (XmlElement)itemNode;
				int id = Int32.Parse(element.GetAttribute ("ID"));
				Link item = new Link();
				item.Point1 = Int32.Parse(element.GetAttribute ("Point1"));
				item.Point2 = Int32.Parse (element.GetAttribute ("Point2"));
				item.Distance = float.Parse (element.GetAttribute ("Distance"));
				_Links [id] = item;
				itemNode = itemNode.NextSibling;
			}
		}


		/// <summary>
		/// 加载
		/// </summary>
		public bool Load ()
		{
			XmlNode node = XmlHelp.LoadXMlRoot (ConfigPath);
			if (node == null) {
				return false;
			}

			this.Clear ();

			while (node != null) {
				if (node.Name == "Points") {
					LoadPoint (node);
				} else if (node.Name == "Meshes") {
					LoadMesh (node);
				}else if (node.Name == "Links") {
					LoadLink (node);
				}

				node = node.NextSibling;
			}

			return true;
		}
		/// <summary>
		/// 清空
		/// </summary>
		public void Clear ()
		{
			_Points.Clear ();
			_Meshes.Clear ();
			_Links.Clear ();
		}
	}
}


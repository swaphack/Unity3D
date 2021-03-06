﻿using System;
using System.Collections.Generic;
using Model.Battle;
using Game.Helper;
using UnityEngine;
using Controller.Battle.Terrain;
using Controller.AI.Movement;

namespace Controller.Battle
{
	/// <summary>
	/// 战场
	/// </summary>
	public class Field : IDisposable
	{
		/// <summary>
		/// 存活的队伍
		/// </summary>
		private Dictionary<int, Team> _AliveTeams;
		/// <summary>
		/// 死亡的队伍
		/// </summary>
		private Dictionary<int, Team> _DeadTeams;
		/// <summary>
		/// 待移除队伍
		/// </summary>
		private List<Team> _WaitForRemoveTeams;
		/// <summary>
		/// 进行时间
		/// </summary>
		private int _Time;
		/// <summary>
		/// 是否正在运行中
		/// </summary>
		private bool _Running;
		/// <summary>
		/// 地图加载管理
		/// </summary>
		private MapLoader _MapLoader;
		/// <summary>
		/// 地图
		/// </summary>
		private Map _Map;
		/// <summary>
		/// 交通管理
		/// </summary>
		private Traffic _Traffic;
		/// <summary>
		/// 是否是模拟
		/// </summary>
		private bool _IsSimulate;

		/// <summary>
		/// 存活的队伍
		/// </summary>
		public Dictionary<int, Team> AliveTeams {
			get { 
				return _AliveTeams;
			}
		}

		/// <summary>
		/// 死亡的队伍
		/// </summary>
		public Dictionary<int, Team> DeadTeams {
			get { 
				return _DeadTeams;
			}
		}

		/// <summary>
		/// 地图加载
		/// </summary>
		/// <value>The map.</value>
		public MapLoader MapLoader {
			get { 
				return _MapLoader;
			}
		}

		/// <summary>
		/// 地图
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get { 
				return _Map;
			}
		}

		/// <summary>
		/// 是否是模拟，不考虑队伍生死
		/// </summary>
		/// <value><c>true</c> if this instance is simulate; otherwise, <c>false</c>.</value>
		public bool IsSimulate {
			get { 
				return _IsSimulate;
			}
			set { 
				_IsSimulate = value;
			}
		}

		/// <summary>
		/// 开始战斗
		/// </summary>
		//public event OnBattleBroadcast OnBeginBattle;
		/// <summary>
		/// 结束战斗
		/// </summary>
		//public event OnBattleBroadcast OnEndBattle;

		public Field ()
		{
			_AliveTeams = new Dictionary<int, Team> ();
			_DeadTeams = new Dictionary<int, Team> ();
			_MapLoader = new MapLoader ();

			_Map = new Map ();

			_Traffic = new Traffic ();

			_WaitForRemoveTeams = new List<Team> ();

			_IsSimulate = true;
		}

		/// <summary>
		/// 添加队伍
		/// </summary>
		/// <param name="team">Team.</param>
		public void AddTeam(Team team)
		{
			if (team == null) {
				return;
			}

			_AliveTeams.Add (team.ID, team);

			team.OnDestory += OnTeamDestory;
			team.OnUnitCreate += OnCreateUnit;
			team.OnUnitDestory += OnDestoryUnit;
		}

		/// <summary>
		/// 查找队伍
		/// </summary>
		/// <returns>The team.</returns>
		/// <param name="teamID">Team I.</param>
		public Team GetTeam(int teamID)
		{
			if (_AliveTeams.ContainsKey (teamID)) {
				return _AliveTeams [teamID];
			}

			if (_DeadTeams.ContainsKey (teamID)) {
				return _DeadTeams [teamID];
			}

			return null;
		}

		/// <summary>
		/// 获取活着的队伍
		/// </summary>
		/// <returns>The alive team.</returns>
		/// <param name="teamID">Team I.</param>
		public Team GetAliveTeam(int teamID)
		{
			if (_AliveTeams.ContainsKey (teamID)) {
				return _AliveTeams [teamID];
			}

			return null;
		}

		/// <summary>
		///  获取死亡的队伍
		/// </summary>
		/// <returns>The dead team.</returns>
		/// <param name="teamID">Team I.</param>
		public Team GetDeadTeam(int teamID)
		{
			if (_DeadTeams.ContainsKey (teamID)) {
				return _DeadTeams [teamID];
			}

			return null;
		}

		/// <summary>
		/// 获取其他非存活的队伍
		/// </summary>
		/// <returns>The other alive team.</returns>
		/// <param name="teamID">Team I.</param>
		public List<Team> GetOtherAliveTeam(int teamID)
		{
			List<Team> teams = new List<Team> ();
			foreach (KeyValuePair<int,Team> item in _AliveTeams) {
				if (item.Key != teamID) {
					teams.Add (item.Value);
				}
			}

			return teams;
		}

		/// <summary>
		/// 销毁队伍
		/// </summary>
		/// <param name="team">Team.</param>
		private void OnTeamDestory(Team team)
		{
			if (team == null) {
				return;
			}

			if (_WaitForRemoveTeams.Contains (team)) {
				return;
			}

			_WaitForRemoveTeams.Add (team);
		}

		/// <summary>
		/// 创建单位
		/// </summary>
		/// <param name="team">Unit.</param>
		private void OnCreateUnit(Unit unit)
		{
			if (unit == null || _Map == null) {
				return;
			}

			_Map.AddMemberTransform (unit.MemberTransform);
		}

		/// <summary>
		/// 销毁单位
		/// </summary>
		/// <param name="unit">Unit.</param>
		private void OnDestoryUnit(Unit unit)
		{
			if (unit == null) {
				return;
			}

			_Map.RemoveMemberTransform (unit.MemberTransform);
			unit.RestObject();
		}

		/// <summary>
		/// 开始战斗
		/// </summary>
		public void Start()
		{
			_Running = true;

			//OnBeginBattle ();
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Pause()
		{
			_Running = false;
		}

		/// <summary>
		/// 恢复
		/// </summary>
		public void Resume()
		{
			_Running = true;
		}

		/// <summary>
		/// 更新战斗
		/// </summary>
		/// <param name="dt">Dt.</param>
		public void Update(float dt)
		{
			if (!_Running) {
				return;
			}

			if (_Map == null) {
				return;
			}

			// 正在加载资源
			if (!IsFinishedLoadResource ()) {
				return;
			}

			// 初始化地图
			if (!_Map.Init ()) {
				return;
			}

			// 待移除的队伍
			HandWaitForRemoveTeams ();

			// 战斗结束
			if (!IsSimulate && CheckFinishBattle ()) {
				return;
			}

			// 将要移除的单位
			foreach (KeyValuePair<int,Team> item in _AliveTeams) {
				item.Value.HandWaitForRemoveUnits ();
			}

			// 死亡单位
			foreach (KeyValuePair<int,Team> item in _DeadTeams) {
				item.Value.UpdateDeadUnits (dt);
			}

			foreach (KeyValuePair<int,Team> item in _AliveTeams) {
				item.Value.UpdateDeadUnits (dt);
			}

			_Traffic.Update (dt);

			// 活着单位
			foreach (KeyValuePair<int,Team> item in _AliveTeams) {
				if (item.Value.MoveGroup.State == FormationState.Formed) { // 集结完毕
					item.Value.UpdateAliveUnits (dt);
				}
			}
		}

		/// <summary>
		/// 处理待移除队伍列表
		/// </summary>
		private void HandWaitForRemoveTeams()
		{
			if (_WaitForRemoveTeams.Count == 0) {
				return;
			}
			int count = _WaitForRemoveTeams.Count;
			for (int i =0; i< count; i++) {
				if (_AliveTeams.ContainsKey (_WaitForRemoveTeams[i].ID)) {
					_AliveTeams.Remove (_WaitForRemoveTeams[i].ID);
				}

				if (!_DeadTeams.ContainsKey (_WaitForRemoveTeams[i].ID)) {
					_DeadTeams.Add (_WaitForRemoveTeams[i].ID, _WaitForRemoveTeams[i]);
				}
			}

			_WaitForRemoveTeams.Clear ();
		}

		/// <summary>
		/// 检查战斗是否结束
		/// </summary>
		/// <returns><c>true</c>, if finish battle was checked, <c>false</c> otherwise.</returns>
		private bool CheckFinishBattle()
		{
			if (_AliveTeams.Count == 1) {
				foreach (KeyValuePair<int,Team> item in _AliveTeams) {
					foreach (KeyValuePair<int, Unit> item2 in item.Value.AliveUnits.Units) {
						item2.Value.UnitBehaviour.PlayWin ();
					}
				}

				Utility.UnloadUnusedObject ();
				return true;
			}

			return false;
		}

		/// <summary>
		/// 是否资源加载结束
		/// </summary>
		/// <returns><c>true</c> if this instance is finished load resource; otherwise, <c>false</c>.</returns>
		private bool IsFinishedLoadResource()
		{
			// 正在加载资源
			if (_MapLoader.LoadAssetBundle ()) {
				return false;
			}

			// 正在加载单位
			bool isLoaded = true;
			foreach (KeyValuePair<int,Team> item in _AliveTeams) {
				if (!item.Value.IsLoaded) {
					isLoaded = !item.Value.LoadUnits ();
				}/* else if (!item.Value.IsBuildUp) {
					item.Value.IsBuildUp = true;
					item.Value.MoveGroup.Concentrate (new Vector3 (10, 1, 10));
				}*/
			}

			return isLoaded;
		}

		/// <summary>
		/// 战斗是否结束
		/// </summary>
		/// <returns><c>true</c> if this instance is end battle; otherwise, <c>false</c>.</returns>
		private bool IsEndBattle()
		{
			if (_AliveTeams.Count > 1) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Controller.Battle.Field"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Controller.Battle.Field"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Controller.Battle.Field"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Controller.Battle.Field"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Controller.Battle.Field"/> was occupying.</remarks>
		public void Dispose()
		{
			_AliveTeams.Clear ();
			_DeadTeams.Clear ();
			_MapLoader.Dispose ();
			_Map.Dispose ();
		}
	}
}


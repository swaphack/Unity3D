﻿using System;
using Model.Base;

namespace Controller.Battle
{
	/// <summary>
	/// 战斗广播
	/// </summary>
	public delegate void OnBattleBroadcast();

	/// <summary>
	/// 队伍广播
	/// </summary>
	public delegate void OnTeamBroadcast(Team Team);

	/// <summary>
	/// 单位广播
	/// </summary>
	public delegate void OnUnitBroadcast(Unit unit);

	/// <summary>
	/// 动作广播
	/// </summary>
	public delegate void OnActionBroadcast(int tag);
	/// <summary>
	/// 单位动作广播
	/// </summary>
	public delegate void OnUnitActionBroadcast(Unit unit, int tag);
	/// <summary>
	/// 单位碰撞
	/// </summary>
	public delegate void OnUnitCollisonBroadcast(Unit src, Unit target);
}


﻿using UnityEngine;
using System.Collections.Generic;
using Model.Skill;
using Model.Base;
using Model.Battle;
using Controller.Battle;
using Controller.AI.Task;
using Game.Helper;
using Controller.Battle.AI;
using Controller.Battle.Task;
using Controller.Battle.Spell;

namespace Controller.Battle.AI
{
	/// <summary>
	/// 施法者
	/// </summary>
	public class SpellCaster : UnitTask
	{
		/// <summary>
		/// 当前正在执行的技能
		/// </summary>
		private SkillIndex _CurrentSkillIndex;
		/// <summary>
		/// 生效的技能
		/// </summary>
		private SkillModel _SkillModel;
		/// <summary>
		/// 目标
		/// </summary>
		private SelectedTarget _SelectedTarget;
		/// <summary>
		/// 是否正在施法
		/// </summary>
		private bool _bRunningSkill;

		public SpellCaster()
		{
			_CurrentSkillIndex = SkillIndex.Max;
		}

		/// <summary>
		/// 初始化任务
		/// </summary>
		/// <param name="task">Task.</param>
		protected override void InitTask(UnitTask task)
		{
			base.InitTask (task);

			_CurrentSkillIndex = SkillIndex.Max;
			_SkillModel = null;
			_SelectedTarget = null;
			_bRunningSkill = false;
		}

		/// <summary>
		/// 更新施法者
		/// </summary>
		/// <param name="dt">Dt.</param>
		public override void Update(float dt)
		{
			base.Update (dt);

			// 检查生效技能
			if (_CurrentSkillIndex == SkillIndex.Max) {
				if (!CheckEnableSkill()) {
					return;
				}	
			}
			// 查询技能数据
			if (_SkillModel == null) {
				_SkillModel = Src.Skill.GetSkillModel(_CurrentSkillIndex);
				if (_SkillModel == null) {
					IsFinish = true;
					return;
				}
			}

			// 查找技能施法目标
			if (_SelectedTarget == null) {
				_SelectedTarget = TargetSelectMechanism.Instance.FindTargets (TargetSelectType.Nearest, Src, Field);
				if (_SelectedTarget == null) {
					return;
				}
			}

			// 离自己最近的目标
			/*
			if (!_SelectedTarget.IsFirstTargetEnabled) {
				_SelectedTarget = null;
				return;
			}
			*/

			// 是否到达目标
			if (!CheckInSkillRadius ()) {
				if (Src.Walker.Empty){
					ResetMoveToTarget();
				}
				return;
			}

			if (_bRunningSkill) {
				return;
			}

			if (!_bRunningSkill && CheckCanPlayAttack()) {
				_bRunningSkill = true;
				// 施法
				Src.MemberModel.LookAt (_SelectedTarget.FirstTarget.MemberTransform.Position);
				Src.MemberModel.OnActionEnd += OnEndAction;
				Src.UnitBehaviour.PlayAttack ();

				Log.Warning ("Src : " + Src.ID + " Start Attack");
			}
		}

		/// <summary>
		/// 检查技能是否可用
		/// </summary>
		/// <returns><c>true</c> if this instance is skill enable the specified skillIndex; otherwise, <c>false</c>.</returns>
		/// <param name="skillIndex">Skill index.</param>
		private bool IsSkillEnable (SkillIndex skillIndex)
		{
			if (Src == null) {
				return false;
			}

			SkillModel skillModel = Src.Skill.GetSkillModel (skillIndex);
			if (skillModel == null) {
				return false;
			}

			UnitSkill.SkillValue skillValue = Src.Skill.GetSkillValue (skillIndex);
			if (skillValue == null) {
				return false;
			}

			bool bOk = false;
			do {
				// 禁止普通攻击？
				if (skillModel.IsNormalAttack && Src.Buff.HasType(BuffType.ForbiddenNormalAttack)) {
					break;
				}
				// 禁止魔法攻击？
				if (!skillModel.IsNormalAttack && Src.Buff.HasType(BuffType.ForbiddenSkillAttack)) {
					break;
				}
				// 冷却完毕？
				if (skillValue.CurrentValue > 0) {
					break;
				}
				// 魔法充足？
				if (Src.Property.GetValue(PropertyType.CurrentManaPoints) < skillModel.CostMana) {
					break;
				}

				bOk = true;

			} while(false);

			return bOk;
		}

		/// <summary>
		/// 检查可用技能
		/// </summary>
		/// <returns><c>true</c>, if enable skill was checked, <c>false</c> otherwise.</returns>
		private bool CheckEnableSkill()
		{
			if (_CurrentSkillIndex != SkillIndex.Max) {
				return true;
			}

			for (int i = (int)SkillIndex.NormalAttack; i < (int)SkillIndex.Max; i++) {
				if (IsSkillEnable ((SkillIndex)i)) {
					_CurrentSkillIndex = (SkillIndex)i;
					break;
				}
			}

			return _CurrentSkillIndex != SkillIndex.Max;
		}

		/// <summary>
		/// 是否在攻击范围内
		/// </summary>
		/// <returns><c>true</c>, if in skill radius was checked, <c>false</c> otherwise.</returns>
		private bool CheckInSkillRadius()
		{
			if (_SkillModel == null || _SelectedTarget == null || _SelectedTarget.FirstTarget == null) {
				return false;
			}

			float distance = Vector3.Distance (Src.MemberTransform.Position, _SelectedTarget.FirstTarget.MemberTransform.Position);
			if (_SkillModel.Radius < distance) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// 检查是否可以播放攻击
		/// </summary>
		/// <returns><c>true</c>, if can play attack was checked, <c>false</c> otherwise.</returns>
		private bool CheckCanPlayAttack()
		{
			if (Src.MemberModel.IsPlay (UnitAction.t_getHit)
			    || Src.MemberModel.IsPlay (UnitAction.t_die)) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// 动作播放完回调
		/// </summary>
		/// <param name="tag">Tag.</param>
		private void OnEndAction(int tag)
		{
			// 非攻击动作
			if (tag != UnitAction.t_attack_01
				&& tag != UnitAction.t_attack_02
				&& tag != UnitAction.t_attack_03) {
				return;
			}

			Log.Warning ("Src : " + Src.ID + " End Attack");
			Src.Skill.ResetSkillValue (_CurrentSkillIndex);
			IsFinish = true;


			if (_SelectedTarget != null && _SelectedTarget.Targets != null) {
				int count = _SelectedTarget.Targets.Count;
				for (int i = 0; i < count; i++) {
					Unit target = _SelectedTarget.Targets [i];
					if (!target.Property.Dead
						&& !target.MemberModel.IsPlay (UnitAction.t_attack_01)
						&& !target.MemberModel.IsPlay (UnitAction.t_attack_02)
						&& !target.MemberModel.IsPlay (UnitAction.t_attack_03)) {
						int idx = Random.Range (1, 10);
						if (idx == 1) {
							target.UnitBehaviour.PlaySimDead ();
						} else {
							target.UnitBehaviour.PlayGetHit ();
						}
					}
				}
			}
		}

		/// <summary>
		/// 重置移动目标
		/// </summary>
		private void ResetMoveToTarget() {
			if (_SelectedTarget == null || _SelectedTarget.FirstTarget == null) {
				return;
			}
			// 静止行走的buff
			if (Src.Buff.HasType (BuffType.ForbiddenWalk)) {
				return;
			}

			// 已死亡，重新选择目标
			if (_SelectedTarget.FirstTarget.Property.Dead) {
				_SelectedTarget.ResetFirstTarget();
				return;
			}

			List<Vector2> path = Field.Map.FindWay (
				Src.MemberTransform.Transform,
				_SelectedTarget.FirstTarget.MemberTransform.Transform);
			Src.Walker.Set3DWayBy2D (path, Src.MemberTransform.Position.y);
		}


		/// <summary>
		/// 是否包含目标
		/// </summary>
		/// <returns><c>true</c>, if target was contained, <c>false</c> otherwise.</returns>
		/// <param name="unit">Unit.</param>
		public bool ContainTarget(Unit unit) {
			if (unit == null) {
				return false;
			}

			if (_SelectedTarget == null) {
				return false;
			}

			if (_SelectedTarget.FirstTarget == unit 
				/*|| _SelectedTarget.Targets.Contains (unit)
				 */) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// 清空
		/// </summary>
		public override void Dispose()
		{
			base.Dispose ();
			Src.MemberModel.OnActionEnd -= OnEndAction;
		}
	}
}
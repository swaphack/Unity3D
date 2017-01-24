﻿using UnityEngine;
using UnityEngine.UI;
using System;
using Game;
using Control;

public class UIHomeLayer : Layer
{
	/// <summary>
	/// 食物标签
	/// </summary>
	private Text _LabelFood;
	/// <summary>
	/// 木料标签
	/// </summary>
	private Text _LabelWood;
	/// <summary>
	/// 矿石标签
	/// </summary>
	private Text _LabelIron;

	/// <summary>
	/// 食物按钮
	/// </summary>
	private Button _BtnFood;
	/// <summary>
	/// 木材按钮
	/// </summary>
	private Button _BtnWood;
	/// <summary>
	/// 矿石按钮
	/// </summary>
	private Button _BtnIron;

	public UIHomeLayer ()
	{
		
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	protected override void InitUI()
	{
		_LabelFood = FindCanvas<Text> ("Canvas.CanvasResource.CanvasFood.Text");
		_LabelWood = FindCanvas<Text> ("Canvas.CanvasResource.CanvasWood.Text");
		_LabelIron = FindCanvas<Text> ("Canvas.CanvasResource.CanvasIron.Text");

		_BtnFood = FindCanvas<Button> ("Canvas.CanvasButton.CanvasFood.Button");
		_BtnWood = FindCanvas<Button> ("Canvas.CanvasButton.CanvasWood.Button");
		_BtnIron = FindCanvas<Button> ("Canvas.CanvasButton.CanvasIron.Button");

		_BtnFood.onClick.AddListener(delegate() {
			Player.MainPlayer.Resource.Food += 1;

			ReqPacketLogin packet = PacketHelp.GetRequestPacket<ReqPacketLogin>();
			packet.Name = PacketHelp.GetByteText("1212", 15);
			packet.Password = PacketHelp.GetByteText("123", 20);
			PacketHelp.Send (packet);
		});

		_BtnWood.onClick.AddListener(delegate() {
			Player.MainPlayer.Resource.Wood += 1;
		});

		_BtnIron.onClick.AddListener(delegate() {
			Player.MainPlayer.Resource.Iron += 1;
		});

		Player.MainPlayer.Resource.AddChangedNotify (ResType.Food, () => {
			_LabelFood.text = Player.MainPlayer.Resource.Food.ToString();
			UserDefault.GetInstance ().Set ("Food", Player.MainPlayer.Resource.Food.ToString());
		});

		Player.MainPlayer.Resource.AddChangedNotify (ResType.Wood, () => {
			_LabelWood.text = Player.MainPlayer.Resource.Wood.ToString();
			UserDefault.GetInstance ().Set ("Wood", Player.MainPlayer.Resource.Food.ToString());
		});

		Player.MainPlayer.Resource.AddChangedNotify (ResType.Iron, () => {
			_LabelIron.text = Player.MainPlayer.Resource.Iron.ToString();
			UserDefault.GetInstance ().Set ("Iron", Player.MainPlayer.Resource.Food.ToString());
		});
	}

	/// <summary>
	/// 初始化文本
	/// </summary>
	protected override void InitText()
	{
		_LabelFood.text = Player.MainPlayer.Resource.Food.ToString();
		_LabelWood.text = Player.MainPlayer.Resource.Wood.ToString();
		_LabelIron.text = Player.MainPlayer.Resource.Iron.ToString();

		_BtnFood.GetComponentInChildren<Text>().text = GetLocalText (2);
		_BtnWood.GetComponentInChildren<Text>().text = GetLocalText (1);
		_BtnIron.GetComponentInChildren<Text>().text = GetLocalText (3);

		UserDefault.GetInstance ().Set ("Name", "LinGan");
		UserDefault.GetInstance ().Set ("Password", "123456");
	}

	/// <summary>
	/// 初始化报文监听
	/// </summary>
	protected override void InitPacketListeners()
	{
		PacketHelp.RegisterPacketHandler (PacketID.Login, OnReceivePacket_Login);
		PacketHelp.RegisterPacketHandler (PacketID.Error, OnReceivePacket_Error);
	}

	/// <summary>
	/// 初始化报文监听
	/// </summary>
	protected override void DisponsePacketListeners()
	{
		PacketHelp.UnregisterPacketHandler (PacketID.Login, OnReceivePacket_Login);
		PacketHelp.UnregisterPacketHandler (PacketID.Error, OnReceivePacket_Error);
	}

	private void OnReceivePacket_Login(byte[] bytes)
	{
		ReqPacketLogin packet = PacketHelp.GetResponsePacket<ReqPacketLogin> (bytes);
		Log.Info ("Player ID " + PacketHelp.GetStringText (packet.Name));
	}

	private void OnReceivePacket_Error(byte[] bytes)
	{
		RespPacketError packet = PacketHelp.GetResponsePacket<RespPacketError> (bytes);
		Log.Info ("Resp Error Packet, ID : %d" + packet.Header.PacketID);
	}

	/// <summary>
	/// 返回键处理
	/// </summary>
	protected override void OnEscapeHandler()
	{
		Log.Info ("Press Escape Key");
	}
}


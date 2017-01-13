using System;
using System.Text;
using Foundation.Net;
using System.Runtime.InteropServices;

/// <summary>
/// 登录请求
/// 0
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct RespPacketError: IPacket
{
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader Header { get { return PacketHeader; } set { PacketHeader = value; } }
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader PacketHeader;

	public void Init()
	{
		PacketHeader.PacketID = 0;
		PacketHeader.Length = Marshal.SizeOf (this);
	}
}

/// <summary>
/// 登录请求
/// 100
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct ReqPacketLogin: IPacket
{
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader Header { get { return PacketHeader; } set { PacketHeader = value; } }
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader PacketHeader;
	/// <summary>
	/// 用户名
	/// </summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst=15)]
	public byte[] Name;
	/// <summary>
	/// 密码
	/// </summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst=20)]
	public byte[] Password;

	public void Init()
	{
		PacketHeader.PacketID = 100;
		PacketHeader.Length = Marshal.SizeOf (this);
	}
}

/// <summary>
/// 返回玩家信息
/// 101
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct RespPacketPlayerInfo: IPacket
{
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader Header { get { return PacketHeader; } set { PacketHeader = value; } }
	/// <summary>
	/// Packet Header
	/// </summary>
	public PacketHeader PacketHeader;
	/// <summary>
	/// 玩家名称
	/// </summary>
	public int PlayerID;
	/// <summary>
	/// 玩家等级
	/// </summary>
	public int Level;
	/// <summary>
	/// 玩家经验
	/// </summary>
	public int Experience;

	public void Init()
	{
		PacketHeader.PacketID = 101;
		PacketHeader.Length = Marshal.SizeOf (this);
	}
}
namespace GCProject.Main.Protocol;

public class ProtocolDefinde
{
	public const byte GCProtocolRecv1To5 = 16;

	public const byte GCProtocolSend1To5 = 144;

	public const byte GCProtocolSend6 = 148;

	public const byte GCProtocolRecv6 = 20;

	public const byte GCProtocolBoardSend = 148;

	public const byte GCProtocolBoardRecv = 20;

	public static byte[] GCProtocolRecvCMDs = new byte[3] { 16, 20, 20 };

	public const byte GCProtocolHeader1 = 109;

	public const byte GCProtocolHeader2 = 115;

	public const byte GCPlcProtocolGetH20 = 3;

	public const int GCPlcProtocolGetH20Len = 8;

	public const byte GCPlcProtocolSortMulti = 16;

	public const int GCPlcProtocolSortMultiLen = 11;

	public const byte GCPlcProtocolSortSingle = 6;

	public const int GCPlcProtocolSortSingleLen = 8;
}

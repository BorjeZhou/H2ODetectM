namespace GCProject.Main.Protocol;

public class GCProtocolSend1To5 : IProtocol
{
	private byte data = 128;

	public override byte FunctionCode => 144;

	protected override byte[] BuildBodyData()
	{
		return new byte[1] { data };
	}
}

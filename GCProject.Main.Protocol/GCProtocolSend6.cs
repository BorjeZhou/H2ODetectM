namespace GCProject.Main.Protocol;

public class GCProtocolSend6 : IProtocol
{
	private byte data = 128;

	public override byte FunctionCode => 148;

	protected override byte[] BuildBodyData()
	{
		return new byte[1] { data };
	}
}

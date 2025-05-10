using System;
using System.Collections.Generic;

namespace GCProject.Main.Protocol;

public class ProtocolHelper
{
	public static IProtocol ExtractProtocol(List<byte[]> recvData)
	{
		IProtocol ResProtocol = null;
		try
		{
			foreach (byte[] recvDatum in recvData)
			{
				if (!string.IsNullOrEmpty(IProtocol.ExtractData(recvDatum, out var bodyData, out var functionCode)))
				{
					continue;
				}
				switch (functionCode)
				{
				case 16:
				{
					GCProtocolRecv1To5 pro = new GCProtocolRecv1To5(bodyData);
					if (ResProtocol == null)
					{
						ResProtocol = new GCProtocolRecv1To5Package();
					}
					(ResProtocol as GCProtocolRecv1To5Package)?.SetGCPRotocolRecv1To5(pro);
					break;
				}
				case 20:
					ResProtocol = new GCProtocolRecv6(bodyData);
					break;
				}
			}
		}
		catch (Exception)
		{
		}
		return ResProtocol;
	}
}

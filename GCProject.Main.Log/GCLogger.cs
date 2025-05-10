using System.Reflection;
using log4net;

namespace GCProject.Main.Log;

public class GCLogger
{
	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
}

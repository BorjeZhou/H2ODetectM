using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GCProject.Main.Resource;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Res
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("GCProject.Main.Resource.Res", typeof(Res).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static Icon AppLogo1 => (Icon)ResourceManager.GetObject("AppLogo1", resourceCulture);

	internal static Bitmap Car1 => (Bitmap)ResourceManager.GetObject("Car1", resourceCulture);

	internal static Bitmap Car2 => (Bitmap)ResourceManager.GetObject("Car2", resourceCulture);

	internal static Bitmap MainLogo => (Bitmap)ResourceManager.GetObject("MainLogo", resourceCulture);

	internal static Bitmap PMT9000 => (Bitmap)ResourceManager.GetObject("PMT9000", resourceCulture);

	internal static Bitmap PMT9000_Agent => (Bitmap)ResourceManager.GetObject("PMT9000_Agent", resourceCulture);

	internal Res()
	{
	}
}

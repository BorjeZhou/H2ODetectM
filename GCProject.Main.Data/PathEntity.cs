namespace GCProject.Main.Data;

public class PathEntity
{
	public string Path { get; set; }

	public string DisplayName { get; set; }

	public PathEntity(string dispalyName, string path)
	{
		DisplayName = dispalyName;
		Path = path;
	}
}

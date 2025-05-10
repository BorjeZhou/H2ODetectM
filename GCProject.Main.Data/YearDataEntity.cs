using System.Collections.Generic;

namespace GCProject.Main.Data;

public class YearDataEntity : PathEntity
{
	public List<MothDataEntity> MothDatas { get; set; } = new List<MothDataEntity>();


	public YearDataEntity(string dispalyName, string path)
		: base(dispalyName, path)
	{
	}
}

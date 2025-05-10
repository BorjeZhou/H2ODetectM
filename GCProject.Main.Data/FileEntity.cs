using System.Collections.Generic;

namespace GCProject.Main.Data;

public class FileEntity
{
	private List<DataEntity> _datas;

	public string DisplayName { get; set; }

	public string File { get; set; }

	public List<DataEntity> Datas
	{
		get
		{
			if (_datas == null)
			{
				Load();
			}
			return _datas;
		}
	}

	public FileEntity(string dispalyName, string file)
	{
		DisplayName = dispalyName;
		File = file;
	}

	public void Load()
	{
		_datas = new List<DataEntity>();
	}
}

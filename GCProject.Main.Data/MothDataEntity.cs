using System;
using System.Collections.Generic;
using System.IO;

namespace GCProject.Main.Data;

public class MothDataEntity : PathEntity
{
	private List<FileEntity> _fileEntities;

	public List<FileEntity> FileEntities
	{
		get
		{
			if (_fileEntities == null)
			{
				LoadFileEntities();
			}
			return _fileEntities;
		}
	}

	public MothDataEntity(string dispalyName, string path)
		: base(dispalyName, path)
	{
	}

	private void LoadFileEntities()
	{
		_fileEntities = new List<FileEntity>();
		string[] files = Directory.GetFiles(base.Path);
		foreach (string file in files)
		{
			if (file.EndsWith(".csv"))
			{
				string dispalyName = System.IO.Path.GetFileNameWithoutExtension(file).Replace("_", "-");
				if (DateTime.TryParse(dispalyName, out var _))
				{
					_fileEntities.Add(new FileEntity(dispalyName, file));
				}
			}
		}
	}
}

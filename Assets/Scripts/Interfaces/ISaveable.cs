using System;

namespace CardGameArchive
{
	public interface ISaveable
	{
		public SaveData Save();
		public void Load();
	}

}
using System;
using System.IO;
using CKAN;

namespace PartManagerPlugin
{
    public static class Cache
    {
		private static string PartManagerPath
	    {
		    get
		    {
			    var path = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), "PartManager");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
		    }
	    }

	    private static string CachePath
	    {
		    get
		    {
				var path = Path.Combine(PartManagerPath, "cache");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
		    }
	    }

        public static void RemovePartFromCache(string part)
        {
			var fullPath = Path.Combine(CachePath, part);
            File.Delete(fullPath);
        }

        public static void MovePartToCache(string part)
        {
	        var fullPath = Path.Combine(Main.Instance.CurrentInstance.GameDir(), part);
			var targetPath = Path.Combine(CachePath, part);

            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetPath));
            }
            catch (Exception) { }

            File.Move(fullPath, targetPath);
        }

        public static void MovePartFromCache(string part)
        {
			var fullPath = Path.Combine(CachePath, part);
            var targetPath = Path.Combine(Main.Instance.CurrentInstance.GameDir(), part);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            }
            catch (Exception) { }

            File.Move(fullPath, targetPath);
        }

    }
}

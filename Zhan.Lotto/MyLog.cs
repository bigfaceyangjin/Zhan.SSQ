using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Zhan.Lotto
{
	/// <summary>
	/// 日志类
	/// </summary>
	public class MyLog
	{
		public static void SetLog(string msg)
		{
			StreamWriter sw = null;
			try
			{
				string path = AppDomain.CurrentDomain.BaseDirectory;
				string url = Path.Combine(path, "log.txt");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				sw = File.AppendText(url);
				sw.WriteLine("{0} {1}", msg, DateTime.Now.ToString("yyyy--MM--dd HH:mm:ss"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally {
				sw.Flush();
				sw.Close();
				sw.Dispose();
			}
		}
	}
}

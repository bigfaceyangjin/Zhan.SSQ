using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Zhan.SSQ
{
	public class RandomHelper
	{
		/// <summary>
		/// 利用Guid作为种子生成随机数
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int GetRandom(int min,int max) 
		{
			int y = 0;
			Thread.Sleep(300);
			string guid= Guid.NewGuid().ToString();//唯一标识
			for (int i = 0; i < guid.Length; i++)
			{
				switch (guid[i])
				{
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
					case 'g':
						y += 1;
						break;
					case 'h':
					case 'i':
					case 'j':
					case 'k':
					case 'l':
					case 'm':
					case 'n':
						y += 2;
						break;
					case 'o':
					case 'p':
					case 'q':
					case 'r':
					case 's':
					case 't':
						y += 3;
						break;
					case 'u':
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case 'z':
						y += 4;
						break;
					default:
						y += 5;
						break;
				}
			}
			return new Random(y).Next(min, max);
		}
	}
}

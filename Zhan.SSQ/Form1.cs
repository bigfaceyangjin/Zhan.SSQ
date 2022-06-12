using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zhan.SSQ
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private string[] blueStrs = {"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15","16" };
		private string[] redStrs = {"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15","16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32","33" };
		private static readonly object SSQ = new object();
		/// <summary>
		/// 项目启动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_start_Click(object sender, EventArgs e)
		{
			try
			{
				this.btn_start.Text = "运行ing...";
				this.btn_start.Enabled = false;
				this.lbl_red1.Text = "00";
				this.lbl_red2.Text = "00";
				this.lbl_red3.Text = "00";
				this.lbl_red4.Text = "00";
				this.lbl_red5.Text = "00";
				this.lbl_red6.Text = "00";
				this.lbl_blue.Text = "00";
				TaskFactory taskFactory = new TaskFactory();
				//不允许出现重复数字 
				foreach (Control control in this.groupBox1.Controls)
				{
					if (control is Label)
					{
						Label lbl = (Label)control;
						if (control.Name.Contains("blue"))//蓝球
						{
							taskFactory.StartNew(() =>
							{
								while (true)
								{
									//获取号码下标
									int blueIndex = this.GetRandomLong(0, blueStrs.Length);
									//通过下标访问蓝球集合
									this.UpdateControl(lbl, this.blueStrs[blueIndex]);
								}
							});
						}
						//红球
						else {
							taskFactory.StartNew(()=> {
								while (true) {
									int redIndex = this.GetRandomLong(0, this.redStrs.Length);
									lock (SSQ)
									{
										//遍历其他五个球 如果有相同存在 就进入下一次循环
										if (this.IsExist(redStrs[redIndex]))
										{
											continue; 
										}
									}
									this.UpdateControl(lbl, this.redStrs[redIndex]);
								} 
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"双色球项目出现异常：{ex.Message}");
			}
		}

		/// <summary>
		/// 获取随机数
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int GetRandom(int min,int max) {
			Guid guid= Guid.NewGuid();//全球唯一
			string guidStr = guid.ToString();
			int Need = DateTime.Now.Millisecond;
			for (int i = 0; i < guidStr.Length; i++)
			{
				switch (guidStr[i]) {
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
					case 'g':
						Need += 1;
						break;
					case 'h':
					case 'i':
					case 'j':
					case 'k':
					case 'l':
					case 'm':
					case 'n':
						Need += 2;
						break;
					case 'o':
					case 'p':
					case 'q':
						Need += 3;
						break;
					case 'r':
					case 's':
					case 't':
					case 'u':
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case 'z':  
						Need += 4;
						break;
					default:
						Need += 5;
						break;
				}
			} 
			return new Random(Need).Next(min, max);
		}

		/// <summary>
		/// 将获取随机数延时
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int GetRandomLong(int min,int max) {
			Thread.Sleep(this.GetRandom(500,1000));
			return this.GetRandom(min,max);
		}
		/// <summary>
		/// 解决异步线程修改UI线程问题
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="name"></param>
		public void UpdateControl(Label lbl, string name)
		{
			if (lbl.InvokeRequired)
			{
				lbl.Invoke(new Action(() =>
				{
					lbl.Text = name;
				}));
			}
			else {
				lbl.Text = name;
			}
		}

		/// <summary>
		/// 判断是否存在重复球号
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsExist(string name) {
			foreach (Control control in this.groupBox1.Controls)
			{ 
				if (control is Label)
				{
					if (control.Text == name)
					{
						return true;//存在重复球号
					}
				}
			}
			return false;
		}
		private void btn_stop_Click(object sender, EventArgs e)
		{
			CancellationTokenSource cls = new CancellationTokenSource();
			cls.Cancel();
		}
	}
}

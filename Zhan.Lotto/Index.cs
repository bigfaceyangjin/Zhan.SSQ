using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;

namespace Zhan.Lotto
{
	public partial class Index : Form
	{
		public Index()
		{
			InitializeComponent();
		}
		//蓝球集合
		private string[] blueStrs = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"};
		//红球集合
		private string[] redStrs = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33","34","35" };
		//线程集合
		List<Task> taskList = new List<Task>(); 
		//常规锁
		private static readonly object dlt = new object();
		//公共变量 控制线程是否停止任务
		private bool IsGo = true;
		//音效全局变量
		SoundPlayer soundPlayer = new SoundPlayer();
		/// <summary>
		/// 开始摇号
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_start_Click(object sender, EventArgs e)
		{
			try
			{
				//音效
				soundPlayer.SoundLocation = Application.StartupPath+ @"\中国风仙侠道骨纯音乐_爱给网_aigei_com.wav";
				soundPlayer.Load();
				soundPlayer.Play();
				//恢复球号为00
				foreach (Control control in this.panel1.Controls)
				{
					control.Text = "00";
				}
				this.btn_start.Enabled = false;
				this.btn_start.Text = "ing...";
				this.IsGo = true;//默认执行任务
				this.taskList = new List<Task>();//清空task集合
				foreach (Control lbl in this.panel1.Controls)
				{
					//红球
					if (lbl.Name.Contains("red"))
					{
						this.OperationBall(lbl, redStrs, "red");
					}
					else//蓝球
					{
						this.OperationBall(lbl, blueStrs, "blue"); 
					}
				}
				//启动一个线程判断所有球号是否已经开始启动
				Task.Run(()=> {
					while (true)
					{
						if (!IsExistsRed("00", "red") && !IsExistsRed("00","blue"))
						{
							this.btn_end.Enabled = true;
							break;	
						}
					}
				});
				//等待任务完成再打印信息
				Task.WhenAll(taskList.ToArray()).ContinueWith(t => { this.ShowInfo(); });
			}
			catch (AggregateException ex) 
			{//线程异常专用捕获器
				foreach (var item in ex.InnerExceptions)
				{
					Console.WriteLine(item.Message);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			} 
		}

		/// <summary>
		/// 获取随机球号
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		private int GetRandom(int min, int max)
		{
			Thread.Sleep(300);//每次获取随机数前休息0.3s 否则数字变化太快了
			string guid = Guid.NewGuid().ToString();//唯一标识
			int y = 0;
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
		/// <summary>
		/// 将修改界面信息交给UI线程处理
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="num"></param>
		private void UpdateTxt(Control lbl, string num)
		{
			if (lbl.InvokeRequired)
			{
				this.Invoke(new Action(() =>
				{
					lbl.Text = num;
				}));
			}
			else
			{
				lbl.Text = num;
			}
		}
		/// <summary>
		/// 是否存在相同球号
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		private bool IsExistsRed(string num,string color)
		{
			foreach (Control lbl in this.panel1.Controls)
			{
				if (lbl.Text.Equals(num) && lbl.Name.Contains(color))//如果其他球号跟当前球号相同
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 异步操作球号
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="balls"></param>
		/// <param name="color"></param>
		private void OperationBall(Control lbl,string[] balls,string color) {
			taskList.Add(
							Task.Run(() => {
								while (this.IsGo)
								{
									string num = balls[GetRandom(0, balls.Length)];//随机球号
									lock (dlt)//只允许一个线程进来判断
									{
										if (this.IsExistsRed(num,color))//存在相同球号 跳出循环
										{
											continue;
										}
										this.UpdateTxt(lbl, num);//修改球号
									}
								}
							})
							);
		}
		/// <summary>
		/// 显示开奖信息
		/// </summary>
		private void ShowInfo()
		{
			MessageBox.Show(string.Format("红球：{0},{1},{2},{3},{4}\n蓝球：{5},{6}",lbl_red1.Text, lbl_red2.Text, lbl_red3.Text, lbl_red4.Text, lbl_red5.Text, lbl_blue1.Text, lbl_blue2.Text));
		}
		private void Index_Load(object sender, EventArgs e)
		{
			this.btn_end.Enabled = false;
		}

		private void btn_end_Click(object sender, EventArgs e)
		{
			this.btn_end.Enabled = false;
			this.btn_start.Enabled = true;
			this.IsGo = false;
			soundPlayer.Stop();
			//Task.Run(()=> {
			//	Task.WaitAll(taskList.ToArray());
			//	this.ShowInfo();
			//}); 
		}
	}
}

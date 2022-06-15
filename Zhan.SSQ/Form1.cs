using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
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
		//蓝球集合
		private string[] blueStrs = {"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15","16" };
		//红球集合
		private string[] redStrs = {"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15","16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32","33" };
		//修改球号锁
		private static readonly object SSQ = new object();
		//按钮状态
		private bool flag = true;
		//task线程集合
		private List<Task> taskList = new List<Task>();
		private SoundPlayer soundPlayer = new SoundPlayer();//只能播放wav文件
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
				this.taskList = new List<Task>();
				this.flag = true;
				this.lbl_red1.Text = "00";
				this.lbl_red2.Text = "00";
				this.lbl_red3.Text = "00";
				this.lbl_red4.Text = "00";
				this.lbl_red5.Text = "00";
				this.lbl_red6.Text = "00";
				this.lbl_blue.Text = "00";
				//正确时机打开Stop Enabled
				Task.Run(()=> {
					while (true)
					{
						Thread.Sleep(500);//休息一下再次判断
						if (!this.IsExsit("00") && !this.lbl_blue.Text.Equals("00"))//保证蓝球红球都不为00
						{
							//修改界面信息需要使用当前类实例的Invoke方法调用UI线程来修改
							this.Invoke(new Action(() =>
							{
								this.btn_stop.Enabled = true;
							}));
							break;//修改后记得跳出循环
						}
					}
				});
				TaskFactory taskFactory = new TaskFactory();
				//遍历所有控件 
				foreach (Control control in this.groupBox1.Controls)
				{ 
					if (control is Label && control.Name.Contains("blue"))//蓝球
					{
						taskList.Add(Task.Run(() => {
							while (flag)
							{
								Thread.Sleep(200);
								int index = RandomHelper.GetRandom(0, this.blueStrs.Length);
								//this.lbl_blue.Text = blueStrs[index];//子线程无法修改界面 这是主线程的事
								this.UpdateLblTxt(control, blueStrs[index]);
							}
						})); 
					}
					else
					{
						taskList.Add(taskFactory.StartNew(() => {
							while (flag)
							{
								Thread.Sleep(200);
								int index = RandomHelper.GetRandom(0, redStrs.Length);
								lock (SSQ)
								{
									if (this.IsExsit(redStrs[index]))
									{
										continue;//球号相同 跳出循环
									}
									this.UpdateLblTxt(control, redStrs[index]);
								}
							}
						}));
					}
				}
				//解决点击停止后显示数据不是界面实际数据问题 (因为当flag为false时 线程有可能还在继续他的最后一个任务)
				//方法一
				//Task.Run(()=> { 
				//		Task.WaitAll(taskList.ToArray());
				//	this.ShowResult();
				//});
				
				//方法二
				Task.WhenAll(taskList.ToArray()).ContinueWith(t => { this.ShowResult(); });

				//音效  
				soundPlayer.SoundLocation = string.Format(Application.StartupPath+ "中国风仙侠道骨纯音乐_爱给网_aigei_com.wav");
				soundPlayer.Load();
				soundPlayer.Play();//异步播放
			}
			catch (Exception ex)
			{
				Console.WriteLine($"双色球项目出现异常：{ex.Message}");
			}
		}
		/// <summary>
		/// 主线程修改界面信息
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="txt"></param>
		private void UpdateLblTxt(Control lbl,string txt) {
			if (lbl.InvokeRequired)//如果设置了 CheckForillegalCrossThreadCalls=false  属性就表示可以跨线程操作界面 默认不允许
			{
				//交给UI线程去处理
				if (flag) { 
					this.Invoke(new Action(() => {
						lbl.Text = txt;
					}));
				}
			}
			{
				lbl.Text = txt;
			}
		}

		/// <summary>
		/// 是否存在相同球号
		/// </summary>
		/// <param name="txt"></param>
		/// <returns></returns>
		private bool IsExsit(string txt) {
			foreach (Control control in this.groupBox1.Controls)
			{
				if (control is Label && control.Name.Contains("red"))
				{
					if (control.Text == txt)
					{
						return true;//存在相同球号
					}
				}
			}
			return false;//不存在
		}
		private void btn_stop_Click(object sender, EventArgs e)
		{
			this.flag = false;
			this.btn_stop.Enabled = false;
			this.btn_start.Enabled = true; 
			soundPlayer.Stop();
			//CancellationTokenSource cls = new CancellationTokenSource();
			//cls.Cancel();
		}
		/// <summary>
		/// 打印开奖结果信息
		/// </summary>
		private void ShowResult() {
			MessageBox.Show(string.Format("开奖号码为：{0} {1} {2} {3} {4} {5} {6}",this.lbl_red1.Text, this.lbl_red2.Text, this.lbl_red3.Text, this.lbl_red4.Text, this.lbl_red5.Text, this.lbl_red6.Text, this.lbl_blue.Text)) ;
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			this.btn_stop.Enabled = false;
		}
	}
}

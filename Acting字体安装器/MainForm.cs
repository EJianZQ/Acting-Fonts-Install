using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HZH_Controls;
using HZH_Controls.Forms;
using HZH_Controls.Controls;
using Newtonsoft.Json;
using System.Threading;
using System.IO;


namespace Acting字体安装器
{
    public partial class MainForm : FrmBase
    {
        public string cloudJson;
        public FontsData fontsData;
        public const int localVersion = 1;
        public MainForm()
        {
            InitializeComponent();
            //允许从线程间调用组件
            Control.CheckForIllegalCrossThreadCalls = false;

            //设置窗口的拖动
            this.InitFormMove(this); 
            this.InitFormMove(ucStep);
            this.InitFormMove(tabPage1);
            this.InitFormMove(tabPage2);

            //去除选项卡
            this.tabControl.Region = new Region(new RectangleF(this.tabPage1.Left, this.tabPage1.Top, this.tabPage1.Width, this.tabPage1.Height));
            cloudJson = GetData.Get("https://download.3tme.cn/FontsInstall/data.json");//获取数据

            if (cloudJson == "Error" || cloudJson == null)
            {
                FrmDialog.ShowDialog(this, "安装器无法获取云端数据，部分功能将不可用\n\n已启用离线模式，不可用的功能不会影响字体安装\n\n不可用功能：在线教程与版本更新", "Acting字体安装器");
                ucBtnExt_文字教程.Enabled = false;
                ucBtnExt_文字教程.FillColor = Color.FromArgb(96,96,96);
                ucBtnExt_文字教程.RectColor = ucBtnExt_文字教程.FillColor;
                ucBtnExt_视频教程.Enabled = false;
                ucBtnExt_视频教程.FillColor = Color.FromArgb(96, 96, 96);
                ucBtnExt_视频教程.RectColor = ucBtnExt_文字教程.FillColor;
            }
            else
            {
                fontsData = JsonConvert.DeserializeObject<FontsData>(cloudJson);
                UpdateData data = new UpdateData();
                data.fontsdata = fontsData;
                data.win32Window = this;
                //通过ParameterizedThreadStart创建带参线程
                Thread thread = new Thread(new ParameterizedThreadStart(UpdateCheck));
                thread.Start(data);
            }
        }

        public static void UpdateCheck(object UpdateData)
        {
            UpdateData updateData = (UpdateData)UpdateData;
            if (updateData.fontsdata.Version != localVersion)
            {
                if (FrmDialog.ShowDialog(updateData.win32Window, "此字体安装器可能不是最新版，是否更新至最新版\n\n如需要更新至最新版请点击\"确定\"按钮\n\n不需要更新请点击\"取消\"按钮，不更新不影响使用", "Acting字体安装器", true) == System.Windows.Forms.DialogResult.OK)
                {
                    System.Diagnostics.Process.Start(updateData.fontsdata.DownLink);
                }
            }
        }

        private void ucBtnExt_文字教程_BtnClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(fontsData.Texttutorial);
        }

        private void ucBtnExt_视频教程_BtnClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(fontsData.Videotutorial);
        }

        private void ucBtnExt_第一步_BtnClick(object sender, EventArgs e)
        {
            if(File.Exists(Environment.CurrentDirectory + @"\Fonts\msyhl.ttc") == true && File.Exists(Environment.CurrentDirectory + @"\Fonts\msyhbd.ttc") == true)
            {
                //FrmDialog.ShowDialog(this, "Success 正确解压了文件", "Acting字体安装器");
                ucStep.StepIndex++;
                tabControl.SelectedTab = tabPage2;
                ucBtnExt_第二步.Enabled = false;
                ucBtnExt_第二步.FillColor = Color.FromArgb(96, 96, 96);
                ucBtnExt_第二步.RectColor = ucBtnExt_第二步.FillColor;
            }
            else
            {
                FrmDialog.ShowDialog(this, "很抱歉，当前无法进行下一步，原因如下:\n\n1.您在压缩包内直接运行了软件，请确保解压到文件夹后再运行本软件\n\n2.文件是否有损坏或缺少，尝试重新下载解压覆盖", "Acting字体安装器");
            }
        }

        private void ucBtnExt_引导安装_BtnClick(object sender, EventArgs e)
        {
            ucBtnExt_第二步.Enabled = false;
            ucBtnExt_强制安装.Enabled = false;
            ucBtnExt_引导安装.Enabled = false;
            label3.Text = "现在，来帮您进行引导安装";
            label4.Text = "将会调用系统自带的字体安装器来帮助您安装字体\n稍后将会打开2个窗口，您需在2个窗口左上角点击安装\n引导过程会在5秒后开始，如您不明白可观看视频教程";
            Thread installThread = new Thread(delegate () {
            Thread.Sleep(5000);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\fontview.exe", Environment.CurrentDirectory + @"\Fonts\msyhl.ttc");
            Thread.Sleep(500);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\fontview.exe", Environment.CurrentDirectory + @"\Fonts\msyhbd.ttc");
            label3.Text = "如何，进展是否顺利？";
            label4.Text = "请在打开的2个窗口的左上角点击\"安装\"并完成安装过程\n完成字体的安装后请点击\"下一步\"按钮来进行最后操作\n如您不明白可访问文字教程或观看视频教程";
            Thread.Sleep(3000);
            ucBtnExt_强制安装.Enabled = true;
            ucBtnExt_引导安装.Enabled = true;
            ucBtnExt_第二步.Enabled = true;
            ucBtnExt_第二步.FillColor = ucBtnExt_第一步.FillColor;
            ucBtnExt_第二步.RectColor = ucBtnExt_第二步.FillColor;
            });
            installThread.Start();
        }

        private void ucBtnExt_强制安装_BtnClick(object sender, EventArgs e)
        {
            if(FrmDialog.ShowDialog(this, "强制安装需申请操作目录系统权限和文件读写权限\n\n建议您关闭相关杀毒软件和防护措施，且失败率较高\n\n如需要强制安装请点击\"确定\"按钮", "Acting字体安装器", true) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    File.Copy(Environment.CurrentDirectory + @"\Fonts\msyhbd.ttc", @"C:\Windows\Fonts\msyhbd.ttc", false);
                    File.Copy(Environment.CurrentDirectory + @"\Fonts\msyhl.ttc", @"C:\Windows\Fonts\msyhl.ttc", false);
                }
                catch (UnauthorizedAccessException)
                {
                    FrmDialog.ShowDialog(this, "强制安装失败，原因：\n\n没有权限进行系统文件操作\n\n请关闭杀毒软件并以管理员身份运行软件", "Acting字体安装器");
                }
                catch (FileNotFoundException)
                {
                    FrmDialog.ShowDialog(this, "强制安装失败，原因：\n\n字体源文件未找到\n\n请将压缩包完全解压到文件夹后重试", "Acting字体安装器");
                }
                catch (IOException)
                {
                    FrmDialog.ShowDialog(this, "强制安装失败，原因：\n\n发生了 I/O 错误,可能是文件正在被使用或无权限", "Acting字体安装器");
                }
                if (File.Exists(@"C:\Windows\Fonts\msyhl.ttc") == true && File.Exists(@"C:\Windows\Fonts\msyhbd.ttc") == true)
                {
                    ucBtnExt_强制安装.Enabled = false;
                    ucBtnExt_引导安装.Enabled = false;
                    ucBtnExt_第二步.Enabled = true;
                    ucBtnExt_第二步.FillColor = ucBtnExt_第一步.FillColor;
                    ucBtnExt_第二步.RectColor = ucBtnExt_第二步.FillColor;
                    FrmDialog.ShowDialog(this, "安装成功，字体文件已存在\n\n请点击\"下一步\"按钮来进行最后操作", "Acting字体安装器");
                }
                else
                {
                    FrmDialog.ShowDialog(this, "强制安装失败，原因：\n\n未知错误，请使用引导安装", "Acting字体安装器");
                }
            }
        }

        private void ucBtnExt_第二步_BtnClick(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Windows\Fonts\msyhl.ttc") == true && File.Exists(@"C:\Windows\Fonts\msyhbd.ttc") == true)
            {
                //安装完成
                ucStep.StepIndex++;
                tabControl.SelectedTab = tabPage3;
                //动态加载字体
                try
                {
                    PrivateFontCollection pfc1 = new PrivateFontCollection();
                    pfc1.AddFontFile(@"C:\Windows\Fonts\msyhbd.ttc");
                    Font f1 = new Font(pfc1.Families[0], 12, FontStyle.Bold);
                    label_字体粗.Font = f1;

                    PrivateFontCollection pfc2 = new PrivateFontCollection();
                    pfc2.AddFontFile(@"C:\Windows\Fonts\msyhl.ttc");
                    Font f2 = new Font(pfc2.Families[0], 12);
                    label_字体细.Font = f2;
                }
                catch
                {
                    FrmDialog.ShowDialog(this, "字体可能安装失败，原因：\n\n未知错误，请手动安装字体，具体查看视频教程", "Acting字体安装器");
                }


                if(fontsData.BuyKeysSwitch != 1)
                {
                    ucBtnExt_购买卡密.Visible = false;
                }
            }
            else
            {
                FrmDialog.ShowDialog(this, "很抱歉，字体似乎并未安装成功\n\n1.重新尝试点击\"引导安装\"再次进行安装\n\n2.点击\"强制安装\"来强制安装", "Acting字体安装器");
            }
        }

        private void ucBtnExt_退出_BtnClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ucBtnExt_购买卡密_BtnClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(fontsData.BuyKeysLink);
        }
    }
}

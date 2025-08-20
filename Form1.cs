using MEMZEffect;
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

namespace MEMZEffectTest
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private List<Action> effectActions = new List<Action>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化效果列表
            InitializeEffectList();
        }

        private void InitializeEffectList()
        {
            // 添加效果到列表
            effectListBox.Items.Add("屏幕反色");
            effectListBox.Items.Add("屏幕上下翻转");
            effectListBox.Items.Add("屏幕水平翻转");
            effectListBox.Items.Add("屏幕位移");
            effectListBox.Items.Add("屏幕花屏位移");
            effectListBox.Items.Add("随机屏幕乱码");
            effectListBox.Items.Add("屏幕网格效果");
            effectListBox.Items.Add("彩虹效果");
            effectListBox.Items.Add("屏幕波浪效果");
            effectListBox.Items.Add("屏幕闪烁");
            effectListBox.Items.Add("彩虹猫中隧道");
            effectListBox.Items.Add("彩虹猫左隧道");
            effectListBox.Items.Add("彩虹猫右隧道");
            effectListBox.Items.Add("彩虹猫上隧道");
            effectListBox.Items.Add("彩虹猫下隧道");
            effectListBox.Items.Add("屏幕融化");
            effectListBox.Items.Add("鼠标抖动");
            effectListBox.Items.Add("鼠标乱动");
            effectListBox.Items.Add("隐藏鼠标");
            effectListBox.Items.Add("显示鼠标");
            effectListBox.Items.Add("随机位置画图标");
            effectListBox.Items.Add("疯狂画图标");
            effectListBox.Items.Add("鼠标画一大堆图标");

            // 添加对应的动作
            effectActions.Add(() => EffectModule.ScreenInvert());
            effectActions.Add(() => EffectModule.ScreenFlipVertical());
            effectActions.Add(() => EffectModule.ScreenFlipHorizontal());
            effectActions.Add(() => EffectModule.ScreenShiftKill());
            effectActions.Add(() => EffectModule.ScreenMessyShiftKill());
            effectActions.Add(() => EffectModule.RandomScreenMess());
            effectActions.Add(() => EffectModule.ScreenGridEffect());
            effectActions.Add(() => EffectModule.RainbowEffect());
            effectActions.Add(() => EffectModule.ScreenWaveEffect());
            effectActions.Add(() => EffectModule.ScreenFlash());
            effectActions.Add(() => EffectModule.NyanCatMiddleTunnel());
            effectActions.Add(() => EffectModule.NyanCatLeftTunnel());
            effectActions.Add(() => EffectModule.NyanCatRightTunnel());
            effectActions.Add(() => EffectModule.NyanCatTopTunnel());
            effectActions.Add(() => EffectModule.NyanCatBottomTunnel());
            effectActions.Add(() => EffectModule.ScreenMelt());
            effectActions.Add(() => EffectModule.MouseShake());
            effectActions.Add(() => EffectModule.MouseRandomMove());
            effectActions.Add(() => EffectModule.HideMouse());
            effectActions.Add(() => EffectModule.ShowMouse());
            effectActions.Add(() => EffectModule.DrawIconsAtRandomPositions());
            effectActions.Add(() => EffectModule.DrawIconsCrazy());
            effectActions.Add(() => EffectModule.DrawIconsAtMouse());

            // 默认选择第一个效果
            if (effectListBox.Items.Count > 0)
                effectListBox.SelectedIndex = 0;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            effectTimer.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            effectTimer.Stop();
        }

        private void speedTrackBar_Scroll(object sender, EventArgs e)
        {
            effectTimer.Interval = speedTrackBar.Value;
            speedLabel.Text = string.Format("速度 ({0}ms)", speedTrackBar.Value);
        }

        private void effectTimer_Tick(object sender, EventArgs e)
        {
            if (randomEffectCheckBox.Checked)
            {
                // 随机选择一个效果
                int randomIndex = random.Next(0, effectActions.Count);
                try
                {
                    effectActions[randomIndex]();
                }
                catch (Exception ex)
                {
                    // 简单异常处理
                    Console.WriteLine("效果执行错误: " + ex.Message);
                }
            }
            else if (effectListBox.SelectedIndex >= 0 && effectListBox.SelectedIndex < effectActions.Count)
            {
                // 执行选中的效果
                try
                {
                    effectActions[effectListBox.SelectedIndex]();
                }
                catch (Exception ex)
                {
                    // 简单异常处理
                    Console.WriteLine("效果执行错误: " + ex.Message);
                }
            }
        }
    }
}

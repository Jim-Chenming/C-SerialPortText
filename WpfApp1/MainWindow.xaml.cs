using System;
using System.Text;
using System.Windows;
using System.IO.Ports;//串口
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace MyServer
{
    //获取鼠标X,Y 全局位置信息
    class MouseXY {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;

            public POINT(int x, int y) {
                this.X = x;
                this.Y = y;
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
    }

    //APP入口
    public partial class MainWindow : Window
    {
        bool ReadIsHex;
        bool SendIsHex;
        SerialPort SerialPort_var = new SerialPort();
        SynchronizationContext SyncContext = null;
        private Thread my_thread;
        private bool my_thread_work;
        private Timer tim;
        private bool SendRGB;

        //在接收到数据后的服务函数中，线程安全回调函数；避免直接操作UI线程
        private void PostCallback(object str) {
            TextBox_Data.AppendText(str.ToString());
        }        
        private void Post_Count_Callback(object str) {
            Lable_ReadDataNumber.Content = Convert.ToInt32(Lable_ReadDataNumber.Content)+ Convert.ToInt32(str);
        }
        private void PostRGBCallback(object str) {
            try {
                    if (SendRGB && SerialPort_var.IsOpen) 
                    {
                        byte[] myColor = new byte[3];
                        uint index = 0;
                        MouseXY.POINT pi = new MouseXY.POINT();
                        MouseXY.GetCursorPos(out pi);
                        IntPtr hdc = GetDC(IntPtr.Zero);
                        uint pixel = GetPixel(hdc, pi.X, pi.Y);

                        ReleaseDC(IntPtr.Zero, hdc);
                        myColor[0] = (byte)(pixel & 0x000000FF);
                        myColor[1] = (byte)((pixel & 0x0000FF00) >> 8);
                        myColor[2] = (byte)((pixel & 0x00FF0000) >> 16);

                        SerialPort_var.Write('+'.ToString());
                        SerialPort_var.Write(myColor, 0, 3);//写数据
                        SerialPort_var.Write('-'.ToString());

                        Color color = new Color();
                        color.R = myColor[0];
                        color.G = myColor[1];
                        color.B = myColor[2];
                    }
                }
            catch (NotImplementedException e) {

            }
        }
        //POST回调函数结束

        //定时器任务
        private void TimerCallBack(object state) {
            SyncContext.Post(PostRGBCallback, null);
        }
        public MainWindow()
        {
            InitializeComponent();
            //线程切换
            SyncContext = SynchronizationContext.Current;
            //串口默认初始化
            SerialPort_var.Encoding = Encoding.GetEncoding("GB2312");//默认使用 GB2312 编码
            ComboBox_BaudRate.SelectedIndex = 5;//默认使用 115200 波特率
            ComboBox_ChooseMode.SelectedIndex = 0;//默认模式
            //定时器对象，指定1000ms后启动，并且每隔100进入一次服务函数
            tim = new Timer(TimerCallBack,null,1000,100);
            //自动扫描串口
            ButtonScanfPort(null,null);
        }


        //扫描串口
        private void ButtonScanfPort(object sender, RoutedEventArgs e) {
            if (this.Button_ScanfPort.Opacity != 1)//如果不可点击
                return;
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            int i;
            //先清空原来列表,避免已经使用中途拔掉串口
            ComboBox_PortNumber.Items.Clear();

            for (i = 0; i < portList.Length; ++i)
            {
                string name = portList[i];
                ComboBox_PortNumber.Items.Add(name);
            }
            //默认选择第一项
            if (ComboBox_PortNumber.Items.Count!=0) 
            {
                ComboBox_PortNumber.SelectedIndex=0;
            }
        }

        //打开串口
        private void ButtonOpenPortClick(object sender, RoutedEventArgs e) {
            if (SerialPort_var.IsOpen) //如果没有打开串口，则提示打开串口
            {
                my_thread_work = false;
                SerialPort_var.Close();
                this.ButtonOpenPort.Content = "打开串口";
                this.Button_ScanfPort.Opacity = 1;
                return ;
            }
            
            string PortNumber = ComboBox_PortNumber.Text;
            if (PortNumber.Length == 0) {
                MessageBox.Show("请先将设备插至电脑,扫描串口，然后选择串口号","——提示——");
                return;
            }

            //串口配置
            SerialPort_var.PortName = PortNumber;//选择串口号
            SerialPort_var.BaudRate=Convert.ToInt32(ComboBox_BaudRate.Text);//选择波特率
            SerialPort_var.StopBits = StopBits.One;
            SerialPort_var.DataBits = 8;
            try {
                SerialPort_var.Open();//打开串口
                if (SerialPort_var.IsOpen) {
                    //串口打开成功，启动接收线程
                    my_thread=new Thread(My_SerialDataReceivedThread);
                    my_thread_work=true;
                    my_thread.Start();
                    //修改按键显示
                    this.ButtonOpenPort.Content = "关闭串口";
                    this.Button_ScanfPort.Opacity = 0.4;
                }
            }
            catch { 

            }
        }
 

        private void My_SerialDataReceivedThread() 
        {
            try {
                while (my_thread_work) 
                {
                    Thread.Sleep(100);
                    if (SerialPort_var.IsOpen)//判断串口是否打开，如果打开执行下一步操作
                    { 
                        if (ReadIsHex)  //如果接收模式为Hex接收
                        {
                            byte data;
                            data = (byte)SerialPort_var.ReadByte();//此处需要强制类型转换，将(int)类型数据转换为(byte类型数据，不必考虑是否会丢失数据
                            string str = Convert.ToString(data, 16).ToUpper();//转换为大写十六进制字符串
                            SyncContext.Post(PostCallback, "0x" + (str.Length == 1 ? "0" + str : str) + " ");//线程数据传递
                            SyncContext.Post(Post_Count_Callback, 1);//线程数据传递
                            continue;
                        }
                        string r_str;
                        r_str = SerialPort_var.ReadExisting();
                        SyncContext.Post(PostCallback, r_str);//线程数据传递
                        SyncContext.Post(Post_Count_Callback, r_str.Length);//线程数据传递
                    }
                }
            }
            catch { }
        }

        private void ButtonSendData(object sender, RoutedEventArgs e) 
        {
            byte[] Data = new byte[1];
            string sendStr= TextBox_SendData.Text;
            int i = 0;
            if (SerialPort_var.IsOpen)//判断串口是否打开，如果打开执行下一步操作
            {
                if (TextBox_SendData.Text != "")
                {
                        if (!SendIsHex)//如果发送模式是字符模式
                        {
                            try 
                            {
                                SerialPort_var.Write(sendStr);//写数据
                                Lable_SendDataNumber.Content = Convert.ToInt32(Lable_SendDataNumber.Content) + sendStr.Length;
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show("串口数据写入错误", "错误");//出错提示
                                SerialPort_var.Close(); 
                                this.ButtonOpenPort.Content = "打开串口";
                            }
                        }
                        else
                        {
                            sendStr = Regex.Replace(sendStr, @"\s", "");//去除空格

                            for (i = 0; i < (sendStr.Length - sendStr.Length % 2) / 2; i++)//取余3运算作用是防止用户输入的字符为奇数个
                            {
                                Data[0] = Convert.ToByte(sendStr.Substring(i * 2, 2), 16);
                                SerialPort_var.Write(Data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
                            }
                            i *= 2;
                            if (sendStr.Length % 2 != 0)//剩下一位单独处理
                            {
                                Data[0] = Convert.ToByte(sendStr.Substring(sendStr.Length - 1, 1), 16);//单独发送B（0B）
                                SerialPort_var.Write(Data, 0, 1);//发送
                                i++;
                            }
                        Lable_SendDataNumber.Content = Convert.ToInt32(Lable_SendDataNumber.Content) + i;

                    }
                }
            }
        }
        //清空数据
        private void ButtonClearDataPort(object sender, RoutedEventArgs e) {
            this.TextBox_Data.Clear();
            Lable_ReadDataNumber.Content = 0;
            Lable_SendDataNumber.Content = 0;
        }

        private void ButtonHexRead(object sender, RoutedEventArgs e) {
            if (!this.ButtonHexOrText_Read.Content.Equals("Hex")) {
                ReadIsHex = true;
                this.ButtonHexOrText_Read.Content = "Hex";
            }
            else 
            {
                ReadIsHex = false;
                this.ButtonHexOrText_Read.Content = "Text";
            }
        }

        private void ButtonHexSend(object sender, RoutedEventArgs e) {
            if (!this.ButtonHexOrText_Send.Content.Equals("Hex")) {
                SendIsHex = true;
                this.ButtonHexOrText_Send.Content = "Hex";
            }
            else {
                SendIsHex = false;
                this.ButtonHexOrText_Send.Content = "Text";
            }
        }

        private void ButtonSendRGB(object sender, RoutedEventArgs e) {
            if (!SerialPort_var.IsOpen) { 
                MessageBox.Show("请先打开串口","——提示——");
                return;
            }

            if (this.ComboBox_ChooseMode.Opacity != 0.4) {
                SendRGB = true;
                this.Button_SendRGB.Content = "效果停止";
                this.ComboBox_ChooseMode.IsEnabled = false;
                this.ComboBox_ChooseMode.Opacity = 0.4;
                return;
            }
            SendRGB = false;
            this.Button_SendRGB.Content = "效果启动";
            this.ComboBox_ChooseMode.IsEnabled = true;
            this.ComboBox_ChooseMode.Opacity = 1;
        }


        //获取颜色的动态库功能函数
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
    }
}

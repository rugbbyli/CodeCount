using CodeCount;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeCountUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ComboBox_Lan.ItemsSource = LanguageSetting.Setting.Keys;
            ComboBox_Lan.SelectionChanged += ComboBox_Lan_SelectionChanged;
            ComboBox_Lan.SelectedIndex = 0;
        }

        void ComboBox_Lan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var set = LanguageSetting.Setting[ComboBox_Lan.SelectedItem.ToString()];
            if (set != null)
            {
                this.DataContext = set;
            }
        }

        private void Button_Select_Path_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();

            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBox_Path.Text = dlg.SelectedPath;
            }
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_Path.Text) ||
                string.IsNullOrEmpty(TextBox_Ext.Text) ||
                string.IsNullOrEmpty(TextBox_LineCmt.Text) ||
                string.IsNullOrEmpty(TextBox_LineStr.Text))
            {
                MessageBox.Show("请检查输入参数的完整性！");
                return;
            }

            var setting = new Setting()
            {
                Path = TextBox_Path.Text,
                ExtArrayStr = TextBox_Ext.Text,
                LineCommentStr = TextBox_LineCmt.Text,
                BlockCommentStartStr = TextBox_BlockCmtLeft.Text,
                BlockCommentEndStr = TextBox_BlockCmtRight.Text,
                BlockStringStartStr = TextBox_BlockStrLeft.Text,
                BlockStringEndStr = TextBox_BlockStrRight.Text,
                LineStringArrayStr = TextBox_LineStr.Text,
            };

            var logName = "log_" + setting.Path.Replace(System.IO.Path.DirectorySeparatorChar, '_').Replace(":", "") + ".txt";
            LogFile = new System.IO.StreamWriter(logName, false);

            //启动CodeCount.exe，并传入参数字符串
            var startInfo = new System.Diagnostics.ProcessStartInfo("CodeCount.exe", setting.ToString());
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            var process = System.Diagnostics.Process.Start(startInfo);

            process.OutputDataReceived += process_OutputDataReceived;
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();

            LogFile.Close();

            //打开统计结果
            System.Diagnostics.Process.Start("notepad.exe", System.IO.Path.Combine(Environment.CurrentDirectory, logName));
        }

        System.IO.StreamWriter LogFile;

        void process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            LogFile.WriteLine(e.Data);
        }
    }
}

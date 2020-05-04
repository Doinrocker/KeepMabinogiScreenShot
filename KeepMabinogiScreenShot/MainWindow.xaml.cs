// Copyright by SangHyuk Kim
// All Rights Reserved.
//
// Author: SangHyuk Kim, shkim0008@gmail.com

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KeepMabinogiScreenShot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public struct ButtonConfiguration
        {
            public Brush BGColor;
            public Brush FGColor;

            public ButtonConfiguration(Brush bgColor, Brush fgColor)
            {
                this.BGColor = bgColor;
                this.FGColor = fgColor;
            }
        }
        const string SetupFileName = "setup.json";
        private string targetPath;
        private string targetPattern;
        private readonly Processor processor;
        private readonly BackgroundWorker backupScreenShotMonitor;
        private readonly ButtonConfiguration runBtnDefaultConfig = new ButtonConfiguration(Brushes.White, Brushes.Black);
        private readonly ButtonConfiguration runBtnWorkingConfig = new ButtonConfiguration(Brushes.Crimson, Brushes.White);

        public MainWindow()
        {
            InitializeComponent();
            this.GetSetupFromJson();
            this.processor = new Processor(this.targetPath, this.targetPattern);
            this.backupScreenShotMonitor = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.backupScreenShotMonitor.DoWork += this.monitoring_run;
            this.Topmost = true;
        }

        private void GetSetupFromJson()
        {
            string setupFileContents;
            using (StreamReader sw = new StreamReader(SetupFileName))
            {
                setupFileContents = sw.ReadToEnd();
            }
            var jsonReadObj = JObject.Parse(setupFileContents);
            var targetPath = jsonReadObj.SelectToken("target_path").ToString();
            targetPath = Environment.ExpandEnvironmentVariables(targetPath);
            if (System.IO.Directory.Exists(targetPath))
            {
                this.targetPath = targetPath;
            }
            else
            {
                throw new System.ArgumentException("Invalid target path from setup.json");
            }

            this.targetPattern = jsonReadObj.SelectToken("target_pattern").ToString();

            if (string.IsNullOrEmpty(this.targetPattern))
            {
                throw new System.ArgumentException("Invalid target pattern from setup.json");
            }
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.backupScreenShotMonitor.IsBusy)
            {
                this.processor.BackupScreenShots();
                this.backupScreenShotMonitor.RunWorkerAsync();
                this.runButton.Background = this.runBtnWorkingConfig.BGColor;
                this.runButton.Foreground = this.runBtnWorkingConfig.FGColor;
                var flashButton = FindResource("FlashButton") as Storyboard;
                flashButton.Begin();
            }
        }

        private void forceStopButton_Click(object sender, RoutedEventArgs e)
        {
            this.processor.ForceStop();
            this.backupScreenShotMonitor.CancelAsync();
            this.runButton.Background = this.runBtnDefaultConfig.BGColor;
            this.runButton.Foreground = this.runBtnDefaultConfig.FGColor;
        }

        private void mvBackupFilesButton_Click(object sender, RoutedEventArgs e)
        {
            this.processor.MoveBackupFiles();
        }

        private void monitoring_run(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (this.processor.WorkComplete)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                    { 
                        var flashButton = FindResource("FlashButton") as Storyboard;
                        flashButton.Stop();
                        this.runButton.Background = this.runBtnDefaultConfig.BGColor;
                        this.runButton.Foreground = this.runBtnDefaultConfig.FGColor;
                    });
                    return;
                }
            }
        }
    }
}

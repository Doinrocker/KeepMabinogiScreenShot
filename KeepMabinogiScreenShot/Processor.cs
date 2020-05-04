// Copyright by SangHyuk Kim
// All Rights Reserved.
//
// Author: SangHyuk Kim, shkim0008@gmail.com

using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading;

namespace KeepMabinogiScreenShot
{
    /// <summary>
    /// 
    /// </summary>
    class Processor
    {
        const int NumDefaultScreenShots = 6;
        const int DelayTimeMs = 10;
        readonly string targetPath;
        readonly string backupPath;
        readonly string backupPattern;
        private readonly BackgroundWorker workerObj;

        public bool WorkComplete { get; private set; } = false;

        public Processor(string targetPath, string backupPattern)
        {
            this.targetPath = targetPath;
            this.backupPattern = backupPattern;
            this.backupPath = $"{targetPath}backup/";
            this.workerObj = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.workerObj.DoWork += this.BackupScreenShots;
            this.workerObj.RunWorkerCompleted += this.BackuptScreenShotDone;
        }

        public void ForceStop()
        {
            if (this.workerObj.IsBusy)
            {
                this.workerObj.CancelAsync();
            }
        }

        public void MoveBackupFiles()
        {
            var now = DateTime.Now.ToString("yyyy_MM_dd_");
            var screenShotNamePattern = now + '*';
            string[] screenShotFileList = Directory.GetFiles(this.targetPath, screenShotNamePattern);
            int startNumber = 0;
            if (screenShotFileList.Length > 0)
            {
                string lastFileName = screenShotFileList[screenShotFileList.Length - 1];
                string[] seperateName = lastFileName.Split('_');
                int lastNumber = int.Parse(seperateName[seperateName.Length - 1].Split('.')[0]);
                startNumber = lastNumber + 1;
            }

            string[] backupFileList = Directory.GetFiles($"{this.backupPath}", this.backupPattern);
            if (backupFileList.Length < 1)
            {
                return;
            }
            else
            {
                foreach (var file in backupFileList.Select((name, index) => new { index, name }))
                {
                    var name = file.name;
                    var number = startNumber + file.index;
                    var destFileName = $"{this.targetPath}/{now}{number:0000}.jpg";
                    File.Move(name, destFileName);
                }
            }
        }

        public void BackupScreenShots()
        {
            if (!this.workerObj.IsBusy)
            {
                this.WorkComplete = false;
                this.workerObj.RunWorkerAsync();
            }
        }

        public void BackupScreenShots(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if ((sender as BackgroundWorker).CancellationPending == true)
                {
                    return;
                }
                else
                {
                    string[] files = Directory.GetFiles(this.targetPath, $"{this.backupPattern}*");
                    if (files.Length > NumDefaultScreenShots - 1)
                    {
                        foreach (var targetName in files)
                        {
                            var fileName = Path.GetFileName(targetName);
                            var destName = $"{this.backupPath}{fileName}";
                            File.Copy(targetName, destName, true);
                        }
                        return;
                    }
                }
                Thread.Sleep(DelayTimeMs);
            }
        }

        public void BackuptScreenShotDone(object sendeer, RunWorkerCompletedEventArgs e)
        {
            this.WorkComplete = true;
        }
    }
}

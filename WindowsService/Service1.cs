using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
       
        public Service1()
        {
            InitializeComponent();
          
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;

        ITelegramBotClient bot = new TelegramBotClient("");
        long chatId = 0;           
        public Logger()
        {

           
            watcher = new FileSystemWatcher("C:\\Users\\freia\\Downloads\\");
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }

        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
            var stream = System.IO.File.Open("C:/Log/Log.txt", FileMode.Open);
            bot.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, "xas.txt"));
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            var stream = System.IO.File.Open("C:/Log/Log.txt", FileMode.Open);
            bot.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, "xas.txt"));
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            var stream = System.IO.File.Open("C:/Log/Log.txt", FileMode.Open);
            bot.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, "xas.txt"));
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            var stream = System.IO.File.Open("C:/Log/Log.txt", FileMode.Open);
            bot.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, "xas.txt"));
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Log\\Log.txt", true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
}

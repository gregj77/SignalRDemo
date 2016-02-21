using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Chat.Client.ControlLogic;
using DtoModel;

namespace SiteConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string User { get; set; }
        public static bool IsBencharkMode { get; private set; }

        protected async override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1 && e.Args[0] == "benchmark")
            {
                IsBencharkMode = true;
                var mre = new ManualResetEvent(false);
                var client = new ChatClient(e.Args[0]);
                client
                    .EnterRoom(e.Args[0])
                    .TakeWhile(msg => msg.Message != "DONE")
                    .Subscribe(_ => { }, async () =>
                    {
                        await Task.Delay(1000);
                        mre.Set();
                    });
                await Task.Delay(500);
                await client.SendMessage(new ChatMessage
                {
                    Room = e.Args[0],
                    Message = "Client " + Process.GetCurrentProcess().Id + " is ready!"
                });
                mre.WaitOne(TimeSpan.FromMinutes(5.0));
                await client.SendMessage(new ChatMessage
                {
                    Room = e.Args[0],
                    Message = "Client " + Process.GetCurrentProcess().Id + " exiting!"
                });
                await Task.Delay(500);
                client.Dispose();
                await Task.Delay(500);
                Environment.Exit(0);
            }
            else
            {
                base.OnStartup(e);
            }
        }
    }
}

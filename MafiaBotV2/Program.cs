using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using MafiaBotV2.Util;

namespace MafiaBotV2
{
    class Program
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args) {
            //while (true) {
                try {
                    Bot bot = new Bot("config.xml");
                    bot.Run();
                    bot.Shutdown();
                    System.Threading.Thread.Sleep(1000);
                }
                catch(Exception ex) {
                    Log.Fatal("Exception in main loop: " + ex.ToString());
                }
            //}
        }

    }
}

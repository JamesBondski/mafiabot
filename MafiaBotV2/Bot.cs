using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MafiaBotV2.Network;
using MafiaBotV2.Network.Irc;
using MafiaBotV2.Network.File;
using MafiaBotV2.Util;

namespace MafiaBotV2
{
    public class Bot
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        XDocument config;
        public System.Xml.Linq.XDocument Config {
            get { return config; }
        }
        NetMaster net;
        public MafiaBotV2.Network.NetMaster Master {
            get { return net; }
        }
        NetChannel mainChannel;
        public MafiaBotV2.Network.NetChannel MainChannel {
            get { return mainChannel; }
        }
        GameManager games;
        public MafiaBotV2.GameManager Games {
            get { return games; }
        }

        public Bot(string configFile) : this(configFile, null) {}

        public Bot(string configFile, NetMaster master) {
            config = XDocument.Load(configFile);

            var server = (from s in config.Descendants("Server")
                          select new { host = s.Element("Host").Value, port = s.Element("Port").Value, name = s.Element("Nickname").Value, channel = s.Element("Channel").Value, password = s.Element("Password") }
                         ).ElementAt(0);

            if(master == null) {
                Log.Info("Connecting to " + server.host + ":" + server.port);
                this.net = new IrcMaster(server.host, Int32.Parse(server.port));
            }
            else {
                this.net = master;
            }
            
            games = new GameManager(this);

            string password = null;
            if(server.password != null) {
                password = server.password.Value;
            }
            Log.Info("Logging in as " + server.name + " (Using Password: " + (password != null) + ")");
            if (net is IrcMaster) {
                (net as IrcMaster).Login(server.name, "MafiaBot V2", password);
            }

            mainChannel = net.GetChannel(server.channel);

            mainChannel.Commands.Add(new Commands.VariantsCommand());
            mainChannel.Commands.Add(new Commands.CreateCommand(this));
            mainChannel.Commands.Add(new Commands.ListCommand(this));
            mainChannel.Commands.Add(new Commands.OpDestroyCommand(this));
            mainChannel.Commands.Add(new Commands.HelpCommand());
        }

        public void Run() {
            while (net.ProcessMessage()) {
                Scheduler.Instance.Execute();
                System.Threading.Thread.Sleep(1);
            }
        }

        public void Shutdown() {
            net.Close();
        }
    }
}

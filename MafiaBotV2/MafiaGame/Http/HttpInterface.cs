using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;
using System.IO;
using MafiaBotV2.MafiaLib;

namespace MafiaBotV2.MafiaGame.Http
{
    public class HttpInterface
    {
        HttpListener listener;
        MafiaGame game;
        int secret;

        public string Url {
            get {
                return "http://mafiabot.game-host.org:16981/" + secret + "/";
            }
        }

        public HttpInterface(MafiaGame game) {
            secret = (new Random()).Next();
            this.game = game;

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:16981/" + secret + "/");

            listener.Start();
            listener.BeginGetContext(new AsyncCallback(OnRequest), null);
        }

        public void Close() {
            listener.Stop();
        }

        public void OnRequest(IAsyncResult async) {
            HttpListenerContext context;
            try {
                context = listener.EndGetContext(async);
            }
            catch (ObjectDisposedException) {
                return;
            }
            listener.BeginGetContext(new AsyncCallback(OnRequest), null);

            string path = context.Request.Url.AbsolutePath.Substring(secret.ToString().Length + 2);

            switch (path) {
                case "":
                    ReplyWithFile(context, "MafiaBotV2.MafiaGame.Http.Data.admin.html");
                    break;
                case "mafiabot.js":
                    ReplyWithFile(context, "MafiaBotV2.MafiaGame.Http.Data.mafiabot.js");
                    break;
                case "mafiabot.css":
                    ReplyWithFile(context, "MafiaBotV2.MafiaGame.Http.Data.mafiabot.css");
                    break;
                case "init.js":
                    ReplyWithString(context, GetInitJS());
                    break;
                case "nolynch":
                    game.Village.Rules.AllowNolynch = (context.Request.QueryString["value"] == "true");
                    ReplyWithString(context, "");
                    break;
                case "cardflip":
                    game.Village.Rules.CardFlip = (context.Request.QueryString["value"] == "true");
                    ReplyWithString(context, "");
                    break;
                case "night0":
                    if (context.Request.QueryString["value"] == "true") {
                        game.Village.Rules.InitialPhase = PhaseType.Night0;
                    }
                    else {
                        game.Village.Rules.InitialPhase = PhaseType.Day;
                    }
                    ReplyWithString(context, "");
                    break;
                case "addrole":
                    string role = context.Request.QueryString["role"];
                    string faction = context.Request.QueryString["faction"];
                    game.Village.Factions.First(f => f.Name.ToLower() == faction.ToLower()).CreateMember(role, role);
                    game.MaxPlayers = game.Village.Rules.MaximumPopulation;
                    ReplyWithString(context, "");
                    break;
                case "removerole":
                    string roleName = context.Request.QueryString["role"];
                    string factionName = context.Request.QueryString["faction"];
                    VillageMember member = game.Village.Factions.First(f => f.Name.ToLower() == factionName.ToLower()).Members.First(vm => vm.RoleName.ToLower() == roleName.ToLower());
                    member.Faction.RemoveMember(member);
                    game.MaxPlayers = game.Village.Rules.MaximumPopulation;
                    ReplyWithString(context, "");
                    break;
                default:
                    context.Response.OutputStream.Close();
                    break;
            }
        }

        private string GetInitJS() {
            StringBuilder js = new StringBuilder();
            VillageRules rules = game.Village.Rules;

            if(rules.AllowNolynch) {
                js.Append("$(\"#nolynch\").attr(\"checked\", true);");
            }
            if (rules.CardFlip) {
                js.Append("$(\"#cardflip\").attr(\"checked\", true);");
            }
            if (rules.InitialPhase == PhaseType.Night0) {
                js.Append("$(\"#night0\").attr(\"checked\", true);");
            }

            foreach (Faction f in game.Village.Factions) {
                foreach (VillageMember member in f.Members) {
                    if (f.Name == "Town") {
                        js.Append("addTownRole(");
                    }
                    else if (f.Name == "Mafia") {
                        js.Append("addMafiaRole(");
                    }
                    js.Append('"').Append(member.RoleName).Append('"').Append(");");
                }
            }

            foreach (KeyValuePair<string, RoleManager.RoleInfo> role in RoleManager.Instance.Roles) {
                js.Append("addRole(").Append('"').Append(role.Key).Append('"').Append(");");
            }

            return js.ToString();
        }

        private void ReplyWithFile(HttpListenerContext context, string path) {
            Stream adminHtmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            byte[] buffer = new byte[adminHtmlStream.Length];
            adminHtmlStream.Read(buffer, 0, (int)adminHtmlStream.Length);

            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        private void ReplyWithString(HttpListenerContext context, string content) {
            byte[] buffer = System.Text.Encoding.Default.GetBytes(content);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}

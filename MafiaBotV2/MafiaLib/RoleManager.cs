using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace MafiaBotV2.MafiaLib
{
    public class RoleManager
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        Dictionary<string, RoleInfo> roles = new Dictionary<string, RoleInfo>();
        public Dictionary<string, RoleInfo> Roles {
            get { return roles; }
        }

        private RoleManager() {
            Load();
        }

        public void InitRole(VillageMember member, string roleName) {
            if(!roles.ContainsKey(roleName)) {
                throw new NoSuchRoleException("There is no role with the name " + roleName);
            }

            RoleInfo role = roles[roleName];

            if(role.Inherits != null) {
                InitRole(member, role.Inherits);
            }

            member.RoleName = role.Name;

            foreach(RoleInfo.PowerInfo powerInfo in role.Powers) {
                List<object> args = new List<object>();
                
                object power;
                if (powerInfo.Type == RoleInfo.PowerType.Effect) {
                    args.Add(member);
                }
                if (powerInfo.Args.Count > 0) {
                    args.Add(powerInfo.Args);   
                }
                power = powerInfo.PowerType.InvokeMember("", BindingFlags.CreateInstance, null, null, args.ToArray());

                if(powerInfo.Duration != 0) {
                    if (power is Effect) {
                        (power as Effect).Duration = powerInfo.Duration;
                    }
                    else {
                        (power as Power).Charges = powerInfo.Duration;
                    }
                }
                
                if(powerInfo.Type == RoleInfo.PowerType.Power) {
                    member.AddPower((Power)power);
                }
                else {
                    member.ApplyEffect((Effect)power);
                }
            }

            if(role.ShowsAs != null) {
                member.RoleName = role.ShowsAs;
            }
        }

        private void Load() {
            XmlDocument doc = new XmlDocument();
            doc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("MafiaBotV2.MafiaLib.Data.Roles.xml"));

            foreach (XmlNode roleNode in doc.SelectNodes("Roles/Role")) {
                LoadRole(roleNode);
            }
        }

        private void LoadRole(XmlNode roleNode) {
            try {
                RoleInfo role = new RoleInfo();
                role.Name = roleNode.SelectSingleNode("Name").InnerText;
                if(roleNode.SelectSingleNode("InheritsFrom") != null) {
                    role.Inherits = roleNode.SelectSingleNode("InheritsFrom").InnerText;
                }
                if(roleNode.SelectSingleNode("ShowsAs") != null) {
                    role.ShowsAs = roleNode.SelectSingleNode("ShowsAs").InnerText;
                }
                foreach(XmlNode powerNode in roleNode.SelectNodes("Powers/Power")) {
                    LoadPower(powerNode, role, RoleInfo.PowerType.Power);
                }
                foreach (XmlNode powerNode in roleNode.SelectNodes("Effects/Effect")) {
                    LoadPower(powerNode, role, RoleInfo.PowerType.Effect);
                }
                roles.Add(role.Name, role);
            }
            catch(Exception ex) {
                Log.Warn("Failed to load a role: " + ex.Message);
            }
        }

        private void LoadPower(XmlNode powerNode, RoleInfo role, RoleInfo.PowerType powerType) {
            RoleInfo.PowerInfo power = new RoleInfo.PowerInfo();

            string typeString = powerNode.Attributes["type"].Value;

            // Arguments
            foreach(XmlNode arg in powerNode.ChildNodes) {
                power.Args.Add(arg.Name, arg.InnerText);
            }

            power.PowerType = Type.GetType(typeString);
            if (power.PowerType == null) {
                Log.Warn("Invalid power type defined for role " + role.Name);
                return;
            }
            power.Type = powerType;

            if(powerNode.Attributes["duration"] != null) {
                power.Duration = Int32.Parse(powerNode.Attributes["duration"].Value);
            }
            if (powerNode.Attributes["charges"] != null) {
                power.Duration = Int32.Parse(powerNode.Attributes["charges"].Value);
            }

            role.Powers.Add(power);
        }

        #region RoleManager Singleton
        public static RoleManager Instance {
            get {
                return NestedRoleManager.instance;
            }
        }

        class NestedRoleManager
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NestedRoleManager() {
            }

            internal static readonly RoleManager instance = new RoleManager();
        }
        #endregion
        
        public class RoleInfo {
            string name;
            public string Name {
                get { return name; }
                set { name = value; }
            }
            string inherits;
            public string Inherits {
                get { return inherits; }
                set { inherits = value; }
            }
            List<PowerInfo> powers = new List<PowerInfo>();
            public List<PowerInfo> Powers {
                get { return powers; }
            }
            string showsAs;
            public string ShowsAs {
                get { return showsAs; }
                set { showsAs = value; }
            }

            public class PowerInfo {
                Type powerType;
                public System.Type PowerType {
                    get { return powerType; }
                    set { powerType = value; }
                }
                Dictionary<string,string> args = new Dictionary<string,string>();
                public Dictionary<string, string> Args {
                    get { return args; }
                    set { args = value; }
                }
                PowerType type;
                public PowerType Type {
                    get { return type; }
                    set { type = value; }
                }
                int duration = 0;
                public int Duration {
                    get { return duration; }
                    set { duration = value; }
                }
            }
            public enum PowerType {
                Power,
                Effect
            }
        }
    }

    [global::System.Serializable]
    public class NoSuchRoleException : Exception
    {
        public NoSuchRoleException() { }
        public NoSuchRoleException(string message) : base(message) { }
        public NoSuchRoleException(string message, Exception inner) : base(message, inner) { }
        protected NoSuchRoleException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

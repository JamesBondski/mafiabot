using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MafiaBotV2.Util;

namespace MafiaBotV2.MafiaLib
{
    public class GameOverEventArgs : EventArgs
    {
        List<Faction> winningFactions;
        public List<Faction> WinningFactions {
            get { return winningFactions; }
        }

        string winMessage;
        public string WinMessage {
            get { return winMessage; }
        }

        public GameOverEventArgs(List<Faction> winningFactions, string winMessage) {
            this.winningFactions = winningFactions;
            this.winMessage = winMessage;
        }
    }

    public class Village
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<GameOverEventArgs> GameOver;
        public event EventHandler<PhaseOverEventArgs> PhaseOver;

        string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }

        List<VillageMember> members = new List<VillageMember>();
        public List<VillageMember> Members {
            get { return members; }
            set { members = value; }
        }

        public List<VillageMember> AliveMembers {
            get { return GetAliveMembers(); }
        }

        List<Faction> factions = new List<Faction>();
        public List<Faction> Factions {
            get { return factions; }
            set { factions = value; }
        }

        VillageRules rules = new VillageRules();
        public MafiaBotV2.MafiaLib.VillageRules Rules {
            get { return rules; }
        }

        Phase phase = null;
        public MafiaBotV2.MafiaLib.Phase Phase {
            get { return phase; }
        }

        IOutsideConnector outside = new DummyOutsideConnector();
        public MafiaBotV2.MafiaLib.IOutsideConnector Outside {
            get { return outside; }
            set { outside = value; }
        }

        public event EventHandler<ActionEventArgs> ActionPrepared;
        public event EventHandler<ActionEventArgs> ActionExecuted;

        public Village() {
        }

        public Village(IVariantLoader loader) {
            loader.Load(this);
            ActionExecuted += new EventHandler<ActionEventArgs>(OnActionExecuted);
        }

        void OnActionExecuted(object sender, ActionEventArgs e) {
            if(GetWinningFactions().Count > 0) {
                phase.End();
            }
        }

        public void Start() {
            phase = new Phase(this, Rules.InitialPhase);
            phase.PhaseOver += new EventHandler(OnPhaseOver);

            Outside.SendMessage("It is now " + phase.Type.ToString());
            if (Rules.InitialPhase == PhaseType.Day) {
                Outside.AllowTalking(true);
            }
            Log.Debug("Game started (Initial Phase: " + phase.Type.ToString() + ")");
        }

        public void RaiseActionPrepared(Action action) {
            if (ActionPrepared != null) {
                ActionPrepared(this, new ActionEventArgs(action));
            }
        }

        public void RaiseActionExecuted(Action action) {
            if (ActionExecuted != null) {
                ActionExecuted(this, new ActionEventArgs(action));
            }
        }

        public Faction CreateFaction(string name) {
            Log.Debug("Creating faction " + name);
            Faction faction = new Faction(this, name);
            factions.Add(faction);
            return faction;
        }

        void OnPhaseOver(object sender, EventArgs e) {
            Phase oldPhase = phase;

            Outside.SendMessage(phase.Type.ToString() + " is over.");

            List<Faction> winningFactions = GetWinningFactions();
            if (winningFactions.Count > 0) {
                Log.Debug("Game is over.");
                string winMessage = GetWinMessage(winningFactions);
                Outside.SendMessage(winMessage);
                phase = new Phase(this, PhaseType.Over);
                if (GameOver != null) {
                    GameOver(this, new GameOverEventArgs(winningFactions, winMessage));
                }
                return;
            }

            NextPhase();
            phase.PhaseOver += new EventHandler(OnPhaseOver);

            foreach (VillageMember member in Members) {
                member.Turn();
            }
            foreach (Faction faction in Factions) {
                faction.Turn();
            }
            Outside.SendMessage("It is now " + phase.Type.ToString());

            if(PhaseOver != null) {
                PhaseOver(this, new PhaseOverEventArgs(oldPhase, phase));
            }
        }

        void NextPhase() {
            Log.Debug("Determining next phase");
            PhaseType initialPhase = phase.Type;
            do {
                switch(phase.Type) {
                    case PhaseType.Day:
                        phase = new Phase(this, PhaseType.Twilight);
                        break;
                    case PhaseType.Twilight:
                        phase = new Phase(this, PhaseType.Night);
                        break;
                    case PhaseType.Night:
                        phase = new Phase(this, PhaseType.Dawn);
                        break;
                    case PhaseType.Dawn:
                        phase = new Phase(this, PhaseType.Day);
                        break;
                    case PhaseType.Night0:
                        phase = new Phase(this, PhaseType.Dawn);
                        break;
                }

                if(phase.Type == initialPhase) {
                    throw new MafiaException("No valid phase found!");
                }
            } while (!Members.Any(m => m.Powers.Any(p => p.CanExecute(this) && !p.Phases.Any(ph => ph == PhaseType.Any)))
                && !Factions.Any(f => f.Powers.Any(p => p.CanExecute(this) && !p.Phases.Any(ph => ph == PhaseType.Any))));

            if(phase.Type == PhaseType.Day) {
                Outside.AllowTalking(true);
            }
            else {
                Outside.AllowTalking(false);
            }

            Log.Debug("Next phase found: " + phase.Type.ToString());
        }

        List<Faction> GetWinningFactions() {
            IEnumerable<Faction> winningfactions = Factions.FindAll(f => f.WinConditions.All(c => c.Check(f)));
            return winningfactions.ToList();
        }

        string GetWinMessage(List<Faction> winningFactions) {
            List<string> factionStrings = new List<string>();
            foreach (Faction faction in winningFactions) {
                List<string> members = (from m in faction.Members
                                        where m.State == MemberState.Alive
                                        select m.Name).ToList();
                members.AddRange(from m in faction.Members
                                 where m.State == MemberState.Dead
                                 select (m.Name + "(RIP)"));
                factionStrings.Add(faction.Name + "("+new ListFormatter<string>(members).Format()+")");
            }
            return "The game is over! Winning factions: " + new ListFormatter<string>(factionStrings) + ".";
        }

        List<VillageMember> GetAliveMembers() {
            IEnumerable<VillageMember> aliveMembers = from m in Members
                                                      where m.State == MemberState.Alive
                                                      select m;
            return aliveMembers.ToList();
        }
    }

    public class PhaseOverEventArgs : EventArgs {
        Phase oldPhase;
        public MafiaBotV2.MafiaLib.Phase OldPhase {
            get { return oldPhase; }
        }
        Phase newPhase;
        public MafiaBotV2.MafiaLib.Phase NewPhase {
            get { return newPhase; }
        }
        public PhaseOverEventArgs(Phase oldPhase, Phase newPhase) {
            this.oldPhase = oldPhase;
            this.newPhase = newPhase;
        }
    }
}

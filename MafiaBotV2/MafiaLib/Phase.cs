using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MafiaBotV2.Util;
using System.Linq;

namespace MafiaBotV2.MafiaLib
{
    public enum PhaseType
    {
        Dawn,
        Day,
        Twilight,
        Night,
        Night0,
        Over,
		Any
    }

    public class Phase
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler PhaseOver;

        Village village;

        PhaseType type;
        public MafiaBotV2.MafiaLib.PhaseType Type {
            get { return type; }
        }

        List<KeyValuePair<Actor, Power>> skipped = new List<KeyValuePair<Actor, Power>>();

        List<Action> actions = new List<Action>();
        public List<Action> Actions {
            get { return actions; }
        }

        MafiaVoteCounter votes;
        public MafiaBotV2.MafiaLib.MafiaVoteCounter Votes {
            get { return votes; }
        }

        public Phase(Village village, PhaseType initialPhase) {
            this.village = village;
            this.type = initialPhase;
            village.ActionExecuted += new EventHandler<ActionEventArgs>(OnActionExecuted);

            votes = new MafiaVoteCounter(village.AliveMembers);
            votes.MajorityReached += new EventHandler<MajorityReachedEventArgs<VillageMember>>(OnMajorityReached);
        }

        public bool HasAction(Actor actor, Power power) {
            return 
                Actions.Any(a => a.Who == actor && a.Power == power)
                    || skipped.Any(s => s.Key == actor && s.Value == power);
        }

        void OnActionExecuted(object sender, ActionEventArgs e) {
            if (Type == PhaseType.Day) {
                return;
            }

            foreach (VillageMember actor in village.Members) {
                if(actor.State == MemberState.Alive && actor.HasUnusedPowers) {
                    return;
                }
            }
            foreach(Actor actor in village.Factions) {
                if(actor.HasUnusedPowers) {
                    return;
                }
            }
            End();
        }

        void OnMajorityReached(object sender, MajorityReachedEventArgs<VillageMember> e) {
            Debug.Assert(e.Winner is VillageMember);

            if (e.Winner != Votes.NoLynchMember) {
                e.Winner.Kill(DeathCauseType.Lynch);
            }
            else {
                village.Outside.SendMessage("Noone was lynched today.");
            }
            End();
        }

        public void End() {
            Log.Debug("Phase is over.");
            village.ActionExecuted -= new EventHandler<ActionEventArgs>(OnActionExecuted);

            Log.Debug("Executing queued actions");
            while(actions.Count > 0) {
                SortActions();
                Action action = actions[0];
                actions.RemoveAt(0);

                Log.Debug("Handling action " + action.Power.Name + " by " + action.Who.Name + " on " + action.Target.Name);
                action.Who.RaisePreActionSource(action);
                action.Target.RaisePreActionTarget(action);
                if (action.Cancel) {
                    Log.Debug("Action cancelled");
                }
                else {
                    Log.Debug("Executing action");
                    action.Execute();

                    action.Who.RaisePostActionSource(action);
                    action.Target.RaisePostActionTarget(action);
                    if (action.Result != null) {
                        action.Result.Apply();
                    }
                }
            }

            if(PhaseOver != null) {
                PhaseOver(this, null);
            }
        }

        private void SortActions() {
            List<Action> sortedActions = new List<Action>();

            // First, find all roleblock actions, sort them and add them to the list
            List<Action> blocks = actions.FindAll(a => a.Power.IsRoleblocking);
            blocks = SortBlocks(blocks);
            sortedActions.AddRange(blocks);

            // Next, remove all blocks from actions, sort the remaining actions and add them as well
            actions.RemoveAll(a => a.Power.IsRoleblocking);
            actions.Sort();
            sortedActions.AddRange(actions);

            actions = sortedActions;
        }

        private List<Action> SortBlocks(List<Action> blocks) {
            List<Action> sortedBlocks = new List<Action>();

            while(blocks.Count > 0) {
                // Find an action that is not blocked by someone else. If none exists, the remaining
                // Actions are blocking each other. No need to consider the blocks that are already
                // sorted, as that will be figured out at execution.
                Action block = blocks.Find(a => !blocks.Exists(b => b.Target == a.Who));
                if(block == null) {
                    //Remove all actions by blocked members
                    foreach(Action b in actions.Where(a => !a.Power.IsRoleblocking && blocks.Exists(bl => bl.Target == a.Who))) {
                        actions.Remove(b);
                    }
                    break;
                }
                else {
                    sortedBlocks.Add(block);
                    blocks.Remove(block);
                }
            }
            return sortedBlocks;
        }

        public void QueueAction(Action action) {
            foreach(Action a in actions) {
                if(a.Who == action.Who && a.Power == action.Power) {
                    actions.Remove(a);
                    break;
                }
            }

            actions.Add(action);
        }

        public void SkipPower(Actor actor, Power power) {
        }
    }
}

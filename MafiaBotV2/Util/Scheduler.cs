using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Util
{
    delegate void ScheduledTaskHandler(Scheduler sender, object[] args);

    class Scheduler
    {
        List<Task> tasks;

        private Scheduler() {
            tasks = new List<Task>();
        }

        public void Execute() {
            List<Task> readyTasks = tasks.FindAll(task => DateTime.Now > task.Time);
            foreach(Task task in readyTasks) {
                task.Handler.Invoke(this, task.Args);
            }
            tasks.RemoveAll(task => readyTasks.Contains(task));

        }

        public void QueueTask(DateTime time, ScheduledTaskHandler task) {
            QueueTask(null, time, task);
        }

        public void QueueTask(object key, DateTime time, ScheduledTaskHandler task, params object[] args) {
            Task item = new Task(key, time, task, args);
            tasks.Add(item);
        }

        public void RemoveAll(object key) {
            tasks.RemoveAll(task => task.Key == key);
        }

        public void Clear() {
            tasks.Clear();
        }

        #region Scheduler Singleton
        public static Scheduler Instance {
            get {
                return NestedScheduler.instance;
            }
        }

        class NestedScheduler
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NestedScheduler() {
            }

            internal static readonly Scheduler instance = new Scheduler();
        }
        #endregion

        class Task
        {
            DateTime time;
            public System.DateTime Time {
                get { return time; }
            }
            ScheduledTaskHandler handler;
            public MafiaBotV2.Util.ScheduledTaskHandler Handler {
                get { return handler; }
            }
            object[] args;
            public object[] Args {
                get { return args; }
            }
            object key;
            public object Key {
                get { return key; }
            }

            public Task(object key, DateTime time, ScheduledTaskHandler task, object[] args) {
                this.key = key;
                this.time = time;
                this.handler = task;
                this.args = args;
            }
        }
    }
}

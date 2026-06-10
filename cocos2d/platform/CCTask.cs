using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    public static class CCTask
    {

        #region Threading
            /// <summary>
            /// Checks if the code is currently running on the UI thread.
            /// </summary>
            /// <returns>true if the code is currently running on the UI thread.</returns>
            public static bool IsOnUIThread()
            {
                return (true);

            }

            /// <summary>
            /// Throws an exception if the code is not currently running on the UI thread.
            /// </summary>
            /// <exception cref="InvalidOperationException">Thrown if the code is not currently running on the UI thread.</exception>
            public static void EnsureUIThread()
            {
            }

            public static void RunOnUiThread(Action action)
            {
                action();
            }

            /// <summary>
            /// Runs the given action on the UI thread and blocks the current thread while the action is running.
            /// If the current thread is the UI thread, the action will run immediately.
            /// </summary>
            /// <param name="action">The action to be run on the UI thread</param>
            public static void BlockOnUIThread(Action action)
            {
                action();
            }

        #endregion

        private class TaskSelector : ICCSelectorProtocol
        {
            public void Update(float dt)
            {
            }
        }

        private static ICCSelectorProtocol _taskSelector = new TaskSelector();

        public static void RunOnScheduler(Action action)
        {
            var scheduler = CCDirector.SharedDirector.Scheduler;
            scheduler.ScheduleSelector(f => action(), _taskSelector, 0, 0, 0, false);
        }

        public static object RunAsync(Action action)
        {
            return RunAsync(action, null);
        }
		
        public static object RunAsync(Action action, Action<object> taskCompleted)
        {
            var task = new Task(
                () =>
                {
                    action();

                    if (taskCompleted != null)
                    {
                        var scheduler = CCDirector.SharedDirector.Scheduler;
                        scheduler.ScheduleSelector(f => taskCompleted(null), _taskSelector, 0, 0, 0, false);
                    }
                }
                );

            task.Start();

            return task;
        }
    }
}

using System;
using Cocos2D;
using OpenGLES;
using System.Threading;
using cocos2d.EmbeddableView.OpenTK.Graphics.ES20;

namespace cocos2d.EmbeddableView.OpenTK
{
    public class Threading
    {
        public const int kMaxWaitForUIThread = 750; // In milliseconds

#if !WINDOWS_PHONE
        public static int mainThreadId;
#endif

#if ANDROID
        static List<Action> actions = new List<Action>();
        //static Mutex actionsMutex = new Mutex();
#elif IOS
        public static EAGLContext BackgroundContext;
#elif WINDOWS || DESKTOPGL || ANGLE
        public static IGraphicsContext BackgroundContext;
        public static IWindowInfo WindowInfo;
#endif

#if !WINDOWS_PHONE
        static Threading()
        {
#if WINDOWS_STOREAPP
            mainThreadId = Environment.CurrentManagedThreadId;
#else
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
        }
#endif

        /// <summary>
        /// Checks if the code is currently running on the UI thread.
        /// </summary>
        /// <returns>true if the code is currently running on the UI thread.</returns>
        public static bool IsOnUIThread()
        {
#if WINDOWS_PHONE
            return Deployment.Current.Dispatcher.CheckAccess();
#elif WINDOWS_STOREAPP
            return (mainThreadId == Environment.CurrentManagedThreadId);
#else
            return mainThreadId == Thread.CurrentThread.ManagedThreadId;
#endif
        }

        /// <summary>
        /// Throws an exception if the code is not currently running on the UI thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the code is not currently running on the UI thread.</exception>
        public static void EnsureUIThread()
        {
            if (!IsOnUIThread())
                throw new InvalidOperationException("Operation not called on UI thread.");
        }

#if WINDOWS_PHONE
        internal static void RunOnUIThread(Action action)
        {
            RunOnContainerThread(Deployment.Current.Dispatcher, action);
        }
        
        internal static void RunOnContainerThread(System.Windows.Threading.Dispatcher target, Action action)
        {
            target.BeginInvoke(action);
        }

        internal static void BlockOnContainerThread(System.Windows.Threading.Dispatcher target, Action action)
        {
            if (target.CheckAccess())
            {
                action();
            }
            else
            {
                EventWaitHandle wait = new AutoResetEvent(false);
                target.BeginInvoke(() =>
                {
                    action();
                    wait.Set();
                });
                wait.WaitOne(kMaxWaitForUIThread);
            }
        }
#endif


#if ANDROID
        public static OpenTK.Platform.Android.AndroidGameView GameView { get; set; }
#endif
        /// <summary>
        /// Runs the given action on the UI thread and blocks the current thread while the action is running.
        /// If the current thread is the UI thread, the action will run immediately.
        /// </summary>
        /// <param name="action">The action to be run on the UI thread</param>
        internal static void BlockOnUIThread(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

#if (DIRECTX && !WINDOWS_PHONE) || PSM
            action();
#else
            // If we are already on the UI thread, just call the action and be done with it
            if (IsOnUIThread())
            {
#if WINDOWS_PHONE
                try
                {
                    action();
                }
                catch (UnauthorizedAccessException)
                {
                    // Need to be on a different thread
                    BlockOnContainerThread(Deployment.Current.Dispatcher, action);
                }
#else
                action();
#endif
                return;
            }

#if IOS
            lock (BackgroundContext)
            {
                // Make the context current on this thread if it is not already
                if (!Object.ReferenceEquals(EAGLContext.CurrentContext, BackgroundContext))
                    EAGLContext.SetCurrentContext(BackgroundContext);
                // Execute the action
                action();
                // Must flush the GL calls so the GPU asset is ready for the main context to use it
                GL.Flush();
                OpenTK.Graphics.GraphicsExtensions.CheckGLError();
            }
#elif WINDOWS || DESKTOPGL || ANGLE
            lock (BackgroundContext)
            {
                // Make the context current on this thread
                BackgroundContext.MakeCurrent(WindowInfo);
                // Execute the action
                action();
                // Must flush the GL calls so the texture is ready for the main context to use
                GL.Flush();
                GraphicsExtensions.CheckGLError();
                // Must make the context not current on this thread or the next thread will get error 170 from the MakeCurrent call
                BackgroundContext.MakeCurrent(null);
            }
#elif WINDOWS_PHONE
            BlockOnContainerThread(Deployment.Current.Dispatcher, action);
#else
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
#if MONOMAC
            MonoMac.AppKit.NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
#else
            Add(() =>
#endif
            {
#if ANDROID
                //if (!Game.Instance.Window.GraphicsContext.IsCurrent)
                if (GameView != null)
                    GameView.MakeCurrent();
#endif
                action();
                resetEvent.Set();
            });
            resetEvent.Wait();
#endif
#endif
        }

#if ANDROID
        static void Add(Action action)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// Runs all pending actions.  Must be called from the UI thread.
        /// </summary>
        internal static void Run()
        {
            EnsureUIThread();

            lock (actions)
            {
                foreach (Action action in actions)
                {
                    action();
                }
                actions.Clear();
            }
        }
#endif
    }
}


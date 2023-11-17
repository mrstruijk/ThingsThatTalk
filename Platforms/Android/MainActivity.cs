using System;
using System.Reflection;
using System.Threading;
using _Microsoft.Android.Resource.Designer;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using ProjectName;
using StereoKit;


namespace ThingsThatTalk
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Exported = true)]
    [IntentFilter(new[] {Intent.ActionMain}, Categories = new[] {"org.khronos.openxr.intent.category.IMMERSIVE_HMD", "com.oculus.intent.category.VR", Intent.CategoryLauncher})]
    public class MainActivity : Activity, ISurfaceHolderCallback2
    {
        private View surface;

        private static bool running = false;


        // Events related to surface state changes
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            SK.SetWindow(holder.Surface.Handle);
        }


        public void SurfaceCreated(ISurfaceHolder holder)
        {
            SK.SetWindow(holder.Surface.Handle);
        }


        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            SK.SetWindow(IntPtr.Zero);
        }


        public void SurfaceRedrawNeeded(ISurfaceHolder holder)
        {
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Run();
            SetContentView(ResourceConstant.Layout.activity_main);
        }


        protected override void OnDestroy()
        {
            SK.Quit();
            base.OnDestroy();
        }


        private void Run()
        {
            if (running)
            {
                return;
            }

            running = true;

            // Before anything else, give StereoKit the Activity. This should
            // be set before any other SK calls, otherwise native library
            // loading may fail.
            SK.AndroidActivity = this;

            // Set up a surface for StereoKit to draw on, this is only really
            // important for flatscreen experiences.
            Window.TakeSurface(this);
            Window.SetFormat(Format.Unknown);
            surface = new View(this);
            SetContentView(surface);
            surface.RequestFocus();

            // Task.Run will eat exceptions, but Thread.Start doesn't seem to.
            new Thread(InvokeStereoKit).Start();
        }


        private static void InvokeStereoKit()
        {
            var entryClass = typeof(Program);
            var entryPoint = entryClass?.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // There are a number of potential method signatures for Main, so
            // we need to check each one, and give it the correct values.
            //
            // Converting MethodInfo into an Action instead of calling Invoke on
            // it allows for exceptions to properly bubble up to the IDE.
            var entryParams = entryPoint?.GetParameters();

            if (entryParams == null || entryParams.Length == 0)
            {
                var Program_Main = (Action) Delegate.CreateDelegate(typeof(Action), entryPoint);
                Program_Main();
            }
            else if (entryParams?.Length == 1 && entryParams[0].ParameterType == typeof(string[]))
            {
                var Program_Main = (Action<string[]>) Delegate.CreateDelegate(typeof(Action<string[]>), entryPoint);
                Program_Main(new string[] { });
            }
            else
            {
                throw new Exception("Couldn't invoke Program.Main!");
            }

            Process.KillProcess(Process.MyPid());
        }
    }
}
using System;
using Foundation;
using UIKit;

namespace Xmas_Hell_iOS
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        private static XmasHell.XmasHell game;

        internal static void RunGame()
        {
            game = new XmasHell.XmasHell();
            game.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
    }
}

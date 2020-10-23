//YobbinCallouts by YobB1n

using System;
using System.Net;
using System.Threading;
using LSPD_First_Response.Mod.API;
using Rage;


namespace YobbinCallouts
{
    public class Main : Plugin
    {
        private Version NewVersion = new Version();
        private Version curVersion = new Version("1.1.0");  //DON'T FORGET TO CHANGE THIS, MATTHEW!!!

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("YobbinCallouts " + curVersion + " by YobB1n has been loaded.");   //Returns Callout Version
            //
            try
            {
                Thread FetchVersionThread = new Thread(() =>
                {

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            string s = client.DownloadString("http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=29467&textOnly=1");

                            NewVersion = new Version(s);
                        }
                        catch (Exception e) { Game.LogTrivial("YOBBINCALLOUTS: LSPDFR Update API down. Aborting checks."); }
                    }
                });
                FetchVersionThread.Start();
                while (FetchVersionThread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    GameFiber.Yield();
                }
                // compare the versions  
                if (curVersion.CompareTo(NewVersion) < 0)
                {
                    Game.LogTrivial("YOBBINCALLOUTS: Update Available for Yobbin Callouts. Installed Version " + curVersion + "New Version " + NewVersion);
                    Game.DisplayNotification("~g~Update Available~w~ for ~b~Yobbin Callouts~w~.\nInstalled Version: ~y~" + curVersion + "\n~w~New Version~y~ " + NewVersion);
                }
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Game.LogTrivial("YOBBINCALLOUTS: Error while checking Yobbin Callouts for updates.");
            }
            catch (Exception e)
            {
                Game.LogTrivial("YOBBINCALLOUTS: Error while checking Yobbin Callouts for updates.");
            }
        }
        public override void Finally()
        {
            Game.LogTrivial("YOBBINCALLOUTS: YobbinCallouts has been cleaned up.");
        }
        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("~b~YobbinCallouts ~s~by ~b~YobB1n ~g~Loaded Successfully.");
            }
        }
        private static void RegisterCallouts()
        {

            Game.LogTrivial("==========YOBBINCALLOUTS INFORMATION==========");
            Game.LogTrivial("YobbinCallouts by YobB1n");
            Game.LogTrivial("Version 1.1.0");
            if (Config.INIFile.Exists())
            {
                Game.LogTrivial("YOBBINCALLOUTS: YobbinCallouts Config Installed by User.");
            }
            else
            {
                Game.LogTrivial("YOBBINCALLOUTS: YobbinCallouts Config NOT Installed by User.");
            }
            Game.LogTrivial("Please Join My Discord Server: https://discord.gg/Jv6D3Wa. Enjoy!");
            Game.LogTrivial("Please Report all Bugs in the Discord Server.");
            Game.LogTrivial("==========YOBBINCALLOUTS INFORMATION==========");

            if (Config.BrokenDownVehicle == true || !Config.INIFile.Exists())
            {
                //Functions.RegisterCallout(typeof(Callouts.BrokenDownVehicle));
                Game.LogTrivial("YOBBINCALLOUTS: Broken Down Vehicle Callout Registered."); //Temporarily Disabled While I Fix Something
            }
            if (Config.AssaultOnBus == true || !Config.INIFile.Exists())
            {
                Functions.RegisterCallout(typeof(Callouts.NewAssaultOnBus));
                Game.LogTrivial("YOBBINCALLOUTS: Assault on Bus Callout Registered.");
            }
            if (Config.ShotSpotter == true || !Config.INIFile.Exists())
            {
                Functions.RegisterCallout(typeof(Callouts.ShotSpotter));    //Shot Spotter Callout Under Development Right Now
                Game.LogTrivial("YOBBINCALLOUTS: Shot Spotter Callout Registered.");
            }
            if (Config.TrafficBreak == true || !Config.INIFile.Exists())
            {
                Functions.RegisterCallout(typeof(Callouts.TrafficBreak));    //Also under development
                Game.LogTrivial("YOBBINCALLOUTS: Traffic Break Callout Registered.");  
            }
        }
    }
}

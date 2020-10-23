using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;

namespace YobbinCallouts.Callouts
{
    [CalloutInfo("Traffic Break", CalloutProbability.High)]

    public class TrafficBreak : Callout
    {
        private Vector3 MainSpawnPoint;
        private Vector3 MainDestination;
        private Vector3 BeachSpawnPoint;

        private Blip Area;

        private uint MainTrafficBreak;

        private int MainScenario;

        private bool HelpMessage = false;

        private readonly List<string> Instructions = new List<string>()
        {
         "Activate Your ~y~Emergency Lights~w~ to Slow Traffic Down. Aim for a ~g~Slow Speed~w~ to Keep ~b~Traffic Moving~w~, but Only Just.",
         "Once You Reach the Yellow ~y~Waypoint,~w~ Cancel Your Emergency Lights and Let Traffic ~g~Speed Up~w~ Again.",
        };
        private int InstructionsCount;
        public override bool OnBeforeCalloutDisplayed()
        {
            Game.LogTrivial("==========YOBBINCALLOUTS: Traffic Break Callout Start==========");

            if (NativeFunction.Natives.GET_ZONE_AT_COORDS(Game.LocalPlayer.Character.Position) == "BEACH")  //gonna get rid of this. Old method to select a spawnpoint doesn't work well.
            {
                BeachSpawnPoint = new Vector3(-2011.691f, -446.1866f, 11.36965f);
                MainSpawnPoint = World.GetNextPositionOnStreet(BeachSpawnPoint);
                MainDestination = new Vector3(-1687.246f, -719.8373f, 10.88418f);
            }
            else
            {
                Game.LogTrivial("YOBBINCALLOUTS: No Close Freeway Spawnpoint Found.");
                BeachSpawnPoint = new Vector3(-2011.691f, -446.1866f, 11.36965f);
                MainSpawnPoint = World.GetNextPositionOnStreet(BeachSpawnPoint);
            }
            ShowCalloutAreaBlipBeforeAccepting(MainSpawnPoint, 100);    //Callout Blip Circle with radius 100m
            AddMinimumDistanceCheck(150f, MainSpawnPoint);   //Player must be 150m or further away

            CalloutMessage = "Traffic Break";
            CalloutPosition = MainSpawnPoint;

            Functions.PlayScannerAudio("WE_HAVE_01");   //Add more audio later
            GameFiber.Wait(1000);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("YOBBINCALLOUTS: Traffic Break Callout Accepted by User");
            GameFiber.Wait(2000);
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Area = new Blip(MainSpawnPoint, 125);
            Area.Color = System.Drawing.Color.Yellow;

            System.Random r = new System.Random();
            int Scenario = r.Next(0, 0);
            switch (Scenario)   //Scenario Chooser, I don't think I'm gonna add another one fore a while
            {
                case 0:
                    MainScenario = 0;

                    Game.LogTrivial("YOBBINCALLOUTS: Traffic Break Scenario 0 Chosen");
                    Game.DisplayHelp("Go to the ~y~Area~w~ Shown on The Map to Start the Traffic Break.");
                    Area.IsRouteEnabled = true;

                    break;
            }
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            Game.LogTrivial("YOBBINCALLOUTS: Traffic Break Callout Not Accepted by User.");
            //Functions.PlayScannerAudio("OTHER_UNIT_TAKING_CALL_01");  //this gets annoying after a while
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            base.Process();
            if (MainScenario == 0)
            {
                while (Game.LocalPlayer.Character.DistanceTo(MainSpawnPoint) >= 125f && !Game.IsKeyDown(System.Windows.Forms.Keys.End))
                {
                    MainTrafficBreak = World.AddSpeedZone(MainSpawnPoint, 125, 12f);    //Add a speedzone when the player gets within 125m of the scene.
                    GameFiber.Wait(0);
                }
                if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
                {
                    End();  //End the callout if player presses 'End' before reaching scene
                }
                while (Game.LocalPlayer.Character.DistanceTo(MainSpawnPoint) <= 125f)
                {
                    Game.LogTrivial("YOBBINCALLOUTS: Player is On Scene.");
                    Game.DisplaySubtitle("Press 'Y' to ~g~Start~w~ the Traffic Break.", 2000);
                    while (InstructionsCount < Instructions.Count)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            Game.DisplaySubtitle(Instructions[InstructionsCount]);
                            InstructionsCount++;
                        }
                    }
                    GameFiber.Wait(1000);
                    Game.DisplayNotification("Dispatch, We are Starting the ~y~Traffic Break.");
                    GameFiber.Wait(1000);
                    Area.Delete();
                    World.RemoveSpeedZone(MainTrafficBreak);
                    GameFiber.Wait(500);
                    break;
                }
                while (!Game.IsKeyDown(Keys.End))
                {
                    MainTrafficBreak = World.AddSpeedZone(Game.LocalPlayer.Character.Position, 125, 12f);
                    GameFiber.Wait(0);
                }
                GameFiber.Wait(2000);
                Game.DisplayNotification("Dispatch, Traffic Break ~r~Over. ~y~Traffic Moving Back to Normal.");
                GameFiber.Wait(3000);
                Functions.PlayScannerAudio("REPORT_RESPONSE_COPY_02");
                Area.Delete();

                GameFiber.Wait(1000);
                End();
            }
        }
        public override void End()
        {
            base.End();
            Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
            if (Area.Exists()) { Area.Delete(); }
            Game.LogTrivial("YOBBINCALLOUTS: Traffic Break Callout Finished Cleaning Up.");
        }
    }
}

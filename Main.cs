using LSPD_First_Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.IO;

namespace crime_sim
{
    public class Main : Plugin
    {

        // declare global variables
        public static string[] stolenVehicles = { };

        private void LoadFromIni(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Game.LogTrivial("crime_sim.ini file not found.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("Stolen Vehicles="))
                {
                    string vehiclesString = line.Substring("Stolen Vehicles=".Length);
                    stolenVehicles = vehiclesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Crime Simulator " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            Game.LogTrivial("Go on duty to fully load the Crime Simulator.");

            string filePath = "Plugins/LSPDFR/crime_sim.ini";
            // Load keybindings
            LoadFromIni(filePath);

            
        }
        public override void Finally()
        {
            Game.LogTrivial("Crime Simulator has been cleaned up.");
        }
        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~w~Crime Simulator", "~y~v " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "~o~ by KS","~b~has been initialised. ~g~Reporting for duty!");
                //Game.DisplayNotification("Crime Simulator has loaded successfully, thank you for downloading!");
            }
        }
        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.StolenMoped));
            Functions.RegisterCallout(typeof(Callouts.StolenVehicle));
            //Functions.RegisterCallout(typeof(Callouts.PoliceEscort));
        }
    }
}
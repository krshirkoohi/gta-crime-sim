using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System.Windows.Forms;
using LSPD_First_Response.Engine;
using System.Runtime.CompilerServices;
using NAudio.Wave;
using System.Globalization;

namespace crime_sim.Callouts
{
    [CalloutInfo("StolenVehicle", CalloutProbability.Low)]
    public class StolenVehicle : Callout
    {
        public LHandle pursuit;
        public Vector3 spawnPos;
        public Blip driverBlip;
        public Ped driver, rider1, rider2, rider3;
        public Vehicle vehicle;
        public Boolean isPursuit;
        public Boolean halfFin;
        public int refNum;
        public string CRN;
        private string[] vehicles = crime_sim.Main.stolenVehicles;
        /*private string[] vehicles = { // Vehicles most likely to be stolen and used by a gang
            "baller2",
            "baller3",
            "mesa",
            "patriot",
            "dubsta",
            "dubsta2",
            "schafter2",
            "schafter3",
            "fusilade",
            "schwarzer",
            "tailgater",
            "sentinel2",
            "sentinel",
            //"xls",
            "prairie",
            "fugitive",
            "feltzer2",
            "rhinehart"
        };*/

        private Random rand = new Random();
        private Random randCRN = new Random();

        public override bool OnBeforeCalloutDisplayed()
        {

            // Crime reference number
            refNum = randCRN.Next(10000,700000); 
            CRN = "CAD/" + refNum.ToString() + "/" + "23";

            // Create
            spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            int r = rand.Next(vehicles.Length);
            vehicle = new Vehicle(vehicles[r], spawnPos) { IsPersistent = true };

            // Setup
            driver = new Ped("mp_g_m_pros_01", spawnPos, 0) { IsPersistent = true, BlockPermanentEvents = false };
            rider1 = new Ped("mp_g_m_pros_01", spawnPos, 0) { IsPersistent = true, BlockPermanentEvents = false };
            driver.SetVariation(9, 1, 0); driver.SetVariation(11, 2, 0); driver.Inventory.GiveNewWeapon("weapon_crowbar", 1, true);
            rider1.SetVariation(9, 1, 0); rider1.SetVariation(11, 2, 0); rider1.Inventory.GiveNewWeapon("weapon_hammer", 1, true);
            if (vehicle.FreeSeatsCount > 2)
            {
                rider2 = new Ped("mp_g_m_pros_01", spawnPos, 0) { IsPersistent = true, BlockPermanentEvents = false };
                rider3 = new Ped("mp_g_m_pros_01", spawnPos, 0) { IsPersistent = true, BlockPermanentEvents = false };
                rider2.SetVariation(9, 1, 0); rider2.SetVariation(11, 2, 0); rider2.Inventory.GiveNewWeapon("weapon_hammer", 1, false);
                rider3.SetVariation(9, 1, 0); rider3.SetVariation(11, 2, 0); rider3.Inventory.GiveNewWeapon("weapon_hammer", 1, false);
                rider2.WarpIntoVehicle(vehicle, 1);
                rider3.WarpIntoVehicle(vehicle, 2);
            }
            driver.WarpIntoVehicle(vehicle, -1);
            rider1.WarpIntoVehicle(vehicle, -2);
            if (driver && vehicle)
            {
                driver.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);
            }

            // Handle UI
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPos, 15f);
            this.AddMinimumDistanceCheck(5f, spawnPos);
            this.CalloutMessage = "Stolen Vehicle";
            this.CalloutPosition = spawnPos;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", spawnPos);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {

            isPursuit = false;
            driverBlip = new Blip(driver)
            {
                IsFriendly = false,
                Color = Color.Red,
                IsRouteEnabled = true,
            };
            //suspectBlip = suspect.AttachBlip();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~b~Incident Reported~b~", "Stolen vehicle involved with violent crime sighted by ANPR. ~y~Caution: possibly armed.~w~");
            //Game.DisplaySubtitle("Detain all occupants. Be careful, they may be ~r~armed~w~.", 7500);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            CleanUp();
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            base.Process();
            {
                if (Game.LocalPlayer.Character.Position.DistanceTo(vehicle) <= 50f && isPursuit == false)
                {
                    this.pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(this.pursuit, driver);
                    Functions.AddPedToPursuit(this.pursuit, rider1);
                    if (vehicle.FreeSeatsCount > 2) { 
                        Functions.AddPedToPursuit(this.pursuit, rider2);
                        Functions.AddPedToPursuit(this.pursuit, rider3);
                    }
                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    if (driverBlip) driverBlip.Delete();
                    isPursuit = true; 
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~r~Pursuit Authorised~w~", "Stop the vehicle by all possible means. Press 'End' when finished.");
                    /*WaveStream mainOutputStream = new Mp3FileReader("Plugins/LSPDFR/crime_sim/Audio/Stolen Vehicle Alert.wav");
                    WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
                    WaveOutEvent player = new WaveOutEvent();
                    player.Init(volumeStream);
                    player.Play();*/
                }
                if (Game.IsKeyDown(Keys.End))
                {
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~b~Incident Result", "Incident closed at " + (DateTime.Now.ToString("HH:mm")) + ".");
                    this.End();
                }
            }
        }
        public override void End()
        {
            CleanUp();
            base.End();
        }
        public void CleanUp()
        {
            if (driver.Exists()) driver.Delete();
            if (rider1.Exists()) rider1.Delete();
            if (rider2.Exists()) rider2.Delete();
            if (rider3.Exists()) rider3.Delete();
            if (vehicle.Exists()) vehicle.Delete();
            if (driverBlip.Exists()) driverBlip.Delete();
        }
    }
}

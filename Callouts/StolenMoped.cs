using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSPD_First_Response.Engine;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using NAudio.Wave;
using Rage;

namespace crime_sim.Callouts
{
    [CalloutInfo("StolenMoped", CalloutProbability.Low)]
    public class StolenMoped : Callout
    {
        public LHandle pursuit;
        public Vector3 spawnPos;
        public Blip suspectBlip;
        public Ped suspect, rider;
        public Vehicle moped;
        public Boolean isPursuit;
        public Boolean halfFin;
        public int refNum;
        public string CRN;
        private Random randCRN = new Random();
        private Random rand = new Random();

        public override bool OnBeforeCalloutDisplayed()
        {
            // Crime reference number
            refNum = randCRN.Next(10000, 700000);
            CRN = "CAD/" + refNum.ToString() + "/" + (DateTime.Now.Year - 2000); 
            
            // Create
            spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            moped = new Vehicle("FAGGIO", spawnPos); moped.IsPersistent = true;

            //int chance = rand.Next(0,1);
            //if (chance <= 0.25)
            //{

                //suspect = new Ped("g_m_y_ballaeast_01", spawnPos, 0); suspect.IsPersistent = true; //suspect.SetVariation(12, 0, 0);
                //rider = new Ped("g_f_y_families_01", spawnPos, 0); suspect.IsPersistent = true; rider.SetVariation(2, 0, 0); //rider.SetVariation(12, 0, 0);
            //} 
            //else
            //{
                suspect = new Ped("g_m_y_ballaeast_01", spawnPos, 0); suspect.IsPersistent = true; //suspect.SetVariation(12, 0, 0);
                rider = new Ped("g_m_y_ballaeast_01", spawnPos, 0); suspect.IsPersistent = true; //rider.SetVariation(12, 0, 0);
            //}
            
            // Adjust
            moped.PrimaryColor = Color.Black;
            moped.SecondaryColor = Color.Black;
            moped.LicensePlate = "BY22 NMX";
            //suspect.SetVariation(9, 1, 0); suspect.SetVariation(11, 2, 0); 
            suspect.Inventory.GiveNewWeapon("weapon_hammer", 1, false);
            //rider.SetVariation(9, 1, 0); rider.SetVariation(11, 2, 0); 
            rider.Inventory.GiveNewWeapon("weapon_crowbar", 1, true);

            suspect.GiveHelmet(false, HelmetTypes.RegularMotorcycleHelmet, 1);
            rider.GiveHelmet(false, HelmetTypes.RegularMotorcycleHelmet, 1);

            // Setup
            if (!moped.Exists() || !suspect.Exists() || !rider.Exists()) return false;
            suspect.WarpIntoVehicle(moped, -1); rider.WarpIntoVehicle(moped, 0);
            suspect.BlockPermanentEvents = true; rider.BlockPermanentEvents = true;
            suspect.Tasks.CruiseWithVehicle(moped, 20f, VehicleDrivingFlags.None);

            // Handle UI
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPos, 15f);
            this.AddMinimumDistanceCheck(5f, spawnPos);

            this.CalloutMessage = "Stolen Moped";
            this.CalloutPosition = spawnPos;
            Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT CRIME_RESIST_ARREST IN_OR_ON_POSITION", this.spawnPos);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            isPursuit = false;
            halfFin = false;
            suspectBlip = new Blip(suspect)
            {
                IsFriendly = false,
                Color = Color.Red,
                IsRouteEnabled = true,
            };
            //suspectBlip = suspect.AttachBlip();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~b~Incident Reported~b~", "Moped involved in violent crime sighted by CCTV. ~y~Caution: possibly armed.~w~");
            //Game.DisplaySubtitle("Stop the ~r~suspects~w~ in the act!", 7500);
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
                if (Game.LocalPlayer.Character.Position.DistanceTo(moped) <= 50f && isPursuit == false)
                {
                    this.pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(this.pursuit, suspect);
                    Functions.AddPedToPursuit(this.pursuit, rider);
                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    if (suspectBlip) suspectBlip.Delete();
                    isPursuit = true;
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~r~Pursuit Authorised~w~", "Stop the vehicle by all possible means. Press 'End' when finished.");

                    /*WaveStream mainOutputStream = new Mp3FileReader("Plugins/LSPDFR/crime_sim/Audio/Stolen Vehicle Alert.wav");
                    WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
                    WaveOutEvent player = new WaveOutEvent();
                    player.Init(volumeStream);
                    player.Play();*/

                    //if (suspectBlip.Exists()) suspectBlip.Delete();
                }
                if (
                    ((suspect.IsCuffed && !(rider.IsCuffed)) || 
                    (rider.IsCuffed && !(suspect.IsCuffed)) ||
                    (rider.IsDead && !(suspect.IsCuffed)) ||
                    (rider.IsCuffed && !(suspect.IsDead))) &&
                    halfFin == false)
                {
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~y~Incident Update", "There is still another suspect at large!");
                    halfFin = true;
                }
                if ((suspect.IsCuffed && rider.IsCuffed) ||
                    (suspect.IsDead && rider.IsCuffed) ||
                    (suspect.IsCuffed && rider.IsDead) ||
                    (suspect.IsDead && rider.IsDead))
                {
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", CRN, "~b~Incident Result", "All suspects apprehended.");
                    base.End(); suspect.IsPersistent = false; rider.IsPersistent = false; moped.IsPersistent = false;
                    suspectBlip.Delete();
                    //this.End();
                }
                if (!suspect && !rider && !moped)
                {
                    this.End();
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
            if (suspect.Exists()) suspect.Delete();
            if (rider.Exists()) rider.Delete();
            if (moped.Exists()) moped.Delete();
            if (suspectBlip.Exists()) suspectBlip.Delete();
        }
    }
}

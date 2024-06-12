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

namespace crime_sim.Callouts
{
    [CalloutInfo("Police Escort", CalloutProbability.Medium)]
    public class Coronation : Callout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            // Create convoy

            // Populate convoy

            // Handle UI

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            // Create waypoint

            // Provide context

            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            // Clean up
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            // When player arrives, prompt them

            // Press 'Y' to start the convoy

            // Start AI tasks
            // Car A heads to WA
            // Car B follows Car A, Car C follows Car B, and so on
            // Helicopter joins in after one minute or so and follows from above
            // Helicopter is the dialogue for spotting roadblocks
            
            // Avoid obstacles and keep driving
            // Drivers and passengers are NOT police, or they will stop at disruptions

            // When entering the city, there will be a 50% chance of a Just Stop Oil roadblock
            // If so, notify the player and get them to remove the protesters one-by-one

            // Callout is successful when Car A/Royal Car arrives at WA

            base.Process();
        }
        public override void End()
        {

            base.End();
        }
    }
}

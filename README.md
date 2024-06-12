# gta-crime-sim

## Overview
For GTA 5 using the LSPDFR framework.

## Features
- **Stolen Vehicle Pursuits:** Respond to callouts involving stolen vehicles.
- **Police Escorts:** Manage and execute police escort missions.
- **Configurable Settings:** Customize the list of stolen vehicles via an `ini` file.

## Files
- **Main.cs:** The main plugin file responsible for initializing the plugin and registering callouts.
- **PoliceEscort.cs:** Handles the police escort missions.
- **StolenMoped.cs:** Manages callouts for stolen mopeds.
- **StolenVehicle.cs:** Handles callouts for stolen vehicles.
- **crime_sim.ini:** Configuration file for customizing the plugin settings.

## Installation
1. Ensure you have LSPDFR installed along with the necessary dependencies.
2. Compile the source code files `Main.cs`, `PoliceEscort.cs`, `StolenMoped.cs`, `StolenVehicle.cs` using Microsoft Visual Studio C# for the build **.NET Framework 4.6** (very important) to get the `crime_sim.dll` file.
3. Place the `crime_sim.dll`, and `crime_sim.ini` files in the `Plugins/LSPDFR` directory of your GTA V installation.
4. Compile the C# files into a DLL and place the compiled DLL in the `Plugins` directory.

## Configuration
The `crime_sim.ini` file allows you to specify which vehicles are considered stolen for the purposes of the callouts. Edit this file to include the vehicle model names, separated by commas.

### Example `crime_sim.ini`
```ini
Stolen Vehicles=baller2,baller3,buffalo

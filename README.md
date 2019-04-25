# Volume Rocker

Pin volume of clients to the server's master volume.

Simple and fun toy to mess around with colleagues. 

# Prerequisites

* Visual Studio
* .NET Framework 4.0
* Unsuspecting victims :)

# Configuration & Build

* Change Admin/Settings.cs with your own port and ip / hostname.

* Compile solution and distribute User/bin/Release to victims.
  > You can always rename the resulting .exe to "TestUI.exe" and ask the victim if an UI pops up.

# Running & Implementation Details

* The code does *not* set itself up to run at Startup. Too evil.
* The client exits if no connection can be established.
* Clients start pinning the volume after it connects.
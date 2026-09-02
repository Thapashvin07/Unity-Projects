Setup Instructions:


* Unity Editor > 2022.
* Clone the repository and add the cloned project folder to your new unity project.
* Ensure you are connected to the internet.
* Photon Fusion Setup:
   1. Open Window → Fusion → Fusion Hub.
   2. Check for app id, else create app id using Photon official website steps.
* Navigate to Scenes → MenuScene.
* Play in Editor (Multiplayer Test)
   1. Build and run the project for connecting another player.
   2. Click play in both instances.


Networking Solution:
* This project uses Photon Fusion 2  for multiplayer networking.
* Networking Mode : Shared Mode
* Player Count: 1v1 (2 players)
* Shared Mode allows both players to simulate the game by synchronising their states using network properties without a dedicated server.


Reveal Sequence and Initiative logic:
* When the turn timer becomes 0 the reveal phase happens for both players at the same time using reveal state, else if the player ends the turn his/her reveal phase happens.
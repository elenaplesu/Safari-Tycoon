# Safari Tycoon 

Welcome to **Safari Tycoon**, a wildlife park management simulation game built using C# and Windows Forms (.NET Framework 4.7.2).

Manage a thriving safari, balance your budget, and ensure your animals flourish while keeping them safe from poachers!

## Features

- **Dynamic Animal Ecosystem:** Animals age (Young, Adult, Old), require food and water, reproduce, and roam naturally. Currently features Lions, Tigers, Giraffes, and Rhinoceroses.
- **Economy & Infrastructure:** Build roads, ponds, bushes, and trees. Buy items from the Market to upgrade your park.
- **Jeep Tours:** Send Jeeps on tours across your custom road networks to bring in visitors and generate capital.
- **Threats & Protection:** Poachers will occasionally spawn and try to steal your animals. Deploy Rangers to patrol and protect your wildlife.
- **Difficulty Levels:** Choose between Easy (Weeks), Medium (Days), and Hard (Hours) modes, which scale game timers and survival win conditions.
- **Interactive Map:** Features a scrollable tiled map viewport with a toggleable minimap. Place down infrastructure and scenery seamlessly using the in-game UI.

## Gameplay & Controls

- **Starting out:** Use your initial capital to buy infrastructure (roads, ponds) and animals from the Market.
- **Building:** Click the top UI buttons to select an action (e.g., "Place Road", "Place Pond"), then click on empty grass tiles to place them.
- **Jeep Tours:** Build a connected road network with entry and exit doors, then click the **Travel** button to dispatch a Jeep and earn money.
- **Keyboard Controls:**
  - **Arrow Keys:** Pan the camera around the park.
  - **M:** Toggle the Minimap on the bottom right.

## Winning & Losing

- **Win Condition:** Achieve a stable park with at least **40 visitors**, **5 Carnivores**, **5 Herbivores**, and **$5000 capital**. You must sustain these minimum requirements for 60 consecutive time-ticks (weeks/days/hours depending on your chosen difficulty).
- **Lose Condition:** Run out of both capital ($0) and animals.

## Technical Requirements

- Windows OS
- .NET Framework 4.7.2
- Visual Studio (recommended for building)
- Included `System.Drawing` dependencies

## How to Run

1. Clone this repository to your local machine.
2. Open the project/solution in Visual Studio.
3. **Asset Setup:** The game requires image sprites to render tiles and entities. Ensure you have the `src/` folder (containing images like `grass.jpg`, `water.jpg`, `tiger.png`, `poacher.png`, etc.) in the same directory as your compiled executable (usually `bin/Debug` or `bin/Release`).
4. Build and Run the project (`F5`).

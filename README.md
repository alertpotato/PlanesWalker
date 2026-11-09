# PlanesWalker

A personal **card-based tactical-combat prototype** built with **Unity and C#**. The project explores unit abilities, planned actions, turn resolution and the interaction between unit stats and battlefield position.

Prototype development was intermittent from **August 2023 to August 2025**, following an initial repository setup in February 2022. This is an experimental personal project, not a finished or commercially released game.

## Try an earlier build

- [Prototype V1](https://drive.google.com/file/d/1Qh-TkL4kCk4BqsMDOLCXyL1BtkgUn9Ci/view?usp=sharing)
- [Prototype V2](https://drive.google.com/file/d/11L_-2gjfT28UW9zROj5E1n2aUGXrig0R/view?usp=sharing)

These are the historical builds published with the project. They represent earlier iterations and may differ from the source branches.

## What I worked on

- Unit abilities and interactions, including melee attacks, ranged attacks, charges and retaliation.
- Turn resolution and action ordering.
- Grid-based deployment, positioning and targeting.
- Unit stats, weighted stat progression and upgrade rules.
- A reusable ability/effect structure, with gameplay rules separated from the interaction layer.
- Design documentation for encounters, rewards, resource use and the intended progression loop.

## Combat structure

The prototype separates planning from resolution:

1. Deploy the available units on the battlefield.
2. Select actions and targets through the on-screen UI.
3. Resolve the planned actions in the round's action order.
4. Evaluate the resulting unit state and plan the next round.

The broader design proposes an encounter loop with rewards and changing world conditions. Meta progression, world generation and all planned content should not be assumed to be fully implemented in the downloadable prototypes.

## Design example: cohesion

The design explores **cohesion** as a way to change how incoming damage translates into casualties within a squad. The intended choice is whether to keep a unit engaged or preserve its effectiveness through positioning, abilities and time out of combat.

This makes cohesion both a combat parameter and a pacing constraint: remaining engaged can become progressively more dangerous. The original notes contain the early calculations and examples; they are historical working notes rather than a validated balancing specification.

- [Original design notes, preserved at their published revision](https://github.com/alertpotato/PlanesWalker/blob/4760c7b/README.md)

## Code walkthrough

| Area | Entry point | What to look for |
| --- | --- | --- |
| Ability rules | [UnitAbility.cs](Assets/Components/Unit/Abilities/UnitAbility.cs) | The common structure used by unit actions |
| Concrete abilities | [ArrowVolleyAbility.cs](Assets/Components/Unit/Abilities/ArrowVolleyAbility.cs), [MountedChargeAbility.cs](Assets/Components/Unit/Abilities/MountedChargeAbility.cs) | Different actions built on that structure |
| Round flow | [GameLoopRoundState.cs](Assets/Components/GameLoop/GameLoopRoundState.cs) | Sequencing a combat round |
| Battlefield rules | [BattlefieldLogic.cs](Assets/Components/Battlefield/BattlefieldLogic.cs) | Battlefield interactions and targeting rules |
| Unit data | [ArmyUnitClass.cs](Assets/Components/Unit/ArmyUnitClass.cs) | Unit characteristics and configuration |

## Open in Unity

1. Clone or download the repository's default **`Master`** branch.
2. Add the repository root in Unity Hub.
3. Use **Unity 2022.3.55f1**, as recorded in [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
4. Allow Unity to import the assets and resolve the checked-in packages.
5. Open **`Assets/Scenes/GameLoop.unity`**, the enabled scene in [EditorBuildSettings.asset](ProjectSettings/EditorBuildSettings.asset).
6. Enter Play Mode and use the on-screen deployment and ability controls.

The repository also contains prototype branches whose behavior and input may differ.

## Related work

- [My current game-development portfolio](https://drive.google.com/file/d/18c2vwif-Fdyiiagyl8mQI3w3LAcYNHuj/view)
- [GitHub profile](https://github.com/alertpotato)

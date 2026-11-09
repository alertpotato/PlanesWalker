# PlanesWalker

PlanesWalker is a Unity game prototype about travelling between dimensions and gathering an army from magical and non-magical worlds. The idea grew from the world-hopping fantasy of *The Chronicles of Amber* and *Magic: The Gathering*.

I imagined a roguelike journey built around choosing the next encounter, recruiting squads and improving the army between battles. The prototype includes an encounter-selection loop with generated enemies, rewards and increasing difficulty. The broader idea was to organise encounters into acts, each ending in a boss fight before moving on to the next.

Combat is a mix of a card game and an autobattler, where cards represent squads placed on a tactical battlefield. Positioning affects their available targets and actions, while abilities let the player change how each squad behaves.

I took inspiration from the unit-stack system in Heroes of Might and Magic, where a single battlefield unit represents a group of creatures. I wanted to explore different rules for those groups: simultaneous damage instead of attacks followed by retaliation, engagement limits based on squad size, and morale affecting damage distribution and casualties.

## Implemented systems

- **Grid-based tactical combat:** squad deployment, positioning and targeting rules.
- **Squad behaviour and abilities:** selectable actions and reusable ability and effect logic.
- **Turn resolution:** action ordering and simultaneous damage application.
- **Squad-size calculations:** engagement limits that affect how much of a squad can contribute damage.
- **Morale and casualties:** morale-dependent damage distribution and squad losses.
- **Stats and progression:** unit attributes and weighted stat-upgrade rules.
- **Encounter-based progression:** selection between generated encounters, enemy compositions and rewards, with increasing difficulty across successive battles.
- **Between-battle progression:** recruitment, unit upgrades and resource rewards carried between encounters.
- **Gameplay architecture:** separation of combat rules from the interaction and presentation layer.

## Running the game

### Open in Unity

1. Clone or download the repository's default **`Master`** branch.
2. Add the repository root in Unity Hub.
3. Use **Unity 2022.3.55f1**, as recorded in [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
4. Allow Unity to import the assets and resolve the checked-in packages.
5. Open **`Assets/Scenes/GameLoop.unity`**, the enabled scene in [EditorBuildSettings.asset](ProjectSettings/EditorBuildSettings.asset).
6. Enter Play Mode and use the on-screen deployment and ability controls.

The repository also contains prototype branches whose behavior and input may differ.

### Download a build

- [Prototype V1](https://drive.google.com/file/d/1Qh-TkL4kCk4BqsMDOLCXyL1BtkgUn9Ci/view?usp=sharing)
- [Prototype V2](https://drive.google.com/file/d/11L_-2gjfT28UW9zROj5E1n2aUGXrig0R/view?usp=sharing)

Download and extract a build to try it without Unity Editor. These builds are earlier iterations and may differ from the source branches.

## Design notes

[Original design notes](https://github.com/alertpotato/PlanesWalker/blob/4760c7b/README.md)

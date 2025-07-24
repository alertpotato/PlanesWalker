# Working title "Planes walker"
This is a prototype for a card-based game. It will include deckbuilding mechanics, turn-based combat and some auto-battler features.
At this moment i'm iterating through combat mechanics.

Built on Unity 2022.
# Premise
Game will be really light on story elements. You are an unnamed wizard in medieval fantasy environment who is stalking another wizard. Your magic power gives you an ability to craft reality around yourself laying a path towards more chaotic worlds. It is inspired from "The Chronicles of Amber" book series.
# Game structure
Game has one repeatable loop, where you need to go through certain amount of encounters(turns,days,timeframe not decided) ending with final battle.
"Encounter" is an enemy army with possible rewards and world modifiers.
Gameplay loop is:
- Choose one encounter out of a set
- Place your army on the battlefield
- Fight
- Choose rewards, including new units, upgrades to existing units, resources, hero upgrades
- Repeat
  
While doing all of that there are 3 types of progression to follow: meta progression, world progression and player progression.
- meta progression - something that is exists outside of game and gives player more motivation to start over.
- world progression - modifiers that player gets inside of particular run. They are changing rewards and encounters player gets, giving more replayability.
- hero progression - hero characteristics, resources and units.
  
### Meta progression
Not yet defined.
One of the ideas was to give players different starting heroes, and so they would be locked behind certain achievements etc.
Possible to make different starting world conditions, about what it is later.

### World progression
This is one of the core features of the game. "The world" meant to be super unstable and random, drastically changing each play session.

Idea is one game you pathed to a kingdom of wizards and employ magical creatures into your army, fighting against hordes of undead. Other game you building highly technical army, progressing towards early guns, fighting against steampunk inspired enemies. All of that with different races, all the time mixing technology and magic.

World characteristics that are changing:
- available races
- individual race power
- race society level(is it a goblin village or is a kingdom and they can have goblin knights?)
- world resources and supplies(tied to enemy units power)
- technological level
- magical level

### Hero progression
- Hero abilities
- Hero characteristics, that are boosting units in his army(Heroes od Might and Magic style)
- Units, that can be traded and upgraded
- Resources that directly affect the strength of units(about units later)

## What is Hero
Your army as well as any army you are going against is lead by "Hero".

Heroes gives units boosts to characteristics and uses abilities in battle.
Hero abilities will affect units positioning, units cohesion and give units unique abilities.

Right now heroes are only exist outside of battlefield, but it is possible they will be a central unit on the battlefield.

## Units
Units are second core feature of my game. It will have very robast and flexible unit generator, so they can have multiple dimensions of variability.

Unit structure:
- Race
- Unit Attributes
  - Health - hit points amount of individual unit.
  - Combat Proficiency - CP, difference in CP between two engaged units influences in damage outcome.
  - Cohesion - explained later.
  - Squad Size - Number of units in squad when deployed on battlefield.
  - Cost Of Deployment - Resources cost to deploy squad on battlefield. Explained later.
  - Abilities - Squad abilities. Explained later.
- Armour Set
- Weapons

There are certain templates like "Archers", "Hedge knights", "Hobelars", etc. that have certain starting parameters, that can be changed further depending on race, world and other modifiers.

Armours and weapons are not set in stone yet. Right now there are 3 types of damage: pierce, slash and blunt. Armours have resists against them, weapons have damage values and penetration for each.

Unit abilities are "Arrow volley" for units with ranged weapons, "Charge" for mounted units etc. Each ability can change how unit deals and takes damage, also determines behavior on the battlefield.
Abilities is not some special things for units to do from time to time, its only way they are doing staff. Abilities also used to retaliate in combat, so some archers will have shooting abilities that allows to shoot back, some units on the contrary will have good "attacking" and bad "defending" abilities(or none whatsoever). 

Each unit template have weight tables for each parameter, so it can be generated with different quality levels. One of the aspirations of the game would be hunt for "perfect" unit. For example Knights have lowest squad size, but greater health, cohesion and better equipment sets. it is rare but extremely powerful when they get bonuses to squad size, greatly multiplying squad power.

## Resource system
Right now there are 4 resources:
- Supply
- Weapons
- Money
- Wisdom

Resources multiply power of squads and certain abilities. You get them when as rewards and trade opportunities in certain encounters.

How it works. For example, a unit requires 1 supply and 2 weapons. If the player has this amount, he can call the unit to the battlefield. If the player has 2 supplies and 4 weapons, the unit's strength is "doubled". Most often, this will only be a proportional increase to the number of Squad Size. Some special units always consisting of 1 creature (powerful mages, shamans, ogres, dragons) will receive increases in health points and/or abilities power.

Game structured so you will work mostly with supplies and weapons at the start of the game, then you will encounter merceneries and more "noble" units, that will demand money, and later you will need wisdom for more magical or tech units.

## Cohesion

Third core feature of the game. Cohesion determines how damage is distributed between units in a squad. Goes from -10 to +10.

How it works. Lets say we have squad with 10 units each of them having 10 HP.
- Cohesion=0 is classic HoMM style damage distribution. 50 damage taken means 5 units dead.
- Cohesion>=0 means damage distributed between units, so less of them die. With 10 cohesion first unit will die only on recieving 91 damage. With +1 cohesion taking 50 damage our squad will only lose 4 units instead of 5.
  >NumberOfUnitsLost = ( UnitHP*(1-COH/10) * NumberOfUnits - (MaxPossibleSquadHP - IncomingDamage) ) / UnitHP*(1-COH/10)
- Cohesion<0 means squad taking additional damage due to battle chaos and deserters. On reaching -10 cohesion smallest amount of damage will result in whole sqaud lost. Reaching -3 cohesion upon taking 50 damage your squad will lose 6 units instead of 5(with -1 you will still lose 5 units but will take 54 damage instead of 50).
  >AdditionalCohesionDamage = NumberOfUnitsLost * UnitHP - IncomingDamage
  
As you can see difference between being in positive cohesion compered to negative is quite noticeble. Managing your troops cohesion in combat meant to be main player task.

### Cohesion economy
Upon every battle start your units cohesion is always equals to whatever that unit has + your hero bonuses. Then you lose 1 cohesion every round your unit was engadeg in combat. This mechanic meant to be ever growing danger, 0-sum battle-ending mechanism.

Where is a lot of other ways to gain or lose cohesion, related to hero and unit abilities.

### What problem i tried to solve with such a system
1. I do not like how linear damage is in many games, it feels unfair how easely you can lose units in games like HoMM. Cohesion solves that aspect and also introduces deep mechanic to play with.
2. Any battle is chaos and endurance trial, no battle can be dragged forever. This makes sure you cannot cheese in some cituations by also adding this growing danger feeling with each turn. 

## Battle mechanic
Turn-based, there is a square field, where you can deploy certain amount of troops you acquired. 

Deployment stage will most likely have some strategic mini-game, where you and your opponent place unit 1 by 1, untill all is deployed or field is full.

Each turn you first choose what abilities you want your troops to use, and then your and your enemy actions played in certain order for that round.

There is idea to make 5 different types of cells for troops to be in: Front, Flank(2), Support, Reserve. Depending on the type of cell there is will be different rules of engagement. Possible rules:
- Front attacks Front, if it is mounted units also attacks Flanks.
- Flanks attacks Flanks, if opposite flank empty can attack Front, if it is mounted units also can target Support.
- archers in Support can target Front or Flanks, if it is mounted support can target Flank or Front.
- Reserve units can rest there replanishing there Cohesion, and switch with your units in other places between turns.
  
Nothing is set in stone about it, will be described better later.

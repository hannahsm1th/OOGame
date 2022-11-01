# Game
 A simple game developed for object oriented development

[Demonstration video](https://youtu.be/apd_sgU-gng)

This program is a rogue-like dungeon crawler created using C# and the Unity Engine. It is a 2D game based on the tutorial available from the Unity Asset Store. However, additional features were added.

* Enemies have variable attack and defence

* The player can attack enemies

* The player can collect equipment which improve their attack and defence capabilities, or health

* The player can collect bonus lives

* A heads up display (HUD) communicating information about the player's stats and equipment

* The dungeon now has a boss level

## USE CASES

### Moving the player

Playing the game: Movement (Primary)

1.	User operates the arrow keys

2.	Player moves in one of the four directions

### Using weapons to attack

Playing the game: Use weapon (Primary)

1.	User moves player adjacent to the enemy

2.	User presses the arrow in the direction of the enemy

3.	Enemy loses health relative to their defence and the player’s equipped weapon

4.	If the enemy’s health is reduced to 0, it dies and is removed from the game

3a. Enemy attacks the player back
3b. Player loses health relative to their defence and the enemy’s attack
3c. If the player is reduced to 0 health they can use one life potion and regain all their hearts or the game will end if they have no extra lives

### Picking up equipment

Playing the game: Equipping Items (Primary)

1.	User moves onto a tile with an item

2.	Based on the equipment item’s type, the player’s inventory will update. If there is already another item of the same type it will be replaced

3.	The HUD updates to reflect the change to inventory, or the player’s stats

### Picking up items

Playing the game: Using items (Primary)

1.	User moves onto a tile with a health item

2.	The player’s health will restore if they have empty hearts

3.	The HUD updates to reflect this increase in health

2a. If the item is a life potion, an extra life is added

### Attacking walls

Playing the game: Walls (Primary)

1.	The user moves the player adjacent to a wall

2.	The user presses the arrow in the direction of the wall

3.	The wall loses hit points and changes to a damaged sprite

4.	If the hit points are reduced to 0, the wall disappears and the tile is free to move onto

## CLASS DIAGRAM AND JUSTIFICATION
Please see the figure below for the conceptual diagram and [here](./UMLspecifications.pdf) fr the specification-level class diagram. Details on each method can be found in the comments from the C# scripts.

<img src=".uml.png">

* Game: This manages the entire game system, deploying the other classes in order to create the game play experience.

* Level: This builds the levels, changing the conditions as the player progresses through the game

* Count: This class is encapsulated inside Level and is used to generate a random number of walls, enemies and items

* SoundManager: This class is responsible for deploying the audio feedback the player receives about their actions (using health items, hitting walls, walking) and enemy attacks

* Loader: This class is only responsible for instantiating the game in a singleton pattern

* HUD: This class is responsible for the displaying of the heads up system. It receives input from the HUDSystem and tells the UnityEngine what to display

* HeartImage and EquipmentImage are encapsulated inside and used to display the correct picture to reflect the player’s equipment or health

* The HUDSystem receives input from the Equipment class and Player about what changes are being made to the player and delivers that to the HUD for display via a publisher-subscriber system.

* The Heart class is needed in order to have the “half” heart capability. It is responsible for determining changes in health are reflected in a half heart system and is encapsulated in the HUDSystem

* Equipment is an abstract class that handles common behaviours from the types of equipment. The equipment cannot be encapsulated since it needs to interact with the Game based on what type of item it is.

* Each Equipment child class is responsible for checking if there is a similar type of item equipped

* The Entity class is abstract and handles the interaction between the Entity and environment, as well as movement

* The Enemy class handles interaction from the Enemy when they attack a player

* The Player class is needed to determine what happens to the Player when they interact with their environment by attacking a wall or enemy, or picking up equipment or items. The Player is regenerated every level with their stats and equipment being kept in the Game singleton to ensure consistency between levels

## PROJECT DESIGN

The following principles of coding design were addressed:

### Single Responsibility Principle

This principle was created by Robert Martin, who on his blog described this as “Gather together the things that change for the same reasons. Separate those things that change for different reasons” [1]. In this program, all but the Game class operate on this principal. For example, the HUDSystem, although it interacts with both the Equipment and Player classes, only changes in response to the single reason of a player picking up an item and changing their stats. Although within the Player and Equipment classes there can be different ways for the stats to change the HUDSystem only needs to know that there was a change, not the reason for the change. The methods in the HUDSystem reflect the different ways the player’s stats can change. Similarly, the Player only changes for the reason that its boxCollider encountered another boxCollider. It then determines the appropriate behaviour based on what it encountered. This is made possible by storing most of the player’s persistent information in the Game class.

### Singleton Design Pattern

In order to ensure the Player’s data remained the same and that the Level class could progressively generate harder levels, the Game class was made a singleton. This was done both in the class itself and using the Loader class which check for a Game instance running before the instantiate the Game class.

### Observer Pattern

An observer pattern was used to update the information processed by the HUDSystem to the HUD class which actually displayed the icons. This was done by storing a static HUDSystem in the Game class. The HUD class subscribes to this HUDSystem at the beginning of the game when all the Awake() functions are called. The HUD class then receives notifications from the HUDSystem whenever the Player stats are changed. This is preferable to function calls as it means that the HUDSystem does not need to store or know any information about the HUD, but the HUD does need to know about the HUDSystem. This creates an appropriate one-way flow of information. 
This pattern was not used to communicate between the HUDSystem and other Player and Equipment classes, since the HUDSystem needs to receive specific information about the changes to the Player stats. This would have necessitated the creation of specific Event classes in the Player and Equipment classes that could handle events giving notifications with extra parameters. However, the HUDSystem was already accessible by its attachment to the Game class so using function calls didn’t create any extra accessibility that wasn’t already there.

### Generics

The Entity class has two inheritable methods using abstracts. The AttemptMove method and OnBlocked method are both Generic. In the derived classes, the AttemptMove method is called on whatever possible types of object can be expected. The Enemy class only interacts with the Player so only AttemptMove<Player> is called. In the Player class, the method is called three times. The OnBlocked method then checks by attempting to cast as the various method types and then calls whatever method is appropriate. So if the Player encounters a wall, the DamageWall() method is called. These casts are type safe as AttemptMove will only accept the three possible classes in the casts for the Player interactions. This also allows the entire functionality of Player interactions to be contained within a single OnBlocked method, rather than needed a method for every possible type of interaction.

### Anonymous functions & lambda expressions

An delegate HUDUpdate() is used in the Player class to display information relating to the player stats as a string. Since the string contents change based on what is happening in the game, using lambda expression allows each version of the update being contained in other methods to be local and reflect the particular situation. This allows the HUD text to update the player about how many hearts the have lost or gained.
A lambda expression is used to remove null weapon slots from the equipment list before it is displayed to the HUD.

## REFERENCES

[1] 	R. C. Martin, “The Single Responsibility Principle,” 08 05 2014. [Online]. Available: https://blog.cleancoder.com/uncle-bob/2014/05/08/SingleReponsibilityPrinciple.html. [Accessed 29 09 2020].
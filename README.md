# DotnetOsrsApiWrapper
A .NET Wrapper for the OSRS highscores API. This is a fork of Assasindie's repository, intended to refactor some aspects of the original code such as:
- Moving parser code to a Service class, allowing PlayerInfo to act more like a Domain Object
- Injecting HttpClient into the Service, for improved testability
- Use of async for non-blocking code

I intend to use these changes for another project I have been working on, so am planning to keep it up-to-date when new Activities and/or Skills are added to the game.

# Installation
Coming soon...

# Updates
Updating the package is rather simple, you just need to add the new skill (if one ever comes out) and/or Activity (boss/minigame) in the correct order it appears from the API. The order can be found here https://runescape.wiki/w/Application_programming_interface#Hiscores_Lite_2 .
You just need to add the new field to the PlayerInfo class in the appropiate spot and the package will take care of the rest.

Eg adding a new bosses called Zulrah2 and Zul that appears after and before zulrah respectively in the API list is as simple as editing PlayerInfo to be 
```C#
public Activity Zul { get; set; } = InitialActivityState;
public Activity Zulrah { get; set; } = InitialActivityState;
public Activity Zulrah2 { get; set; } = InitialActivityState;
```

# Usage
Initialize an instance of the class with the OSRS username.
```C#
PlayerInfo Player = new PlayerInfo("Assasindie");
```
Retrieve info about Player's Skills.
```C#
// Retrieve a specific Skill's Level.
public int SlayerLevel = Player.Slayer.Level;
// Retrieve a specific Skill's Rank
public int FletchingRank = Player.Fletching.Rank;
// Retrieve a specific Skill's Experience
public int Hitpoints = Player.Hitpoints.Experience;
```
Retrieve info about Player's Minigame stats.
```C#
// Retrieve a specific Minigames Rank
public int LMSRank = Player.LastManStanding.Rank;
// Retrieve a specific Minigames Score
public int MasterClueCompletions = Player.MasterClueScrolls.Score;
```
Retrieve all values of a Player in string format.
```C#
Console.WriteLine(Player.GetAllValuesToString());
/* outputs : 
Player Name : Assasindie
Overall : Rank : 64962, Level : 2010, Experience : 170238009,
Attack : Rank : 75590, Level : 99, Experience : 14061346,
Defence : Rank : 38037, Level : 99, Experience : 15706736,
Strength : Rank : 59169, Level : 99, Experience : 16523738,
Hitpoints : Rank : 36005, Level : 99, Experience : 28327649,
Ranged : Rank : 46751, Level : 99, Experience : 21773617,
Prayer : Rank : 89261, Level : 84, Experience : 2989692,
Magic : Rank : 49841, Level : 99, Experience : 14699752,
Cooking : Rank : 202197, Level : 89, Experience : 4847633,
Woodcutting : Rank : 97462, Level : 88, Experience : 4525861,
Fletching : Rank : 304461, Level : 80, Experience : 1987306,
Fishing : Rank : 132872, Level : 85, Experience : 3484752,
Firemaking : Rank : 168640, Level : 86, Experience : 3678548,
Crafting : Rank : 109435, Level : 82, Experience : 2564159,
Smithing : Rank : 74662, Level : 85, Experience : 3265164,
Mining : Rank : 126816, Level : 80, Experience : 2005829,
Herblore : Rank : 85437, Level : 85, Experience : 3268295,
Agility : Rank : 95737, Level : 80, Experience : 2001215,
Thieving : Rank : 98240, Level : 81, Experience : 2240683,
Slayer : Rank : 19801, Level : 99, Experience : 14833524,
Farming : Rank : 158825, Level : 82, Experience : 2437750,
Runecrafting : Rank : 71460, Level : 77, Experience : 1558992,
Hunter : Rank : 217113, Level : 71, Experience : 846896,
Construction : Rank : 129107, Level : 82, Experience : 2608872,
BountyHunterRogues : Rank : -1, Score : -1,
BountyHunter : Rank : -1, Score : -1,
LastManStanding : Rank : 14819, Score : 503,
TotalCluesScrolls : Rank : 11530, Score : 756,
BeginnerClueScrolls : Rank : -1, Score : -1,
EasyClueScrolls : Rank : 288644, Score : 7,
MediumClueScrolls : Rank : 7993, Score : 420,
HardClueScrolls : Rank : 22995, Score : 248,
EliteClueScrolls : Rank : 37188, Score : 31,
MasterClueScrolls : Rank : 7456, Score : 50,
*/
```
IEnumerable for Minigames
```C#
foreach(Activity activity in Player.Minigames())
{
     Console.WriteLine(activity.Rank);
     Console.WriteLine(activity.Name);

}
```
IEnumerable for Skills
```C#
foreach(Skill skill in Player.Skills())
{
     Console.WriteLine(skill.Level);
     Console.WriteLine(skill.Name);
}
```


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace DotnetOsrsApiWrapper
{
    public class PlayerInfo
    {
        //initial states for the Skills/Minigames
        private static readonly Skill InitialSkillState = new Skill { Rank = -1, Level = 1, Experience = 1 };
        private static readonly Activity InitialActivityState = new Activity { Rank = -1, Score = -1 };

        //do not re-order this or it will break it all
        public string Name { get; set; } = "";
        public PlayerInfoStatus Status { get; internal set; } = PlayerInfoStatus.Unknown;
        public Skill Overall { get; set; } = InitialSkillState;
        public Skill Attack { get; set; } = InitialSkillState;
        public Skill Defence { get; set; } = InitialSkillState;
        public Skill Strength { get; set; } = InitialSkillState;
        public Skill Hitpoints { get; set; } = InitialSkillState;
        public Skill Ranged { get; set; } = InitialSkillState;
        public Skill Prayer { get; set; } = InitialSkillState;
        public Skill Magic { get; set; } = InitialSkillState;
        public Skill Cooking { get; set; } = InitialSkillState;
        public Skill Woodcutting { get; set; } = InitialSkillState;
        public Skill Fletching { get; set; } = InitialSkillState;
        public Skill Fishing { get; set; } = InitialSkillState;
        public Skill Firemaking { get; set; } = InitialSkillState;
        public Skill Crafting { get; set; } = InitialSkillState;
        public Skill Smithing { get; set; } = InitialSkillState;
        public Skill Mining { get; set; } = InitialSkillState;
        public Skill Herblore { get; set; } = InitialSkillState;
        public Skill Agility { get; set; } = InitialSkillState;
        public Skill Thieving { get; set; } = InitialSkillState;
        public Skill Slayer { get; set; } = InitialSkillState;
        public Skill Farming { get; set; } = InitialSkillState;
        public Skill Runecrafting { get; set; } = InitialSkillState;
        public Skill Hunter { get; set; } = InitialSkillState;
        public Skill Construction { get; set; } = InitialSkillState;

        //minigames
        public Activity LeaguePoints { get; set; } = InitialActivityState;
        public Activity BountyHunterRogues { get; set; } = InitialActivityState;
        public Activity BountyHunter { get; set; } = InitialActivityState;
        public Activity TotalCluesScrolls { get; set; } = InitialActivityState;
        public Activity BeginnerClueScrolls { get; set; } = InitialActivityState;
        public Activity EasyClueScrolls { get; set; } = InitialActivityState;
        public Activity MediumClueScrolls { get; set; } = InitialActivityState;
        public Activity HardClueScrolls { get; set; } = InitialActivityState;
        public Activity EliteClueScrolls { get; set; } = InitialActivityState;
        public Activity MasterClueScrolls { get; set; } = InitialActivityState;
        public Activity LastManStanding { get; set; } = InitialActivityState;
        public Activity PvPArena { get; set; } = InitialActivityState;
        public Activity SoulWarsZeal { get; set; } = InitialActivityState;
        public Activity RiftsClosed { get; set; } = InitialActivityState;

        //bosses
        public Activity AbyssalSire { get; set; } = InitialActivityState;
        public Activity AlchemicalHydra { get; set; } = InitialActivityState;
        public Activity BarrowsChests { get; set; } = InitialActivityState;
        public Activity Bryophyta { get; set; } = InitialActivityState;
        public Activity Callisto { get; set; } = InitialActivityState;
        public Activity Cerberus { get; set; } = InitialActivityState;
        public Activity ChambersofXeric { get; set; } = InitialActivityState;
        public Activity ChambersofXericChallengeMode { get; set; } = InitialActivityState;
        public Activity ChaosElemental { get; set; } = InitialActivityState;
        public Activity ChaosFanatic { get; set; } = InitialActivityState;
        public Activity CommanderZilyana { get; set; } = InitialActivityState;
        public Activity CorporealBeast { get; set; } = InitialActivityState;
        public Activity CrazyArchaeologist { get; set; } = InitialActivityState;
        public Activity DagannothPrime { get; set; } = InitialActivityState;
        public Activity DagannothRex { get; set; } = InitialActivityState;
        public Activity DagannothSupreme { get; set; } = InitialActivityState;
        public Activity DerangedArchaeologist { get; set; } = InitialActivityState;
        public Activity GeneralGraardor { get; set; } = InitialActivityState;
        public Activity GiantMole { get; set; } = InitialActivityState;
        public Activity GrotesqueGuardians { get; set; } = InitialActivityState;
        public Activity Hespori { get; set; } = InitialActivityState;
        public Activity KalphiteQueen { get; set; } = InitialActivityState;
        public Activity KingBlackDragon { get; set; } = InitialActivityState;
        public Activity Kraken { get; set; } = InitialActivityState;
        public Activity Kree { get; set; } = InitialActivityState;
        public Activity Kril { get; set; } = InitialActivityState;
        public Activity Mimic { get; set; } = InitialActivityState;
        public Activity Nex { get; set; } = InitialActivityState;
        public Activity Nightmare { get; set; } = InitialActivityState;
        public Activity PhosanisNightmare { get; set; } = InitialActivityState;
        public Activity Obor { get; set; } = InitialActivityState;
        public Activity PhantomMuspah { get; set; } = InitialActivityState;
        public Activity Sarachnis { get; set; } = InitialActivityState;
        public Activity Scorpia { get; set; } = InitialActivityState;
        public Activity Skotizo { get; set; } = InitialActivityState;
        public Activity Tempoross { get; set; } = InitialActivityState;
        public Activity Gauntlet { get; set; } = InitialActivityState;
        public Activity CorruptedGauntlet { get; set; } = InitialActivityState;
        public Activity TheatreofBlood { get; set; } = InitialActivityState;
        public Activity TheatreofBloodHardMode { get; set; } = InitialActivityState;
        public Activity ThermonuclearSmokeDevil { get; set; } = InitialActivityState;
        public Activity TombsOfAmascut { get; set; } = InitialActivityState;
        public Activity TombsOfAmascutExpertMode { get; set; } = InitialActivityState;
        public Activity Zuk { get; set; } = InitialActivityState;
        public Activity Jad { get; set; } = InitialActivityState;
        public Activity Venenatis { get; set; } = InitialActivityState;
        public Activity Vetion { get; set; } = InitialActivityState;
        public Activity Vorkath { get; set; } = InitialActivityState;
        public Activity Wintertodt { get; set; } = InitialActivityState;
        public Activity Zalcano { get; set; } = InitialActivityState;
        public Activity Zulrah { get; set; } = InitialActivityState;

        internal PlayerInfo() { }

        //returns all info in a big string.
        public string GetAllValuesToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Player Name : {Name} \n");
            // retrieve the property information of the Player Info Class
            PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();
            foreach (PropertyInfo info in properties)
            {
                if (info.PropertyType == typeof(Skill) || info.PropertyType == typeof(Activity))
                {
                    // gets the value of the PropertyInfo (Skill/Minigame) and returns its Properties
                    PropertyInfo[] SkillProperties = info.GetValue(this).GetType().GetProperties();
                    //Skill Name
                    sb.Append($"{info.Name} : ");
                    // Add the Minigame/Skill Properties into the sb
                    foreach (PropertyInfo SkillInfo in SkillProperties)
                    {
                        sb.Append($"{SkillInfo.Name} : {SkillInfo.GetValue(info.GetValue(this))}, ");
                    }
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        //IEnumerable for looping over all Skills
        public IEnumerable<Skill> Skills()
        {
            PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();

            foreach (PropertyInfo info in properties.Where(info => info.PropertyType == typeof(Skill)))
            {
                Skill skill = (Skill)info.GetValue(this, null);
                yield return skill;
            }
        }

        //IEnumberable for looping over all Minigames
        public IEnumerable<Activity> Minigames()
        {
            PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();

            foreach (PropertyInfo info in properties.Where(info => info.PropertyType == typeof(Activity)))
            {
                Activity skill = (Activity)info.GetValue(this, null);
                yield return skill;
            }
        }
    }
}


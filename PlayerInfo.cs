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
        //intial states for the Skills/Minigames
        private static readonly Skill InitialSkillState = new Skill { Rank = -1, Level = 1, Experience = 1 };
        private static readonly Minigame InitialMinigameState = new Minigame { Rank = -1, Score = -1 };

        //do not re-order this or it will break it all
        public string Name { get; set; } = "";
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
        public Minigame BountyHunterRogues { get; set; } = InitialMinigameState;
        public Minigame BountyHunter { get; set; } = InitialMinigameState;
        public Minigame LastManStanding { get; set; } = InitialMinigameState;
        public Minigame TotalCluesScrolls { get; set; } = InitialMinigameState;
        public Minigame BeginnerClueScrolls { get; set; } = InitialMinigameState;
        public Minigame EasyClueScrolls { get; set; } = InitialMinigameState;
        public Minigame MediumClueScrolls { get; set; } = InitialMinigameState;
        public Minigame HardClueScrolls { get; set; } = InitialMinigameState;
        public Minigame EliteClueScrolls { get; set; } = InitialMinigameState;
        public Minigame MasterClueScrolls { get; set; } = InitialMinigameState;

        public PlayerInfo(string UserName)
        {
            Name = UserName;

            HttpWebRequest req;
            //request player info from jagex api
            try
            {
                req = (HttpWebRequest)WebRequest.Create("https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + UserName);

                StreamReader Response = new StreamReader(req.GetResponse().GetResponseStream());
                //gets all the properties of the PlayerInfo class
                PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    //checks the PropertyType of the current Property and sets the value accordingly.
                    if (info.PropertyType == typeof(Skill))
                    {
                        string[] values = Response.ReadLine().Split(',');
                        info.SetValue(this, new Skill
                        {
                            Name = info.Name,
                            Rank = int.Parse(values[0]),
                            Level = int.Parse(values[1]),
                            Experience = int.Parse(values[2])
                        });
                    }

                    if (info.PropertyType == typeof(Minigame))
                    {
                        string[] values = Response.ReadLine().Split(',');
                        info.SetValue(this, new Minigame
                        {
                            Name = info.Name,
                            Rank = int.Parse(values[0]),
                            Score = int.Parse(values[1]),

                        });
                    }
                }
                Response.Dispose();
            }
            catch { }
        }

        //returns all info in a big string.
        public string GetAllValuesToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Player Name : {Name} \n");
            // retrieve the property information of the Player Info Class
            PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();
            foreach (PropertyInfo info in properties)
            {
                if (info.PropertyType == typeof(Skill) || info.PropertyType == typeof(Minigame))
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
        public IEnumerable<Minigame> Minigames()
        {
            PropertyInfo[] properties = typeof(PlayerInfo).GetProperties();

            foreach (PropertyInfo info in properties.Where(info => info.PropertyType == typeof(Minigame)))
            {
                Minigame skill = (Minigame)info.GetValue(this, null);
                yield return skill;
            }
        }
    }
}


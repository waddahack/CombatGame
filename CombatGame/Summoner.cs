using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    public class Summoner : Character
    {
        public static int nbrDefaultSummoner = 0;
        private List<Character> yokelList = new List<Character>();

        public List<Character> YokelList { get { return yokelList; } }

        public override string Name
        {
            get { return name; }
            set
            {
                name = value + " (Summoner)";
                nbrDefaultSummoner--;
            }
        }

        public Summoner()
            : base()
        {
            nbrDefaultSummoner++;
            name = "Chef Péquenaud (Summoner)" + (nbrDefaultSummoner > 1 ? $"{nbrDefaultSummoner}" : "") + " (Summoner)";
            skillDescription = "Fait apparaître un fidèle Péquenaud.";
            hp = 3;
            maxHp = hp;
            dmg = 1;
            maxSkillCooldown = 5;
        }

        public override void EndTurn()
        {
            if (specialSkillUsed)
            {
                Yokel newYokel = new Yokel();
                team.Add(newYokel);
                yokelList.Add(newYokel);
                Console.WriteLine($"{name} a invoqué un Péquenaud.");
            }
            
            base.EndTurn();
        }
    }
}

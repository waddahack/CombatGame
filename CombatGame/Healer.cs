using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Healer : Character
    {

        public static int nbrDefaultHealer = 0;

        private Character targetToHeal;

        public Character TargetToHeal { get { return targetToHeal; } set { targetToHeal = value; } }

        public override string Name
        {
            get { return name; }
            set
            {
                name = value + " (Healer)";
                nbrDefaultHealer--;
            }
        }

        public Healer()
            : base()
        {
            nbrDefaultHealer++;
            skillDescription = "Soigne 1 HP à un allié ou soi-même.";
            name = "Gandhi" + (nbrDefaultHealer > 1 ? $"{nbrDefaultHealer}" : "") + " (Healer)";
            hp = 4;
            maxHp = hp;
            dmg = 1;
            maxSkillCooldown = 3;
        }

        public void Heal()
        {
            if (targetToHeal != null)
                targetToHeal.Hp += 1;
            targetToHeal = null;
        }

        public override void EndTurn()
        {
            if (specialSkillUsed) {
                Console.WriteLine($"{name} à soigné {targetToHeal.Name} de 1 HP.");
                Heal();
            }
            base.EndTurn();
        }
    }
}

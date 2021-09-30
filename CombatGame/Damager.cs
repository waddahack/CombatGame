using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Damager : Character
    {
        public static int nbrDefaultDamager = 0;

        public override string Name
        {
            get { return name; }
            set
            {
                name = value + " (Damager)";
                nbrDefaultDamager--;
            }
        }

        public Damager()
            : base()
        {
            nbrDefaultDamager++;
            name = "Fredo" + (nbrDefaultDamager > 1 ? $"{nbrDefaultDamager}" : "") + " (Damager)";
            skillDescription = "Inflige en retour les dégâts qui lui sont infligés durant ce tour. Les dégâts sont quand même subis.";
            hp = 3;
            maxHp = hp;
            dmg = 2;
            maxSkillCooldown = 3;
        }
    }
}

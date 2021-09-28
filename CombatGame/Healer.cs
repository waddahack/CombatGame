using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Healer : Character
    {

        public override string Name { get { return name; } set { name = value + " (Healer)"; } }

        public Healer()
            : base()
        {
            skillDescription = "Récupère 1 HP.";
            name = "Gandhi (Healer)";
            hp = 4;
            dmg = 1;
            maxSkillCooldown = 3;
        }

        public override void SpecialSkill()
        {
            base.SpecialSkill();
            hp += 1;
        }
    }
}

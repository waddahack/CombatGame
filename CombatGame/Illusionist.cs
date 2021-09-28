using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Illusionist : Character
    {

        public override string Name { get { return name; } set { name = value + " (Illusionist)"; } }

        public Illusionist()
            : base()
        {
            name = "Jean-Magicien (Illusionist)";
            skillDescription = "Si la cible utilise sa COMPETENCE SPE durant ce tour, la cible se mange une longue patate et perd 3 HP. Sinon, l'illusionist trébuche tel un souillon et perd 2 HP.";
            hp = 3;
            dmg = 1;
            maxSkillCooldown = 2;
        }


        public override void EndTurn()
        {
            if (IsSpecialSkillUsed())
            {
                if (target.IsSpecialSkillUsed())
                {
                    target.Hp -= 3;
                    Console.WriteLine($"{target.Name} se mange un coup bas de la pars de l'illusionist et perd 3 points de vie");
                }
                else
                {
                    hp -= 2;
                    Console.WriteLine("L'illusionist a mal enticipé et tombe par terre, se fandant le visage en deux, il perd 2 points de vie");
                }
            }
            base.EndTurn();
        }
    }
}

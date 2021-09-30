using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Illusionist : Character
    {
        public static int nbrDefaultIllu = 0;

        public override string Name
        {
            get { return name; }
            set
            {
                name = value + " (Illusionist)";
                nbrDefaultIllu--;
            }
        }

        public Illusionist()
            : base()
        {
            nbrDefaultIllu++;
            name = "Jean-Magicien" + (nbrDefaultIllu > 1 ? $"{nbrDefaultIllu}" : "") + " (Illusionist)";
            skillDescription = "Si la cible utilise sa COMPETENCE SPE durant ce tour, la cible se mange une longue patate et perd 3 HP. Sinon, l'illusionist trébuche tel un souillon et perd 2 HP.";
            hp = 3;
            maxHp = hp;
            dmg = 1;
            maxSkillCooldown = 2;
        }


        public override void EndTurn()
        {
            if (SpecialSkillUsed)
            {
                if (target.SpecialSkillUsed)
                {
                    target.TakeDamage(3);
                    Console.WriteLine($"{target.Name} se mange un coup bas de la pars de {name} et perd 3 HP.");
                }
                else
                {
                    TakeDamage(2);
                    Console.WriteLine($"{name} a mal anticipé et tombe par terre, se fendant le visage en deux, il perd 2 HP.");
                }
            }
            base.EndTurn();
        }
    }
}

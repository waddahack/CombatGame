using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CombatGame
{
    public class Tank : Character
    {
        public static int nbDefaultTank = 0;
        public override string Name { get { return name; } 
            set { 
                name = value + " (Tank)";
                nbDefaultTank--;
            } }

        public Tank()
            : base()
        {
            nbDefaultTank++;
            name = "Wilfried" + (nbDefaultTank > 1 ? $"{nbDefaultTank}" : "") + " (Tank)";
            skillDescription =  "Sacrifie 1 HP durant ce tour pour gagner 1 ATT qu'il perd à la fin du tour. A la fin du prochain tour, il récupère l'HP perdu. Cette attaque, si défendu, enlève quand même 1 HP à l'adversaire.";
            hp = 5;
            maxHp = hp;
            dmg = 1;
            maxSkillCooldown = 3;
        }

        public override void SpecialSkill()
        {
            base.SpecialSkill();
            if (hp > 0)
            {
                TakeSelfDamage(1);
                dmg += 1;
            }
        }

        public override int AttackEnemy()
        {
            int damage = 0;

            if (target.State != States.Defending)
                damage = dmg;
            else if (specialSkillUsed)
                damage = 1;

            if (target is Damager && target.SpecialSkillUsed)
                TakeSelfDamage(damage);

            target.TakeDamage(damage);
            return damage;

        }

        public override void EndTurn()
        {
            int damageDealt = 0;
            if (state == States.Attacking)
                damageDealt = AttackEnemy();

            if (SpecialSkillUsed)
                dmg -= 1;

            if (skillCooldown == maxSkillCooldown - 2 && hp > 0)
            {
                Console.WriteLine($"{name} a récupéré 1 HP.");
                hp += 1;
            }

            if (skillCooldown > 0)
                skillCooldown -= 1;

            if (damageDealt > 0)
                Console.WriteLine($"{name} a infligé {damageDealt} HP à {target.Name}.");

            if(dmgTaken > 0)
                Console.WriteLine($"{name} s'est infligé {dmgTaken} HP.");
            dmgTaken = 0;

        }
    }
}

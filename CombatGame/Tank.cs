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

        public override string Name { get { return name; } set { name = value + " (Tank)"; } }

        public Tank()
            : base()
        {
            name = "Wilfried (Tank)";
            skillDescription =  "Sacrifie 1 HP durant ce tour pour gagner 1 ATT qu'il perd à la fin du tour. A la fin du prochain tour, il récupère l'HP perdu. Cette attaque, si défendu, enlève quand même 1 HP à l'adversaire.";
            hp = 5;
            dmg = 1;
            maxSkillCooldown = 4;
        }

        public override void SpecialSkill()
        {
            base.SpecialSkill();
            if (hp > 0)
            {
                hp -= 1;
                dmg += 1;
            }
        }

        public override int AttackEnemy()
        {
            int damage = 0;

            if (target.State != States.Defending)
                damage = dmg;
            else if (IsSpecialSkillUsed())
                damage = 1;

            if  (target is Damager && target.IsSpecialSkillUsed())
                hp -= damage;

            target.Hp -= damage;
            return damage;

        }

        public override void EndTurn()
        {
            if (state == States.Attacking)
                AttackEnemy();

            if (IsSpecialSkillUsed())
                dmg -= 1;

            if (skillCooldown == maxSkillCooldown - 2 && hp > 0)
                hp += 1;

            if (skillCooldown > 0)
                skillCooldown -= 1;
        }
    }
}

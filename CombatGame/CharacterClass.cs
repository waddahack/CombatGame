using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CombatGame
{
    // Class Character
    public abstract class Character
    {
        // Nom de la classe
        public enum States
        {
            Default,
            Attacking,    // Le joueur/ordi a choisit d'attaquer
            Defending,     // Le joueur/ordi a choisit de défendre
        }

        protected States state;

        protected Character target;

        protected List<Character> team;

        protected string name;
        // Point de vie du personnage
        protected int hp, maxHp;
        // Force d'attaque du personnage
        protected int dmg;
        // Description de la classe
        protected string skillDescription;

        protected int skillCooldown, maxSkillCooldown;

        protected bool specialSkillUsed;

        protected int dmgTaken;

        // Accesseurs
        public States State { get { return state; } set { state = value; } }
        public int Hp { get { return hp; } set { hp = value; } }
        public int MaxHp { get { return maxHp; } }
        public int Dmg { get { return dmg; } set { dmg = value; } }
        public string SkillDescription { get { return skillDescription; } }
        public int SkillCooldown { get { return skillCooldown; } }
        public virtual string Name { get { return name; } set { name = value; } }
        public int MaxSkillCooldown { get { return maxSkillCooldown; } }
        public Character Target { get { return target; } set { target = value; } }
        public List<Character> Team { get { return team; } set { team = value; } }
        public bool SpecialSkillUsed { get { return specialSkillUsed; } set { specialSkillUsed = value; } }
        // Constructeur
        public Character()
        {
            state = States.Default;
            skillCooldown = 0;
            specialSkillUsed = false;
            dmgTaken = 0;
        }

        public virtual int AttackEnemy()
        {
            int damage = 0;

            if (target.State != States.Defending)
                damage = dmg;
            if (target is Damager && target.specialSkillUsed)
                TakeSelfDamage(damage);

            target.TakeDamage(damage);
            return damage;
        }

        // Met le state correspondant
        public void SetAttackState()
        {
            state = States.Attacking;
        }

        public void SetDefendState()
        {
            state = States.Defending;
        }

        public bool IsDefended()
        {
            return state == States.Defending;
        }

        public void TakeDamage(int amount)
        {
            hp  -= amount;
        }

        public void TakeSelfDamage(int amount)
        {
            dmgTaken += amount;
            hp -= amount;
        }

        public virtual void SpecialSkill()
        {
            specialSkillUsed = true;
            if(skillCooldown == 0)
                skillCooldown = maxSkillCooldown;
        }

        // Fonction appelé à la fin de chaque tours
        public virtual void EndTurn()
        {
            int damageDealt = 0;
            if (state == States.Attacking)
                damageDealt = AttackEnemy();

            if (skillCooldown > 0)
                skillCooldown -= 1;

            if(damageDealt > 0)
                Console.WriteLine($"{name} a infligé {damageDealt} HP à {target.name}.");

            if (dmgTaken > 0)
                Console.WriteLine($"{name} s'est infligé {dmgTaken} HP.");
            dmgTaken = 0;

        }
    }
}

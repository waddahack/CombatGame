using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CombatGame
{
    // Class Character
    public abstract class Character : Selectionnable
    {
        // Nom de la classe
        public enum States
        {
            Default,
            Attacking,    // Le joueur/ordi à choisit d'attaquer
            Defending,     // Le joueur/ordi à choisit de défendre
        }

        protected States state;

        protected Character target;

        protected string name;
        // Point de vie du personnage
        protected int hp;
        // Force d'attaque du personnage
        protected int dmg;
        // Description de la classe
        protected string skillDescription;

        protected int skillCooldown, maxSkillCooldown;

        // Accesseurs
        public States State { get { return state; } set { state = value; } }
        public int Hp { get { return hp; } set { hp = value; } }
        public int Dmg { get { return dmg; } set { dmg = value; } }
        public string SkillDescription { get { return skillDescription; } }
        public int SkillCooldown { get { return skillCooldown; } }
        public virtual string Name { get { return name; } set { name = value; } }
        public int MaxSkillCooldown { get { return maxSkillCooldown; } }
        public Character Target { get { return target; } set { target = value; } }

        // Constructeur
        public Character()
        {
            state = States.Default;
            skillCooldown = 0;
        }

        public virtual int AttackEnemy()
        {

            int damage = 0;

            if (target.State != States.Defending)
                damage = dmg;
            if (target is Damager && target.IsSpecialSkillUsed())
                hp -= damage;

            target.Hp -= damage;
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


        public virtual void SpecialSkill()
        {
            if(skillCooldown == 0)
                skillCooldown = maxSkillCooldown;
        }

        // Verifie si le skill du personnage a été utilisé ce tour
        public virtual bool IsSpecialSkillUsed()
        {
            return (skillCooldown == maxSkillCooldown);
        }

        // Fonction appelé à la fin de chaque tours
        public virtual void EndTurn()
        {
            if (state == States.Attacking)
                AttackEnemy();

            if (skillCooldown > 0)
                skillCooldown -= 1;
        }
    }
}

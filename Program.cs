using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Program
    {
        // TEST GIT Thomas

        static void Main(string[] args)
        {
            // VARIABLES
            Character playerCharacter;
            Character AICharacter;


            // CHOIX DE PERSO


            // BOUCLE JEU (fin quand un des deux perso n'a plus d'hp)


                // CHOIX JOUEUR


                // CHOIX AI


                // CALCUL DES CHOIX



                // VERIFICATION DE FIN DE PARTIE


            // MESSAGE DE FIN DE PARTIE
        }



        // Class Character
        public abstract class Character
        {
            // Nom de la classe
            public enum States
            {
                Default,
                Attacking,    // Le joueur/ordi à choisit d'attaquer
                Defending,     // Le joueur/ordi à choisit de défendre
            }

            protected States state;

            // Point de vie du personnage
            protected int hp;
            // Force d'attaque du personnage
            protected int dmg;

            protected int skillCooldown, maxSkillCooldown;


            // Accesseurs
            public States State { get { return state; } set { state = value; } }
            public int Hp { get { return hp; } set { hp = value; } }
            public int Dmg { get { return dmg; } set { dmg = value; } }
            public int SkillCooldown { get { return skillCooldown; } }
            public int MaxSkillCooldown { get { return maxSkillCooldown; } }


            // Constructeur
            public Character(int hp, int dmg, int maxSC)
            {
                this.hp = hp;
                this.dmg = dmg;
                state = States.Default;
                skillCooldown = 0;
                maxSkillCooldown = 3;
            }

            public virtual void AttackEnemy(Character target)
            {
                if (target.State != States.Defending)
                    target.Hp -= dmg;
                state = States.Attacking;
            }

            public void SetAttackState()
            {
                state = States.Attacking;
            }

            public void SetDefendState()
            {
                state = States.Defending;
            }

            public virtual void SpecialSkill() {
                skillCooldown = maxSkillCooldown;
            }

            public virtual bool IsSpecialSkillUsed()
            {
                return skillCooldown == maxSkillCooldown;
            }

            public virtual void EndTurn()
            {
                if (skillCooldown > 0) 
                    skillCooldown -= 1;
            }
        }

        public class Tank : Character
        {
            public Tank(int hp, int dmg, int maxSC) : base(hp, dmg, maxSC) { }

            public override void SpecialSkill()
            {
                base.SpecialSkill();
                if (hp > 0)
                {
                    hp -= 1;
                    dmg += 1;
                }
            }

            public override void AttackEnemy(Character target)
            {
                base.AttackEnemy(target);
                if (target.State == States.Defending && IsSpecialSkillUsed())
                    target.Hp -= 1;
            }

            public override void EndTurn()
            {
                base.EndTurn();
                if (IsSpecialSkillUsed())
                    dmg -= 1;
                if (skillCooldown == maxSkillCooldown - 1 && hp > 0)
                    hp += 1;
            }
        }
    }
}


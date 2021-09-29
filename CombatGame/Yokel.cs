using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatGame
{
    class Yokel : Character
    {
        public static int nbYokel = 0;
        public Yokel() : base()
        {
            nbYokel++;
            name = $"Péquenaud{nbYokel}";
            hp = 2;
            maxHp = hp;
            dmg = 1;
            maxSkillCooldown = 5;
        }
    }
}

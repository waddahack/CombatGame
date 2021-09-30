using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace CombatGame
{
    class Program
    {

        static int GAMEHEIGHT, LINESIZE = 60;
        static Random RANDOM = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            // Enregistre les personnages de chaque équipes
            List<Character> team1 = new List<Character>();
            List<Character> team2 = new List<Character>();
            List<String> replayChoices = new List<string>();
            replayChoices.Add("| Rejouer |");
            replayChoices.Add("| Quitter |");

            int replay = 1; // 0 = rejouer, 1 = quitter
            do
            {
                // Contre IA ?
                bool versusAI = ChoosePlayMode();
                Character choice;

                // Equipe 1 choisit ses perso
                Console.Clear();
                do
                {
                    choice = PlayerCharacterChoice(team1, 1);
                    if (choice is Character)
                        team1.Add(choice);
                } while (choice != null || team1.Count < 1);

                // L'IA choisit sont équipe
                if (versusAI)
                    for (int i = 0; i < team1.Count; i++)
                    {
                        int aiCharacterChoice = RANDOM.Next(1, 6); //CHOICE OF AI
                        team2.Add(CharacterChoice(aiCharacterChoice, team2));
                    }
                // Equipe 2 choisit ses perso
                else
                {
                    do
                    {
                        choice = PlayerCharacterChoice(team2, 2);
                        if (choice is Character)
                            team2.Add(choice);
                    } while (choice != null || team2.Count < 1);
                }

                GAMEHEIGHT = 6 + Math.Max(team1.Count, team2.Count) * 2;
                // BOUCLE JEU
                bool endGame = false;
                while (!endGame)
                {
                    // DISPLAY GAME
                    DisplayGame(team1, team2);

                    // PLAYER CHOICE
                    PlayerActionChoice(team1, team2);
                    if (versusAI)
                        AiActionChoice(team2, team1);
                    else
                        PlayerActionChoice(team2, team1, true);

                    // WHAT HAPPENED
                    Console.WriteLine("------ COMBAT LOG ------\n");
                    // CALCUL DES CHOIX ET FIN DE TOUR
                    Console.WriteLine("-- Équipe 1 --\n");
                    for (int i = 0; i < team1.Count; i++)
                        team1[i].EndTurn();
                    Console.WriteLine("\n-- Équipe 2 --\n");
                    for (int i = 0; i < team2.Count; i++)
                        team2[i].EndTurn();

                    // CHECK ENDGAME et retire les persos mort des équipes
                    endGame = EndGameCheck(team1, team2);

                    for (int i = 0; i < team2.Count; i++)
                        team2[i].Reset();

                    Console.Write("\nEntrée pour continuer...");
                    Console.ReadLine();
                }
                DisplayGame(team1, team2);
                if(team1.Count > team2.Count)
                {
                    Console.WriteLine("\nÉquipe 1 a gagné !\n");
                    EndAnimation();
                } 
                else if (team1.Count < team2.Count)
                {
                    Console.WriteLine("\nÉquipe 2 a gagné !\n");
                    EndAnimation();
                }
                else
                    Console.WriteLine("\nÉgalité.\n");
                replay = ArrowChoice(replayChoices, GAMEHEIGHT + 3);
                Console.Clear();
            } while (replay == 0);
        }

        static bool ChoosePlayMode()
        {
            Console.WriteLine("Comment voulez-vous jouer ?\n");
            List<String> choices = new List<String>();
            choices.Add("Joueur VS Ordinateur");
            choices.Add("Joueur VS Joueur");
            int answer = ArrowChoice(choices, 2);
            if (answer == 0) return true;
            else return false;
            
        }

        static void DisplayGame(List<Character> team1, List<Character> team2, int equipe = 1)
        {
            string dmgSymbol = " ATT", CDSymbol = " CD";
            Console.Clear();
            if(!(team2 is null))
                Console.WriteLine(String.Concat(Enumerable.Repeat("*", Console.WindowWidth)));
            Console.WriteLine("Équipe " + equipe + " :" + String.Concat(Enumerable.Repeat(" ", LINESIZE - 10)) + (team2 is null ? "" : "Équipe 2 :") + "\n");
            Character perso1, perso2;
            string perso1Info = "", perso2Info = "";
            for (int i = 0; i<team1.Count; i++)
            {
                perso1 = team1[i];
                perso1Info = $"{ perso1.Name} : {CharacterHearts(perso1)} {perso1.Dmg}{dmgSymbol} {perso1.SkillCooldown}{CDSymbol}";
                if (perso1.IsDefended())
                    perso1Info += " {Vulnérable}";
                perso2Info = "";
                if (!(team2 is null) && i < team2.Count)
                {
                    perso2 = team2[i];
                    perso2Info = $"{perso2.Name} : {CharacterHearts(perso2)} {perso2.Dmg}{dmgSymbol} {perso2.SkillCooldown}{CDSymbol}";
                    if (perso2.IsDefended())
                        perso2Info += " {Vulnérable}";
                    perso1Info += String.Concat(Enumerable.Repeat(" ", LINESIZE-perso1Info.Length));
                }
                Console.WriteLine(perso1Info + perso2Info + "\n");
            }
            if (team2 != null)
                for(int i = team1.Count; i < team2.Count; i++)
                {
                    perso2 = team2[i];
                    perso1Info = "";
                    perso1Info = String.Concat(Enumerable.Repeat(" ", LINESIZE - perso1Info.Length));
                    perso2Info = $"{perso2.Name} : {CharacterHearts(perso2)} {perso2.Dmg}{dmgSymbol} {perso2.SkillCooldown}{CDSymbol}";
                    Console.WriteLine(perso1Info + perso2Info + "\n");
                }

            if (!(team2 is null))
                Console.WriteLine(String.Concat(Enumerable.Repeat("*", Console.WindowWidth)));
        }

        static string CharacterHearts(Character perso)
        {
            string fullHeart = "♥";
            string emptyHeart = "-";
            int fullAmount = perso.Hp;
            int emptyAmount = perso.MaxHp - perso.Hp;
            return String.Concat(Enumerable.Repeat(fullHeart, fullAmount)) + String.Concat(Enumerable.Repeat(emptyHeart, emptyAmount));
        }

        static int ArrowChoice(List<String> choices, int clearIndex, bool team2Playing = false)
        {
            /*for(int i = 0; i < choices.Count; i++)
                choices[i] = $"| {choices[i]} |";*/
            int choice = 0, lineWidth = 0;
            foreach (String c in choices)
                if (c.Length > lineWidth)
                    lineWidth = c.Length + 5;
            bool entered = false;
            string arrow = "<---", noArrow = "    ", displayChoices;
            while (!entered)
            {
                ClearUnder(clearIndex);
                displayChoices = "";
                for (int i = 0; i < choices.Count; i++)
                    displayChoices += (team2Playing ? String.Concat(Enumerable.Repeat(" ", LINESIZE)) : "") + choices[i] + String.Concat(Enumerable.Repeat(" ", lineWidth - choices[i].Length)) + (i == choice ? arrow : noArrow) + (i == choices.Count - 1 ? "" : "\n");
                Console.Write(displayChoices);

                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow && choice > 0)
                    choice -= 1;
                else if (key == ConsoleKey.DownArrow && choice < choices.Count-1)
                    choice += 1;
                if (key == ConsoleKey.Enter)
                    entered = true;
            }
            ClearUnder(clearIndex);
            return choice;
        }

        static void ClearUnder(int top)
        {
            Console.SetCursorPosition(0, top);
            Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth*(Console.WindowHeight-top-1))));
            Console.SetCursorPosition(0, top);
        }
        static void PlayerActionChoice(List<Character> team1, List<Character> team2, bool team2Playing = false)
        {
            int playerChoice;
            List<String> choices = new List<string>();
            foreach(Character perso in team1)
            {
                playerChoice = 0;
                if(team2Playing)
                    Console.SetCursorPosition(LINESIZE, Console.CursorTop);
                Console.WriteLine($"-- {perso.Name} --\n");
                choices.Clear();
                choices.Add("Attaquer");
                if(!perso.IsDefended())
                    choices.Add("Défendre");
                playerChoice = ArrowChoice(choices, GAMEHEIGHT + 2, team2Playing) + 1;
                // TARGETING
                if (playerChoice == 1) // Attacking
                {
                    perso.SetAttackState();
                    if (team2.Count > 1)
                    {
                        choices.Clear();
                        foreach (Character enemy in team2)
                            choices.Add(enemy.Name);
                        perso.Target = team2[ArrowChoice(choices, GAMEHEIGHT + 2, team2Playing)];
                    }
                    else
                        perso.Target = team2[0];
                }
                else // Defending
                    perso.SetDefendState();
                    

                if (perso.SkillCooldown == 0 && !(perso is Yokel) && !(perso is Illusionist && perso.IsDefended()) && !(perso is Tank && perso.IsDefended()))
                {
                    if (team2Playing)
                        Console.SetCursorPosition(LINESIZE, Console.CursorTop);
                    Console.WriteLine($"Utiliser la capacité spéciale :");
                    choices.Clear();
                    choices.Add("Non");
                    choices.Add("Oui");
                    playerChoice = ArrowChoice(choices, GAMEHEIGHT + 3, team2Playing);
                    if (playerChoice == 1) // Use special skill
                    {
                        perso.SpecialSkill();
                        choices.Clear();
                        foreach (Character ally in team1)
                            choices.Add(ally.Name);
                        switch (perso.GetType().Name)
                        {
                            case "Healer":
                                Healer p = (Healer)perso;
                                playerChoice = ArrowChoice(choices, GAMEHEIGHT + 3, team2Playing);
                                p.TargetToHeal = team1[playerChoice];
                                break;
                        }
                    }
                         
                }

                ClearUnder(GAMEHEIGHT);
            }
        }

        static void AiActionChoice(List<Character> team1, List<Character> team2)
        {
            Console.WriteLine($"------ ORDINATEUR ------\n");
            foreach (Character perso in team1)
            {
                Console.WriteLine($"-- {perso.Name} --\n");

                // TARGETING
                if (perso.IsDefended() || RANDOM.Next(0, 100) <= 70)
                {
                    perso.SetAttackState();
                    perso.Target = team2[RANDOM.Next(0, team2.Count)];

                    Console.WriteLine("Attaque {0} !", perso.Target.Name);
                }
                else
                {
                    perso.SetDefendState();
                    
                    Console.WriteLine("Se défend !");
                }
                
                if(RANDOM.Next(0, 100) <= 75 && perso.SkillCooldown == 0)
                {
                    switch (perso.GetType().Name)
                    {
                        case "Healer":
                            foreach (Character ally in team1)
                                if (ally.Hp < ally.MaxHp)
                                {
                                    Healer p = (Healer)perso;
                                    p.TargetToHeal = ally;
                                    perso.SpecialSkill();
                                    break;
                                }
                            break;
                        case "Tank":
                            if (!perso.IsDefended())
                                perso.SpecialSkill();
                            break;
                        case "Illusionist":
                            if (!perso.IsDefended())
                                perso.SpecialSkill();
                            break;
                        case "Yokel":
                            break;
                        default:
                            perso.SpecialSkill();
                            break;
                    }
                    if (perso.SpecialSkillUsed)
                        Console.WriteLine("Utilise sa compétence spéciale !");
                }

                Console.Write("\nEntrée pour continuer...");
                Console.ReadLine();
                ClearUnder(GAMEHEIGHT + 2);
            }
            ClearUnder(GAMEHEIGHT);
        }

        static Character PlayerCharacterChoice(List<Character> team, int equipe)
        {
            List<String> choices = new List<String>();
            int choice;
            choices.Add("Tank");
            choices.Add("Healer");
            choices.Add("Damager");
            choices.Add("Illusionist");
            choices.Add("Summoner");
            choices.Add("\nInfo des personnages");
            choices.Add("\nFINI !");
            do
            {
                DisplayGame(team, null, equipe);
                Console.WriteLine("Recrutez un personnage :\n");
                choice = ArrowChoice(choices, 4 + team.Count*2) + 1;
                if (choice == 6)
                    DisplayCharactersInfo();
            } while (choice == 6);

            Character newCharacter = CharacterChoice(choice, team);
            if(newCharacter is Character)
            {
                Console.Clear();
                Console.Write("Nom du personnage (Entrée pour passer) : ");
                string newCharacterName = Console.ReadLine();
                if (newCharacterName != "")
                    newCharacter.Name = newCharacterName;
            }
            
            return newCharacter;
        }

        static void DisplayCharactersInfo()
        {
            ClearUnder(0);
            Character tankInfo = new Tank();
            Tank.nbDefaultTank--;
            Character healerInfo = new Healer();
            Healer.nbrDefaultHealer--;
            Character damagerInfo = new Damager();
            Damager.nbrDefaultDamager--;
            Character illusionistInfo = new Illusionist();
            Illusionist.nbrDefaultIllu--;
            Character summonerInfo = new Summoner();
            Summoner.nbrDefaultSummoner--;
            Console.WriteLine($"Tank \n   {CharacterHearts(tankInfo)}   ATT : {tankInfo.Dmg}  CD : {tankInfo.MaxSkillCooldown} \n COMPETENCE SPE : {tankInfo.SkillDescription}\n\n");
            Console.WriteLine($"Healer \n   {CharacterHearts(healerInfo)}   ATT : {healerInfo.Dmg}  CD : {healerInfo.MaxSkillCooldown} \n  COMPETENCE SPE : {healerInfo.SkillDescription}\n\n");
            Console.WriteLine($"Damager \n   {CharacterHearts(damagerInfo)}   ATT : {damagerInfo.Dmg}  CD : {damagerInfo.MaxSkillCooldown} \n COMPETENCE SPE : {damagerInfo.SkillDescription}\n\n");
            Console.WriteLine($"Illusionist \n   {CharacterHearts(illusionistInfo)}   ATT : {illusionistInfo.Dmg}  CD : {illusionistInfo.MaxSkillCooldown} \n COMPETENCE SPE : {illusionistInfo.SkillDescription}\n\n");
            Console.WriteLine($"Summoner \n   {CharacterHearts(summonerInfo)}   ATT : {summonerInfo.Dmg}  CD : {summonerInfo.MaxSkillCooldown} \n COMPETENCE SPE : {summonerInfo.SkillDescription}\n\n");

            List<String> choices = new List<String>();
            choices.Add("Retour");
            ArrowChoice(choices, Console.WindowHeight-1);
        }

        static Character CharacterChoice(int choiceIndex, List<Character> team)
        {
            Character newCharacter;
            switch (choiceIndex)
            {
                case 1:
                    newCharacter = new Tank();
                    break;
                case 2:
                    newCharacter = new Healer();
                    break;
                case 3:
                    newCharacter = new Damager();
                    break;
                case 4:
                    newCharacter = new Illusionist();
                    break;
                case 5:
                    newCharacter = new Summoner();
                    break;
                default:
                    newCharacter = null;
                    break;
            }
            if (newCharacter is Character)
                newCharacter.Team = team;
            return newCharacter;
        }


        static bool EndGameCheck(List<Character> team1, List<Character> team2)
        {
            RemoveDeadCharacters(team1);
            RemoveDeadCharacters(team2);

            GAMEHEIGHT = 6 + Math.Max(team1.Count, team2.Count) * 2;

            if (team1.Count == 0 || team2.Count == 0)
                return true;
            return false;
        }

        static void RemoveDeadCharacters(List<Character> team)
        {
            List<Character> toRemove = new List<Character>();

            foreach (Character perso in team)
            {
                perso.SpecialSkillUsed = false;
                if (perso.Hp <= 0)
                    toRemove.Add(perso);
            }

            foreach (Character persoDead in toRemove)
            {
                Console.WriteLine($"\n{persoDead.Name} est mort !");
                team.Remove(persoDead);
                if (persoDead.GetType().Name == "Summoner")
                {
                    Summoner p = (Summoner)persoDead;
                    foreach (Yokel y in p.YokelList)
                        team.Remove(y);
                    p.YokelList.Clear();
                }
            }   
        }

        static void EndAnimation()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            string fin1 = @"
┌────────────────────────────────────────────┐
│                    xxxxxxxxx               │
│                    x      xx               │
│                    x      x                │
│                    x     xx                │
│                    xxxxxxx                 │
│                    xx xxxx                 │
│                  xxx  x  xx                │
│                 xx    x   x                │
│                xx     x   xx               │
│                x     xx    x               │
│               xx     x     x               │
│               x      xx    x               │
│               x     xxxx   x               │
│                   xxx  xx  xx              │
│                 xx      xx                 │
│                 x        x                 │
│                 x        xx                │
│            xxx x          xx xxx           │
│            xxxxx           x xxx           │
│            xxxx            xxxxx           │
│             xxx             xxx            │
│                                            │
│                                            │
└────────────────────────────────────────────┘";
            Console.WriteLine(fin1);
            Console.WriteLine("\nYour enemy has become extremely weak...");
            Thread.Sleep(3000);
            ClearUnder(0);
            string fin2 = @"
┌────────────────────────────────────────────┐
│                                            │
│xx                                          │
│ xx                                         │
│  xx                                        │
│   xx                                       │
│    xx                                    x │
│     xx                                 xx  │
│      x                                 x   │
│      x                                 xx  │
│  xxxxx                                  x  │
│xxxxxxx                                  xxx│
│xxxxxxx                                   xx│
│   xxx                                    x │
│     x                                  xxxx│
│    x                                xxx xxx│
│    x                             xxxx   x x│
│   xx                            xx     xx x│
│  xx                              xxxxxxx  x│
│xxx                                 xxxxxxxx│
xx                               xxxxx     xx│
│                            xxxxx   xx      │
│                         xxx         x      │
│                                     xx     │
└──────────────────────────────────────x─────┘";
            Console.WriteLine(fin2);
            Console.WriteLine("\nYou watch him, injured, laying against the wall.");
            Thread.Sleep(3000);
            ClearUnder(0);
            string fin4 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                      xxxxx                 │
│                    xxx   xxx               │
│                    x      xx               │
│                    x     xx                │
│                    xxxxxxx                 │
│                     x                      │
│                   xxx                      │
│                 xxxxxx                     │
│                xx xx xx                    │
│               x   x   x                    │
│              xx  xx   x                    │
│                  x                         │
│                 xx                         │
│                 xx                         │
│                 x x                        │
│                 x x                        │
│                 x x                        │
│                xx x                        │
│                x  x                        │
└────────────────────────────────────────────┘";
            string fin5 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                      xxxxx                 │
│                    xxx   xxx               │
│                    x      xx               │
│                    x     xx                │
│                    xxxxxxx                 │
│                     x                      │
│                    xx                      │
│                   xxx                      │
│                  xxx                       │
│                 x xx                       │
│                x xxx                       │
│                  x                         │
│                   x                        │
│                   xx                       │
│                   xxx                      │
│                   x xx                     │
│                   xxx                      │
│                xxxx                        │
│                xx x                        │
└────────────────────────────────────────────┘";
            string fin6 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                     xxxx                   │
│                    xx  xx                  │
│                   xx    xx                 │
│                   x     xx                 │
│                   xxx xxx                  │
│                     xxx                    │
│                   xxxx                     │
│                 xxxxx                      │
│                xx xxx                      │
│              xx   xx                       │
│              x    xxxx                     │
│                   x  xx                    │
│                   xx                       │
│                  xxxx                      │
│                  x  xx                     │
│                 x    xx                    │
│                 x     xx                   │
│               xxx      xx                  │
│               xx        xx                 │
│              xx                            │
└────────────────────────────────────────────┘";
            string fin7 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                     xxxx                   │
│                    xx  xx                  │
│                   xx    xx                 │
│                   x     xx                 │
│                   xxx xxx                  │
│                    xxxx                    │
│                   xxxx                     │
│                xxxx xx                     │
│               xx   xxxx                    │
│              xx    x  xxxx                 │
│              xx    x                       │
│               x    x                       │
│                    x                       │
│                   xxx                      │
│                  xxxx                      │
│                  x  xx                     │
│                 xx   x                     │
│                xx    xx                    │
│             xxxx      x                    │
└────────────────────────────────────────────┘";
            string fin8 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                      xxxxx                 │
│                     xx   xx                │
│                    xx     x                │
│                    x     xx                │
│                    xxxxxxx                 │
│                     xxx                    │
│                    xxx                     │
│                    xxx                     │
│                    xxx                     │
│                   xxxxx                    │
│                  xx xxxx                   │
│                     xx                     │
│                      xx                    │
│                      xxx                   │
│                      x xx                  │
│                     xx xx                  │
│                     xxxx                   │
│                   xxxx                     │
│                 xxx x                      │
│                     x                      │
└────────────────────────────────────────────┘";
            string fin9 = @"
┌────────────────────────────────────────────┐
│                                            │
│                      xxx                   │
│                    xxx xx                  │
│                    x    xx                 │
│                    x    xx                 │
│                    xx  xx                  │
│                     xxxx                   │
│                     xxx                    │
│                   xxxx                     │
│                  xx  xx                    │
│                 xx   xxx xx                │
│                 x    x xxx                 │
│                 x    x                     │
│                     xx                     │
│                     xxx                    │
│                     x x                    │
│                     x xx                   │
│                     x  xx                  │
│                    xx   xx                 │
│                    x     xx                │
│                   xx      xx               │
│                  xx        x               │
│                 xx                         │
└────────────────────────────────────────────┘";

            for (int i = 0; i < 20; i++)
            {
                ClearUnder(0);

                switch (i % 4)
                {
                    case 0:
                        {
                            Console.WriteLine(fin6);
                            break;
                        };
                    case 1:
                        {
                            Console.WriteLine(fin7);
                            break;
                        };
                    case 2:
                        {
                            Console.WriteLine(fin8);
                            break;
                        };
                    case 3:
                        {
                            Console.WriteLine(fin9);
                            break;
                        };

                }
                Thread.Sleep(150);
            }

            ClearUnder(0);
            string fin10 = @"
┌────────────────────────────────────────────┐
│                                            │
│                 xxxxxxxxxxxxxxx            │
│           xxxxxxx             xxxx         │
│       xxxxx                      xxxx      │
│    xxx                              xx     │
│   xx                                 xx    │
│  xx                                   xx   │
│  x   x xxxxxxxxxxxxxxxxxxx x x x x     xx  │
│ xx                  xxx            xxxx x  │
│ x                  xx xx                x  │
│xx            xxxxxxx   x xxxxxxxxx      xx │
│x       xxxxxx  x   x     x  xx   xxxxxx  x │
│x       x   x   x   x     x  x    x   xx  x │
│        x   x   x  xx     x  x    x   x   x │
│x       xx  xx xx  x      x  x   xx  xx   x │
│x        xx  xxx xx        x xxxxx  xx   xx │
│x         xxxxxxxx         xxx     xx    x  │
│x                            xxxxxxx    xx  │
│xx                                     xx   │
│ x            xxxxxxxxxxx              x    │
│ xx                                   xx    │
│  x                                  xx     │
│  xx                               xx       │
└────────────────────────────────────────────┘

Our hero walks towards him...";
            Console.WriteLine(fin10);
            Console.WriteLine();
            Thread.Sleep(700);
            ClearUnder(0);
            string fin11 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                 xxxxxxxxxxxxxxx            │
│           xxxxxxx             xxxx         │
│       xxxxx                      xxxx      │
│    xxx                              xx     │
│   xx                                 xx    │
│  xx                                   xx   │
│  x   x xxxxxxxxxxxxxxxxxxx x x x x     xx  │
│ xx                  xxx            xxxx x  │
│ x                  xx xx                x  │
│xx            xxxxxxx   x xxxxxxxxx      xx │
│x       xxxxxx  x   x     x  xx   xxxxxx  x │
│x       x   x   x   x     x  x    x   xx  x │
│        x   x   x  xx     x  x    x   x   x │
│x       xx  xx xx  x      x  x   xx  xx   x │
│x        xx  xxx xx        x xxxxx  xx   xx │
│x         xxxxxxxx         xxx     xx    x  │
│x                            xxxxxxx    xx  │
│xx                                     xx   │
│ x            xxxxxxxxxxx              x    │
│ xx                                   xx    │
│  x                                  xx     │
└──xx───────────────────────────────xx───────┘

Our hero walks towards him...";
            ClearUnder(0);
            int delta = 400;
            Console.WriteLine(fin11);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin10);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin11);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin10);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin11);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin10);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin11);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            Console.WriteLine(fin10);
            Console.WriteLine();
            Thread.Sleep(delta);
            ClearUnder(0);
            string fin12 = @"
┌─────────x──────────────────────────────────┐
│         xx                                 │
│          x                                 │
│          x                                 │
│          x                                 │
│          x                                 │
│          x                                 │
│          x                                 │
│          xx                                │
│         xxx                                │
│         x x                                │
│         x                                  │
│         x  x                               │
│         x  x                          xxxx │
│        x   x                         xx  x │
│            x                         x    x│
│        x   x                         xx xx │
│        x   x                          xxx  │
│        x   x                         xx    │
│        x   x                        xx     │
│        x   x                       xxx     │
│       xx   x          xxx        xxxx      │
│       x    x       xxxx xxxxxxxxxxx        │
│       x    x     xxxxxxxxxxxxxxxx          │
└───────x────────────────x──────x────────────┘";

            Console.WriteLine(fin12);
            Console.WriteLine();
            Thread.Sleep(2000);
            ClearUnder(0);
            string fin13 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                xxxxx                       │
│                x   xx                      │
│                x    x                      │
│              xxxx   x                      │
│           xxxxxxxxxxx                      │
│         xxx   x  xx                        │
│      xxx     xx   xx                  xxxx │
│     xx      xx    xx                 xx  x │
│     x      xx     x                  x    x│
│     x     xxxx    x                  xx xx │
│     x  xxxx  xxxxx                    xxx  │
│     xxx         xx                   xx    │
│    xx           xx                  xx     │
│    xx          xx                  xxx     │
│     xx       xxx      xxx        xxxx      │
│      x      xx     xxxx xxxxxxxxxxx        │
│      xx    xx    xxxxxxxxxxxxxxxx          │
└───────x────────────────x──────x────────────┘

You stand in front of your enemy, as he is trying to breathe.";

            Console.WriteLine(fin13);
            Console.WriteLine();
            Thread.Sleep(1500);
            ClearUnder(0);
            string fin14 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                xxxxx                       │
│                x   xx                      │
│              xxx    x                      │
│           xxxxxxx   x                      │
│         xxx   xxxxxxx                      │
│      xxx     xx   xx                  xxxx │
│     xx      xx    xx                 xx  x │
│     x      xx     x                  x    x│
│     x     xxxx    x                  xx xx │
│     x  xxxx  xxxxx                    xxx  │
│     xxx         xx                   xx    │
│    xx           xx                  xx     │
│    xx          xx                  xxx     │
│     xx       xxx      xxx        xxxx      │
│      x      xx     xxxx xxxxxxxxxxx        │
│      xx    xx    xxxxxxxxxxxxxxxx          │
└───────x────────────────x──────x────────────┘";

            Console.WriteLine(fin14);
            Console.WriteLine();
            Thread.Sleep(700);
            ClearUnder(0);
            string fin15 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│             *sigh*                         │
│                                            │
│                xxxxx                       │
│                x   xx                      │
│              xxx xx x                      │
│           xxxxxxx xxx                      │
│         xxx   xxxxxxx                      │
│      xxx     xxx    x                 xxxx │
│     xx      xx  xxxxx                xx  x │
│     x      xx                        x    x│
│     x     xxxx                       xx xx │
│     x  xxxx  xxxxx                    xxx  │
│     xxx         xx                   xx    │
│    xx           xx                  xx     │
│    xx          xx                  xxx     │
│     xx       xxx      xxx        xxxx      │
│      x      xx     xx    xxxxxxxxxx        │
│      xx    xx    xxxxxxxxxxxxxxxx          │
└───────x────────────────x──────x────────────┘";

            Console.WriteLine(fin15);
            Console.WriteLine();
            Thread.Sleep(2000);
            ClearUnder(0);
            string fin16 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│       WHAT IS WRONG WITH YOU?              │
│                                            │
│              xxxxx                         │
│              x   xx                        │
│              x    x          xxx x         │
│              xx   x x x xx xx              │
│             xxxxxxxxx                      │
│             xxxxx                          │
│            xxxx                       xxxx │
│            xxxxx                     xx  x │
│            xx  xxxxxxxxx             x    x│
│           xxxx          xxx          xx xx │
│         xxx  xxxxx                    xxx  │
│         xx      xx                   xx    │
│          x      xx                  xx     │
│          x     xx                  xxx     │
│         xx   xxx      xxx        xxxx      │
│       xx    xx     xx    xxxxxxxxxx        │
│      xx    xx    xxxxxxxxxxxxxxxx          │
└─────xxx────────────────x──────x────────────┘";

            Console.WriteLine(fin16);
            Console.WriteLine();
            Thread.Sleep(3000);
            ClearUnder(0);
            string fin17 = @"
┌────────────────────────────────────────────┐
│                                            │
│     OH MY GOD, THINK, CHAD, THINK!         │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│          xxxxxx                            │
│          x    xx                           │
│      xxxxx     xxx                         │
│  xxxxx  xx    xx xxxxx                xxxx │
│   xxx    xxxxxx       xx             xx  x │
│      xxx  x          xxx             x    x│
│        xxxxxxxxxxxxx                 xx xx │
│          xx                           xxx  │
│          x                           xx    │
│          x                          xx     │
│  xxxxxxxxxxxxxxxx                  xxx     │
│  xx          xxx      xxx        xxxx      │
│   xxx       xx     xx    xxxxxxxxxx        │
│     xx     xx    xxxxxxxxxxxxxxxx          │
└─────xxx────────────────x──────x────────────┘";

            Console.WriteLine(fin17);
            Console.WriteLine();
            Thread.Sleep(3000);
            ClearUnder(0);
            string fin18 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│                                            │
│                                            │
│              .    .     .                  │
│                                            │
│                                            │
│                                            │
│                                            │
│          xxxxxx                            │
│          x    xx                           │
│         xx     x                           │
│         xx    xx                      xxxx │
│          xxxxxx                      xx  x │
│       xxxxx                          x    x│
│     xxx xxxx                         xx xx │
│   xx     xxxxx                        xxx  │
│  xx      x   xx                      xx    │
│  xxx     x   xx                     xx     │
│  xxxxxxxxxxxxxxxx                  xxx     │
│  xx   xx    xxxx      xxx        xxxx      │
│   xxx       xx     xx    xxxxxxxxxx        │
│     xx     xx    xxxxxxxxxxxxxxxx          │
└─────xxx────────────────x──────x────────────┘";

            Console.WriteLine(fin18);
            Console.WriteLine();
            Thread.Sleep(1500);
            ClearUnder(0);
            string fin19 = @"
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│    YOUR FRIENDS ARE GONE NOW.              │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                                            │
│                   xxxxx                    │
│                   x    x              xxxx │
│                 xxx    x             xx  x │
│              xxxxxxx  xx             x    x│
│           xxxxxx x xxxx              xx xx │
│        xxxx  xx  xx                   xxx  │
│     xxxx    xx    xxxxxxxxxxxx       xx    │
│     x    xxx                        xx     │
│  xxxxxxxxxxxxxxxx                  xxx     │
│  xx   xx    xxxx      xxx        xxxx      │
│   xxx       xx     xx    xxxxxxxxxx        │
│     xx     xx    xxxxxxxxxxxxxxxx          │
└─────xxx────────────────x──────x────────────┘";

            Console.WriteLine(fin19);
            Console.WriteLine();
            Thread.Sleep(3000);
            ClearUnder(0);
            string fin20 = @"
┌────────────────────────────────────────────┐
│                                            │
│                    x    OooOooOoOoo        │
│                    xxx                     │
│     xx                xx  I'm SO smart     │
│     xxx            xxxxx                   │
│       xxx        xxx                       │
│        xx        xxx                       │
│       xx     xxx   xx                      │
│       xx     xxxxxxxx                      │
│        xxx  xx  xx                         │
│          x  x    x                         │
│          x  xxx xx                         │
│         x    xxx                      xxxx │
│         xx  xx                       xx  x │
│          x  x                        x    x│
│           xxx                        xx xx │
│           xx                          xxx  │
│          xx                          xx    │
│          x                          xx     │
│  xxxxxxxxxxxxxxxx                  xxx     │
│  xx   xx    xxxx      xxx        xxxx      │
│   xxx       xx     xx    xxxxxxxxxx        │
│     xx     xx    xxxxxxxxxxxxxxxx          │
└─────xxx────────────────x──────x────────────┘";

            Console.WriteLine(fin20);
            Console.WriteLine();
            Thread.Sleep(2000);
            ClearUnder(0);

            string fin21 = @"
┌────────────────────────────────────────────┐
│AAAAAAAAAAAAAAAAAxxxxxxxx xAAAAAAAAAAAAAAAAA│
│AAAAAAAAAAAAxxxxxxxxxxxxxxxxxAAAAAAAAAAAAAAA│
│         xxxx            xx  xx             │
│        xxxx           xxx     xx           │
│      xx    xxx        xx     x    x          │
│     xx   x    xx      xxxx       xx         │
│     x       xxx          xxxxx    x        │
│    xx    xxxx xxxxxxxx            xx       │
│    x         xx      xxx           x       │
│    x        xx         xx          x       │
│    x       xx           x        xxxxxx    │
│    xxxx    x            xx      xxxxxxxxx  │
│  xxxxxxx   x             x       xxxxxxxx  │
│  xxxxxxx   x             x       xxxxxxxx  │
│ xxxxxxxx   x xxxxxxxxxxx x        xxxxxxx  │
│ xxxxxxxx   xx          xxx        xx    xx │
│ xxxxxxxx                          x      xx│
│xx  xxxxx                        xx        x│
│x       xxxx                    xx          │
│           xxxxx              xxx      x    │
│xxx             xxxxxxxxxxxxxxx      xxx    │
│  xxxx              x             xxx       │
│     xxx            x            xx         │
└────────────────────x───────────────────────┘";

            Console.WriteLine(fin21);
            Console.WriteLine();
            Thread.Sleep(3000);

            Console.ReadLine();
        }
    }
}

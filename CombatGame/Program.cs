using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace CombatGame
{
    class Program
    {

        static int GAMEHEIGHT, LINESIZE = 60;
        static void Main(string[] args)
        {
            // Enregistre les personnages de chaque équipes
            List<Character> team1 = new List<Character>();
            List<Character> team2 = new List<Character>();

            // Contre IA ?
            bool versusAI = ChoosePlayMode();
            Character choice;

            // Equipe 1 choisit ses perso
            Console.Clear();
            do {
                choice = PlayerCharacterChoice(team1, 1);
                if (choice is Character)
                    team1.Add(choice);
            }  while (choice != null || team1.Count < 1);

            // L'IA choisit sont équipe
            if (versusAI)
                for (int i = 0; i < team1.Count; i++)
                    team2.Add(AICharacterChoice());
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

            GAMEHEIGHT = 2 + Math.Max(team1.Count, team2.Count) * 2;
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

                // CALCUL DES CHOIX ET FIN DE TOUR
                foreach (Character perso in team1)
                    perso.EndTurn();
                foreach (Character perso in team2)
                    perso.EndTurn();

                // CHECK ENDGAME et retire les persos mort des équipes
                endGame = EndGameCheck(team1, team2);

                if (versusAI)
                {
                    Console.WriteLine("\nEntrée pour continuer...");
                    Console.ReadLine();
                }
            }
            DisplayGame(team1, team2);
            Console.WriteLine("\nFIN");
            Console.ReadLine();

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
            Console.Clear();
            Console.WriteLine("Equipe " + equipe + " :" + String.Concat(Enumerable.Repeat(" ", LINESIZE - 10)) + (team2 is null ? "" : "Equipe 2 :") + "\n");
            for (int i = 0; i<team1.Count; i++)
            {
                Character perso1 = team1[i], perso2;
                string perso1Info = $"{ perso1.Name} : HP = { perso1.Hp} ATK = { perso1.Dmg} Cooldown = { perso1.SkillCooldown}", perso2Info = "";
                if (!(team2 is null) && i < team2.Count)
                {
                    perso2 = team2[i];
                    perso2Info = $"{perso2.Name} : HP = {perso2.Hp} ATK = {perso2.Dmg} Cooldown = {perso2.SkillCooldown}";
                    perso1Info += String.Concat(Enumerable.Repeat(" ", LINESIZE-perso1Info.Length));
                }
                Console.WriteLine(perso1Info + perso2Info + "\n");
            }
        }

        static int ArrowChoice(List<String> choices, int clearIndex, bool team2Playing = false)
        {
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
            bool useSpecialAttack;
            List<String> choices = new List<string>();
            foreach(Character perso in team1)
            {
                useSpecialAttack = false;
                playerChoice = 0;
                if(team2Playing)
                    Console.SetCursorPosition(LINESIZE, Console.CursorTop);
                Console.WriteLine($"-- {perso.Name} --\n");
                choices.Clear();
                choices.Add("Attaquer");
                choices.Add("Défendre");
                playerChoice = ArrowChoice(choices, GAMEHEIGHT + 2, team2Playing) + 1;
                // TARGETING
                if (playerChoice == 1)
                {
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

                if (perso.SkillCooldown == 0)
                {
                    if (team2Playing)
                        Console.SetCursorPosition(LINESIZE, Console.CursorTop);
                    Console.WriteLine($"Utiliser la capacité spéciale :");
                    choices.Clear();
                    choices.Add("Non");
                    choices.Add("Oui");
                    playerChoice = ArrowChoice(choices, GAMEHEIGHT + 3, team2Playing);
                    if (playerChoice == 1)
                         useSpecialAttack = true;
                }

                ActionChoice(perso, playerChoice, useSpecialAttack);
                ClearUnder(GAMEHEIGHT);
            }
        }

        static void AiActionChoice(List<Character> team1, List<Character> team2)
        {
            foreach (Character perso in team1)
            {
                bool useSpecialAttack = false;
                Random random = new Random();
                Console.WriteLine($"{perso.Name} réfléchit...");
                Thread.Sleep(1000);

                int aiChoice = random.Next(1, 3);
                if (random.Next(1, 3) == 1 && perso.SkillCooldown == 0)
                    useSpecialAttack = true;

                // TARGETING
                if (aiChoice == 1)
                {
                    int targetIndex = random.Next(0, team2.Count);
                    perso.Target = team2[targetIndex];

                    Console.WriteLine("{0} attaque votre {1} !\n\n", perso.Name, perso.Target.Name);
                }
                else
                    Console.WriteLine("{0} se défend !\n\n", perso.Name);

                Thread.Sleep(500);
                ActionChoice(perso, aiChoice, useSpecialAttack, true);
            }
        }

        static void ActionChoice(Character perso, int choiceIndex, bool useSpecialAttack, bool aiPlaying = false)
        {
            if (choiceIndex == 1)
                perso.SetAttackState();
            else
                perso.SetDefendState();

            if (useSpecialAttack)
            {
                if (aiPlaying)
                {
                    Console.WriteLine("{0} active son attaque spécial !", perso.Name);
                    Thread.Sleep(500);
                }
                perso.SpecialSkill();
            }
        }

        static Character PlayerCharacterChoice(List<Character> team, int equipe)
        {
            List<String> choices = new List<String>();
            int choice;
            choices.Add("Tank");
            choices.Add("Healer");
            choices.Add("Damager");
            choices.Add("Illusionist");
            choices.Add("\nInfo des personnages");
            choices.Add("\nFINI !");
            do
            {
                DisplayGame(team, null, equipe);
                Console.WriteLine("Recrutez un personnage :\n");
                choice = ArrowChoice(choices, 4 + team.Count*2) + 1;
                if (choice == 5)
                    DisplayCharactersInfo();
            } while (choice == 5);

            Character newCharacter = CharacterChoice(choice);
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
            Character healerInfo = new Healer();
            Character damagerInfo = new Damager();
            Character illusionistInfo = new Illusionist();
            Console.WriteLine($"Tank \n   HP : {tankInfo.Hp}   ATT : {tankInfo.Dmg} \n   COMPETENCE SPE : {tankInfo.SkillDescription}\n\n");
            Console.WriteLine($"Healer \n   HP : {healerInfo.Hp}   ATT : {healerInfo.Dmg} \n   COMPETENCE SPE : {healerInfo.SkillDescription}\n\n");
            Console.WriteLine($"Damager \n   HP : {damagerInfo.Hp}   ATT : {damagerInfo.Dmg} \n   COMPETENCE SPE : {damagerInfo.SkillDescription}\n\n");
            Console.WriteLine($"Illusionist \n   HP : {illusionistInfo.Hp}   ATT : {illusionistInfo.Dmg} \n   COMPETENCE SPE : {illusionistInfo.SkillDescription}\n\n");

            List<String> choices = new List<String>();
            choices.Add("Retour");
            ArrowChoice(choices, Console.WindowHeight-1);
        }

        static Character AICharacterChoice()
        {
            Random random = new Random();
            int aiCharacterChoice = random.Next(1, 5); //CHOICE OF AI

            Character aiCharacter = CharacterChoice(aiCharacterChoice);
            Console.WriteLine("");
            Console.WriteLine("Ordinateur : J'ai choisi " + aiCharacter.Name);
            Console.WriteLine("");
            return aiCharacter;

        }

        static Character CharacterChoice(int choiceIndex)
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
                default:
                    newCharacter = null;
                    break;
            }
            return newCharacter;
        }


        static bool EndGameCheck(List<Character> team1, List<Character> team2)
        {
            List<Character> toRemove = new List<Character>();

            foreach (Character perso in team1)
                if (perso.Hp <= 0)
                    toRemove.Add(perso);
            foreach (Character persoDead in toRemove)
                team1.Remove(persoDead);

            toRemove.Clear();

            foreach (Character perso in team2)
                if (perso.Hp <= 0)
                    toRemove.Add(perso);
            foreach (Character persoDead in toRemove)
                team2.Remove(persoDead);

            GAMEHEIGHT = 2 + Math.Max(team1.Count, team2.Count) * 2;

            if (team1.Count == 0 || team2.Count == 0)
                return true;
            return false;
        }
    }
}

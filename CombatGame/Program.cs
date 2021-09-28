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
        static void Main(string[] args)
        {
            // Enregistre les personnages de chaque équipes
            List<Character> team1 = new List<Character>();
            List<Character> team2 = new List<Character>();

            // Contre IA ?
            bool versusAI = ChoosePlayMode();
            Character choice;

            // Equipe 1 choisit ses perso
            Console.WriteLine("Equipe 1 choisit sont équipe...");
            do {
                Console.Clear();
                if (team1.Count > 0)
                {
                    Console.Write("Ton équipe : ");
                    foreach (Character perso in team1)
                        Console.Write("  " + perso.Name);
                    Console.WriteLine();
                }

                choice = PlayerCharacterChoice();
                if (choice is Character)
                    team1.Add(choice);
            }  while (choice != null || team1.Count < 1);

            Console.WriteLine("");
            if (versusAI)
            {
                // L'IA choisit sont équipe
                Console.WriteLine("L'ordi choisit sont équipe...");
                for (int i = 0; i < team1.Count; i++)
                    team2.Add(AICharacterChoice());
            }
            else
            {
                // Equipe 2 choisit ses perso
                Console.WriteLine("Equipe 2 choisit sont équipe...");
                do
                {
                    choice = PlayerCharacterChoice();
                    if (choice is Character)
                        team2.Add(choice);
                } while (choice != null || team2.Count < 1);
            }

            Console.Clear();

            // BOUCLE JEU
            bool endGame = false;
            while (!endGame)
            {
                // DISPLAY GAME
                DisplayGame(team1, team2);
                Console.WriteLine("");

                // PLAYER CHOICE
                PlayerActionChoice(team1, team2);
                if (versusAI)
                    AiActionChoice(team2, team1);
                else
                    PlayerActionChoice(team2, team1);

                // CALCUL DES CHOIX ET FIN DE TOUR
                foreach (Character perso in team1)
                    perso.EndTurn();
                foreach (Character perso in team2)
                    perso.EndTurn();

                // CHECK ENDGAME et retire les persos mort des équipes
                endGame = EndGameCheck(team1, team2);

                Console.WriteLine("");
                Console.WriteLine("Entrée pour continuer...");
                Console.ReadLine();
                Console.Clear();
            }

            Console.WriteLine("FIN");
            Console.ReadLine();

        }

        static bool ChoosePlayMode()
        {
            Console.WriteLine("Comment voulez-vous jouer ?");
            Console.WriteLine("(1) Joueur VS AI      (2) Joueur VS Joueur");
            int answer = int.Parse(Console.ReadLine());
            if (answer == 1) return true;
            else return false;
            
        }

        static void DisplayGame(List<Character> team1, List<Character> team2)
        {
            Console.WriteLine("Equipe 1 :");
            for (int i = 0; i<team1.Count; i++)
            {
                Character perso = team1[i];
                Console.WriteLine($"{perso.Name} : HP = {perso.Hp} ATK = {perso.Dmg} Cooldown = {perso.SkillCooldown}");
            }
            Console.WriteLine();

            Console.WriteLine("Equipe 2 :");
            for (int i = 0; i < team1.Count; i++)
            {
                Character perso = team2[i];
                Console.WriteLine($"{perso.Name} : HP = {perso.Hp} ATK = {perso.Dmg} Cooldown = {perso.SkillCooldown}");
            }
            Console.WriteLine("");
        }

        static void PlayerActionChoice(List<Character> playerTeam, List<Character> aiTeam)
        {
            foreach(Character perso in playerTeam)
            {
                bool useSpecialAttack = false;
                int playerChoice;
                Console.WriteLine("Choisissez une action a effectuer pour {0}", perso.Name);
                Console.Write("(1) Attack  /  (2) Defend  : ");
                playerChoice = int.Parse(Console.ReadLine());

                // TARGETING
                if (playerChoice == 1)
                {
                    if (aiTeam.Count > 1)
                        while (true)
                        {
                            Console.Write($"Choisissez un enemie à attaquer (1) {aiTeam[0].Name} / (2) {aiTeam[1].Name}  : ");
                            int enemyIndex = int.Parse(Console.ReadLine());
                            if (enemyIndex < aiTeam.Count)
                                perso.Target = aiTeam[enemyIndex - 1];
                                break;
                            Console.WriteLine("Choisissez un ennemi valide");
                        }
                    else
                        perso.Target = aiTeam[0];
                }

                if (perso.SkillCooldown == 0)
                {
                    Console.WriteLine("Votre personnage peut utiliser sa capacité spécial, voulez vous l'utiliser ?");
                    Console.Write("(1) Utiliser  /  (2) Ne pas utiliser : ");
                    int secondPlayerChoice = int.Parse(Console.ReadLine());
                    if (secondPlayerChoice == 1)
                         useSpecialAttack = true;
                }

                ActionChoice(perso, playerChoice, useSpecialAttack);
                Console.WriteLine("");
            }
        }

        static void AiActionChoice(List<Character> aiTeam, List<Character> playerTeam)
        {
            foreach (Character perso in aiTeam)
            {
                bool useSpecialAttack = false;
                Random random = new Random();
                Console.WriteLine("L'Ordinateur choisit une action à effectuer...");
                Console.WriteLine("");
                Thread.Sleep(1000);

                int aiChoice = random.Next(1, 3);
                if (random.Next(1, 3) == 1 && perso.SkillCooldown == 0)
                    useSpecialAttack = true;

                // TARGETING
                Console.WriteLine("");
                if (aiChoice == 1)
                {
                    int targetIndex = random.Next(0, playerTeam.Count);
                    perso.Target = playerTeam[targetIndex];

                    Console.WriteLine("Ordi : {0} attaque votre {1} !", perso.Name, perso.Target.Name);
                }
                else
                    Console.WriteLine("Ordi : {0} se défend !", perso.Name);

                Thread.Sleep(500);
                ActionChoice(perso, aiChoice, useSpecialAttack);
            }
        }

        static void ActionChoice(Character perso, int choiceIndex, bool useSpecialAttack)
        {
            if (choiceIndex == 1)
                perso.SetAttackState();
            else
                perso.SetDefendState();

            if (useSpecialAttack)
            {
                Console.WriteLine("");
                Console.WriteLine("{0} active son attaque spécial !", perso.Name);
                perso.SpecialSkill();
                Thread.Sleep(500);
            }
        }

        static Character PlayerCharacterChoice()
        {
            Character tankInfo = new Tank();
            Character healerInfo = new Healer();
            Character damagerInfo = new Damager();
            Character illusionistInfo = new Illusionist();

            Console.WriteLine("\nChoississez un personnage a ajouter à votre équipe :\n");
            Console.WriteLine("(0) - MON EQUIPE EST FAITE !");
            Console.WriteLine();
            Console.WriteLine("(1) - Tank \n   HP : {0}   ATT : {1} \n   COMPETENCE SPE : {2}\n", tankInfo.Hp, tankInfo.Dmg, tankInfo.SkillDescription);
            Console.WriteLine();
            Console.WriteLine("(2) - Healer \n   HP : {0}   ATT : {1} \n   COMPETENCE SPE : {2}\n", healerInfo.Hp, healerInfo.Dmg, healerInfo.SkillDescription);
            Console.WriteLine();
            Console.WriteLine("(3) - Damager \n   HP : {0}   Attaque : {1} \n   COMPETENCE SPE : {2}\n", damagerInfo.Hp, damagerInfo.Dmg, damagerInfo.SkillDescription);
            Console.WriteLine();
            Console.WriteLine("(4) - Illusionist \n   HP : {0}   Attaque : {1} \n   COMPETENCE SPE : {2}\n", illusionistInfo.Hp, illusionistInfo.Dmg, illusionistInfo.SkillDescription);
            Console.WriteLine();
            Console.Write("Votre choix : ");

            int answer = -1;
            
            do {
                try
                {
                    answer = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Joue pas au con et entre un chiffre...");
                }
            } while (answer > 4 || answer < 0);

            Character newCharacter = CharacterChoice(answer);
            if(newCharacter is Character)
            {
                Console.Write("Choisissez un nom pour votre personnage : ");
                string newCharacterName = Console.ReadLine();
                if (newCharacterName != "")
                    newCharacter.Name = newCharacterName;
            }
            
            return newCharacter;
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

            if (team1.Count == 0 || team2.Count == 0)
                return true;
            return false;
        }
    }
}

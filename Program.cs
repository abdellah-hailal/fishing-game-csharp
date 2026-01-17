using FishingCardGame;

public static class Program
{
    public static async Task Main()
    {
        bool jouer = true;
        while (jouer)
        {
            // Saisir le nombre de joueurs
            int nbJoueurs;
            do
            {
                Console.Write("Combien de joueurs ? (2 à 4) : ");
            } while (!int.TryParse(Console.ReadLine(), out nbJoueurs) || nbJoueurs < 2 || nbJoueurs > 4);

            // Saisir le nombre de cartes distribuées à chaque joueur
            int nbCartes;
            do
            {
                Console.Write("Combien de cartes par joueur ? (5 à 8) : ");
            } while (!int.TryParse(Console.ReadLine(), out nbCartes) || nbCartes < 5 || nbCartes > 8);

            // Création du paquet complet et mélangé
            var cardPair = new CardPair();
            cardPair.Shuffle();

            // Création des joueurs
            var players = new List<Player>();
            for (int i = 0; i < nbJoueurs; i++)
            {
                Console.WriteLine($"--- Joueur {i + 1} ---");
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine() ?? $"Joueur{i + 1}";
                Console.Write("Nom : ");
                string nom = Console.ReadLine() ?? $"Nom{i + 1}";
                players.Add(new Player(nom, prenom));
            }
            Console.WriteLine();
            
            //  Attribution d'une stratégie aléatoire à chaque joueur
            Random rnd = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                int choix = rnd.Next(1, 4); 
                switch (choix)
                {
                    case 1:
                        players[i].Strategy = new BasicStrategy();
                        Console.WriteLine($"!! {players[i]} adopte une stratégie de base !!");
                        break;
                    case 2:
                        players[i].Strategy = new MinimizePointsStrategy();
                        Console.WriteLine($"!! {players[i]} adopte la stratégie de minimisation des points !!");
                        break;
                    case 3:
                        // Pour la stratégie de blocage, on choisit un joueur cible aléatoire différente du joueur lui-même
                        var autres = players.Where(p => p != players[i]).ToList();
                        var cible = autres[rnd.Next(autres.Count)];
                        players[i].Strategy = new BlockOpponentStrategy(cible);
                        Console.WriteLine($"!! {players[i]} adopte une stratégie de blocage contre {cible} !!");
                        break;
                }
            }


            // Distribution des cartes aux joueurs
            for (int i = 0; i < nbCartes; i++)
            {
                foreach (var player in players)
                {
                    var card = cardPair.Cards[0];
                    cardPair.Cards.RemoveAt(0);
                    player.AddCard(card);
                }
            }
            Console.WriteLine();

            // Création des piles et lancement du jeu
            var remaining = cardPair.Cards.ToList();
            var drawStack = new DrawStack(remaining); // pile de pioche
            var depositStack = new DepositStack(); // pile de dépot
            var board = new GameBoard(players, drawStack, depositStack);
            var game = new FishingGame(board);
            
            // Abonnement de la console au logger du jeu 
            game.OnLog += Console.WriteLine;

            // lancer le jeu
            await game.Start();

            // Menu de fin de jeu
            bool menu = true;
            while (menu)
            {
                Console.WriteLine("\n=== Menu de fin de partie ===");
                Console.WriteLine("1 - Afficher les points");
                Console.WriteLine("2 - Afficher les cartes restantes des joueurs perdants");
                Console.WriteLine("3 - Relancer une nouvelle partie");
                Console.WriteLine("4 - Quitter");
                Console.Write("Votre choix : ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        // Affiches les points de chaque joueur
                        Console.WriteLine("\n--- Points ---");
                        foreach (var entry in game.Scores.OrderBy(kv => kv.Value))
                            Console.WriteLine($"{entry.Key}: {entry.Value} points");
                        break;

                    case "2":
                        // Affiche les cartes restantes de chaque joueur perdant
                        Console.WriteLine("\n--- Cartes des joueurs perdants ---");
                        foreach (var kv in game.LostCards.Where(x => x.Value.Count > 0))
                            Console.WriteLine($"{kv.Key}: {string.Join(", ", kv.Value)}");
                        break;

                    case "3":
                        // Relancer une nouvelle partie 
                        Console.WriteLine("\n--- Nouvelle partie ! ---");
                        menu = false; 
                        break;

                    case "4":
                        // Arrêter le jeu 
                        Console.WriteLine("--- Merci d’avoir joué ! ---");
                        menu = false;
                        jouer = false; 
                        break;

                    default:
                        Console.WriteLine("--- Choix invalide, réessayez. ---");
                        break;
                }
            }
        }
    }
}

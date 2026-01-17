using FishingCardGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FishingGame
{
    private GameBoard _board;
    private int _direction = 1; // Sens de jeu : 1 = horaire et -1 = sens inverse 
    private int _currentIndex;  // index du joueur courant
    private Random _rnd = new Random();
    public event Action<string> OnLog; // pattern observateur 

    public Dictionary<Player, int> Scores { get; private set; } = new Dictionary<Player, int>();
    public Dictionary<Player, List<Card>> LostCards { get; private set; } = new Dictionary<Player, List<Card>>();

    // indique si une carte Valet impose une couleur particulière
    private bool _forceColorFromValet = false;

    public FishingGame(GameBoard board)
    {
        _board = board;
    }

    public async Task Start()
    {
        // On choisit aléatoirement le premier joueur
        _currentIndex = _rnd.Next(_board.Players.Count);
        // On pose une carte pour commencer le jeu
        var firstCard = _board.DrawStack.Draw();
        _board.DepositStack.Deposit(firstCard);
        Log($"!! Partie commence. Première carte sur table: {firstCard} !!");
        CardColor currentColor = firstCard.Color;

        bool finished = false;
        int pendingDraw = 0; // nombre de cartes accumulées à piocher en cas de "2" 

        while (!finished)
        {
            var player = _board.Players[_currentIndex];
            await Task.Delay(700);
            Card topCard = _board.DepositStack.Top;

            // gestion de l'attaque des cartes de "2"
            if (pendingDraw > 0)
            {
                var counterCard = player.Hand.FirstOrDefault(c => c.Value.Equals(CardValue.Deux));
                if (!counterCard.Equals(default(Card)))
                {
                    // le joueur contre avec une autre carte "2"
                    player.RemoveCard(counterCard);
                    _board.DepositStack.Deposit(counterCard);
                    Log($"{player} contre avec {counterCard} !");
                    pendingDraw += 2;
                    currentColor = counterCard.Color;
                    _forceColorFromValet = false;
                }
                else
                {
                    // le joueur n'a pas d'une carte "2" pour contrer donc il pioche les cartes accumulées
                    Log($"--> {player} pioche {pendingDraw} cartes !");
                    for (int i = 0; i < pendingDraw; i++)
                    {
                        if (_board.DrawStack.IsEmpty)
                        {
                            Log(" --> Pile vide, mélange du dépôt...");
                            var newCards = _board.DepositStack.TakeAllExceptTop();
                            if (!newCards.Any())
                            {
                                Log(" --> Aucune carte disponible.");
                                finished = true; break;
                            }
                            newCards = newCards.OrderBy(c => _rnd.Next()).ToList();
                            _board.DrawStack = new DrawStack(newCards);
                        }
                        if (finished) break;
                        if (_board.DrawStack.IsEmpty) break;

                        var drawn = _board.DrawStack.Draw();
                        player.AddCard(drawn);
                    }
                    pendingDraw = 0;
                    _forceColorFromValet = false;
                }
                if (finished) { ComputeScores(null); break; }
                AdvanceTurn();
                continue;
            }

            // le joueur choisit une carte pour jouer via sa stratégie
            var playable = player.Strategy?.ChooseCard(
                player,
                topCard,
                _board.Players,
                _forceColorFromValet ? currentColor : (CardColor?)null
            ) ?? default;

            bool cardPlayedSuccessfully = false;

            if (!playable.Equals(default(Card)))
            {
                // cas interdit : le joueur ne peut pas jouer une carte Valet sur une carte "2"
                if (topCard.Value.Equals(CardValue.Deux) && playable.Value.Equals(CardValue.Valet))
                {
                    playable = default;
                }
                else
                {
                    // vérifier si la carte est compatible la couleur demandée
                    bool isCompatible;
                    if (_forceColorFromValet)
                    {
                        isCompatible = playable.Color.Equals(currentColor) || playable.Value.Equals(CardValue.Valet);
                        if (!isCompatible) Log($"!! {player} doit jouer la couleur {currentColor} ou un Valet !!");
                    }
                    else
                    {
                        isCompatible = playable.Value.Equals(CardValue.Valet) ||
                                       playable.Color.Equals(currentColor) ||
                                       playable.Value.Equals(topCard.Value);
                    }

                    if (!isCompatible)
                        playable = default;
                }
                
                // jouer une carte valide
                if (!playable.Equals(default(Card)))
                {
                    
                    player.RemoveCard(playable);
                    _board.DepositStack.Deposit(playable);
                    Log($"{player} joue {playable} (reste {player.Hand.Count} cartes)");
                    Log($" --> Dernière carte sur la pile de dépot est: {_board.DepositStack.Top}");
                    cardPlayedSuccessfully = true;
                    _forceColorFromValet = false;

                    // les spécifications des cartes "2", As, Dix et Valet
                    if (playable.Value.Equals(CardValue.Deux))
                    {
                        pendingDraw = 2;
                        Log($" --> {player} lance une attaque de 2 !");
                        currentColor = playable.Color;
                    }
                    else if (playable.Value.Equals(CardValue.As))
                    {
                        Log("️ --> Prochain joueur saute son tour !");
                        currentColor = playable.Color;
                        AdvanceTurn();
                    }
                    else if (playable.Value.Equals(CardValue.Dix))
                    {
                        _direction *= -1;
                        Log(" --> Sens du jeu inversé !");
                        currentColor = playable.Color;
                    }
                    else if (playable.Value.Equals(CardValue.Valet))
                    {
                        // le joueur choisit une couleur en se basant sur le nombre de ses cartes qui ont la même couleur
                        var couleursDisponibles = CardColor.All.ToList();
                        var meilleureCouleur = couleursDisponibles
                            .Select(c => new { Color = c, Count = player.Hand.Count(card => card.Color.Equals(c)) })
                            .OrderByDescending(x => x.Count)
                            .FirstOrDefault();

                        CardColor chosenColor = (meilleureCouleur?.Count > 0)
                            ? meilleureCouleur.Color
                            : couleursDisponibles[_rnd.Next(couleursDisponibles.Count)];

                        currentColor = chosenColor;
                        Log($" --> {player} a changé la couleur en {currentColor}");
                        _forceColorFromValet = true;
                    }
                    else
                    {
                        currentColor = playable.Color;
                    }

                    // Vérification fin de partie
                    if (player.Hand.Count == 1)
                    {
                        Log($"!! Attention ! {player} n’a plus qu’une seule carte !!");
                    }
                    if (player.Hand.Count == 0)
                    {
                        Log($" Félicitations, {player} a gagné !!");
                        finished = true; ComputeScores(player); break;
                    }
                }
            }

            // le joueur pioche une carte 
            if (!cardPlayedSuccessfully)
            {
                if (_board.DrawStack.IsEmpty)
                {
                    Log(" --> Pile vide, mélange du dépôt...");
                    var newCards = _board.DepositStack.TakeAllExceptTop();
                    if (!newCards.Any())
                    {
                        Log(" --> Plus aucune carte disponible.");
                        finished = true; ComputeScores(null); break;
                    }
                    newCards = newCards.OrderBy(c => _rnd.Next()).ToList();
                    _board.DrawStack = new DrawStack(newCards);
                }

                if (!_board.DrawStack.IsEmpty)
                {
                    var drawn = _board.DrawStack.Draw();
                    player.AddCard(drawn);
                    Log($"{player} pioche une carte (reste {player.Hand.Count} cartes)");
                }

                
            }

            if (!finished)
            {
                AdvanceTurn();
            }
        }
    }

    // Méthode pour compter les points de chaque joueur
    private void ComputeScores(Player winner)
    {
        Scores.Clear(); LostCards.Clear();
        if (winner == null)
        {
            foreach (var player in _board.Players)
            {
                int total = player.Hand.Sum(c => c.Value.Points);
                Scores[player] = total; LostCards[player] = new List<Card>(player.Hand);
            }
        }
        else
        {
            foreach (var player in _board.Players)
            {
                if (player == winner)
                {
                    Scores[player] = 0; LostCards[player] = new List<Card>();
                }
                else
                {
                    int total = player.Hand.Sum(c => c.Value.Points);
                    Scores[player] = total; LostCards[player] = new List<Card>(player.Hand);
                }
            }
        }
    }

    // Méthode pour passer au joueur suivant
    private void AdvanceTurn()
    {
        _currentIndex = (_currentIndex + _direction + _board.Players.Count) % _board.Players.Count;
    }

    private void Log(string msg) => OnLog?.Invoke(msg);
}
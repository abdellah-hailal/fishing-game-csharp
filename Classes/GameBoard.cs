namespace FishingCardGame;

public class GameBoard
{
    // Liste de tous les joueurs présents dans la partie
    public List<Player> Players { get; }
    // La pioche (pile de cartes restantes)
    public DrawStack DrawStack { get; set; }
    // Le dépot (pile où les joueurs posent les cartes jouées)
    public DepositStack DepositStack { get; }
    public GameBoard(List<Player> players, DrawStack drawStack, DepositStack depositStack)
    {
        Players = players; 
        DrawStack = drawStack; 
        DepositStack = depositStack;
    }
}
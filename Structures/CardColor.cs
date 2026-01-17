namespace FishingCardGame;

public struct CardColor
{
    public string Name { get; }
    private CardColor(string name) { Name = name; }
    public override string ToString() => Name;

    public static readonly CardColor Trefle = new CardColor("Trèfle");
    public static readonly CardColor Carreau = new CardColor("Carreau");
    public static readonly CardColor Coeur = new CardColor("Cœur");
    public static readonly CardColor Pique = new CardColor("Pique");

    public static readonly CardColor[] All = { Trefle, Carreau, Coeur, Pique };

}
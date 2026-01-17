namespace FishingCardGame;

public struct CardValue
{
    public string Name { get; }
    public int Points { get; }

    private CardValue(string name, int points) { Name = name; Points = points; }
    public override string ToString() => Name;

    public static readonly CardValue As = new CardValue("As", 11);
    public static readonly CardValue Deux = new CardValue("2", 2);
    public static readonly CardValue Trois = new CardValue("3", 3);
    public static readonly CardValue Quatre = new CardValue("4", 4);
    public static readonly CardValue Cinq = new CardValue("5", 5);
    public static readonly CardValue Six = new CardValue("6", 6);
    public static readonly CardValue Sept = new CardValue("7", 7);
    public static readonly CardValue Huit = new CardValue("8", 8);
    public static readonly CardValue Neuf = new CardValue("9", 9);
    public static readonly CardValue Dix = new CardValue("10", 10);
    public static readonly CardValue Valet = new CardValue("Valet", 2);
    public static readonly CardValue Dame = new CardValue("Dame", 2);
    public static readonly CardValue Roi = new CardValue("Roi", 2);

    public static readonly CardValue[] All = 
    {
        As, Deux, Trois, Quatre, Cinq, Six, Sept, Huit, Neuf, Dix, Valet, Dame, Roi
    };

}
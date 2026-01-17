namespace FishingCardGame;

public struct Card
{
    public CardValue Value { get; }
    public CardColor Color { get; }
    public Card(CardValue value, CardColor color) { Value = value; Color = color; }
    public override string ToString() => $"{Value} de {Color}";
}
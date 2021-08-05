namespace GameStore.Models.Games
{
    public class PurchaseFormModel
    {
        public string CardNumber { get; init; }

        public string ExpirationDate { get; init; }

        public string CCV { get; init; }

        public int GameId { get; init; }
    }
}

namespace GameStore.Models.Clients
{
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public class ClientProfileViewModel
    {
        public int? RelationId { get; init; }

        public int ProfileId { get; init; }

        public int ClientId { get; init; }

        public bool AreFriends { get; init; }

        public string Username { get; init; }

        public string Description { get; init; }

        public string ProfilePictureUrl { get; init; }

        public int ReviewsCount { get; init; }

        public double AvarageRating { get; init; }

        public bool AreFriendsPrivate { get; init; }

        public bool AreGamesPrivate { get; init; }

        public IEnumerable<GameHoverViewModel> Games { get; init; } = new List<GameHoverViewModel>();

        public IEnumerable<FriendsViewModel> Friends { get; init; } = new List<FriendsViewModel>();

        public IEnumerable<ReviewViewModel> Reviews { get; init; } = new List<ReviewViewModel>();
    }
}

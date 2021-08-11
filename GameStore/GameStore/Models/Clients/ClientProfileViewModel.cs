namespace GameStore.Models.Clients
{
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public class ClientProfileViewModel
    {
        public int Id { get; init; }

        public string Username { get; init; }

        public string Description { get; init; }

        public string ProfilePictureUrl { get; init; }

        public bool AreFriendsPrivate { get; init; }

        public bool AreGamesPrivate { get; init; }

        public IEnumerable<GameHoverViewModel> Games { get; init; } = new List<GameHoverViewModel>();

        public IEnumerable<ClientRelationship> Friends { get; init; } = new List<ClientRelationship>();

        public IEnumerable<ReviewViewModel> Reviews { get; init; } = new List<ReviewViewModel>();
    }
}

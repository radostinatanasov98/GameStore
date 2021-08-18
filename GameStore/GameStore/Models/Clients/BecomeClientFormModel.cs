namespace GameStore.Models.Clients
{
    using System.ComponentModel.DataAnnotations;
    using static Data.DataConstants;

    public class BecomeClientFormModel
    {
        [Required]
        [MaxLength(Client.NameMaxLength)]
        [Display(Name = "Client Name")]
        public string Name { get; init; }

        [Display(Name = "Profile picture")]
        [RegularExpression(Shared.ImageUrlRegularExpression,
            ErrorMessage = Shared.InvalidUrlErrorMessage)]
        public string ProfilePictureUrl { get; init; }

        public string Description { get; init; }

        public bool AreFriendsPrivate { get; init; }

        public bool AreGamesPrivate { get; init; }
    }
}

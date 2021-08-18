namespace GameStore.Models.Publishers
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class BecomePublisherFormModel
    {
        [Required]
        [MaxLength(Publisher.NameMaxLength)]
        [Display(Name = "Company Name")]
        public string Name { get; init; }

        [Required(ErrorMessage = Shared.InvalidUrlErrorMessage)]
        [RegularExpression(Shared.ImageUrlRegularExpression)]
        public string PictureUrl { get; init; }
    }
}

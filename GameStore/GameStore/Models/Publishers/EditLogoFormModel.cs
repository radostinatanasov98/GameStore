namespace GameStore.Models.Publishers
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Shared;

    public class EditLogoFormModel
    {
        [Required]
        [RegularExpression(ImageUrlRegularExpression, 
            ErrorMessage = InvalidUrlErrorMessage)]
        public string Url { get; init; }

        public int Id { get; set; }
    }
}

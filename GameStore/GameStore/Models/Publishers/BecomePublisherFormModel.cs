namespace GameStore.Models.Publishers
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Publisher;

    public class BecomePublisherFormModel
    {
        [Required]
        [MaxLength(NameMaxLength)]
        [Display(Name = "Company Name")]
        public string Name { get; init; }
    }
}

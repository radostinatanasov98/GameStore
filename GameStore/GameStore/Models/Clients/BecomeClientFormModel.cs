namespace GameStore.Models.Clients
{
    using System.ComponentModel.DataAnnotations;
    using static Data.DataConstants.Client;

    public class BecomeClientFormModel
    {
        [Required]
        [MaxLength(NameMaxLength)]
        [Display(Name = "Client Name")]
        public string Name { get; init; }
    }
}

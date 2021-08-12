namespace GameStore.Models.Clients
{
    public class EditProfileFormModel
    {
        public int ProfileId { get; set; }

        public string PictureUrl { get; init; }

        public string Description { get; init; }
    }
}

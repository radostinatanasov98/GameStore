namespace GameStore.Services.Users
{
    public interface IUserService
    {
        public bool IsUserPublisher(string userId);

        public bool IsUserClient(string userId);
    }
}

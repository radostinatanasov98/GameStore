namespace GameStore.Services.Users
{
    public interface IUserService
    {
        public bool IsUserClient(string userId);

        public bool IsUserPublisher(string userId);
    }
}

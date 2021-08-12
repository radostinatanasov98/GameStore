namespace GameStore.Services.Users
{
    using GameStore.Data;
    using System.Linq;


    public class UserService : IUserService
    {
        private readonly GameStoreDbContext data;

        public UserService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public bool IsUserPublisher(string userId)
            => this.data.Publishers.Any(p => p.UserId == userId);

        public bool IsUserClient(string userId)
            => this.data.Clients.Any(p => p.UserId == userId);
    }
}

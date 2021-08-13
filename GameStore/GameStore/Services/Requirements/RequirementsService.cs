namespace GameStore.Services.Requirements
{
    using GameStore.Data;
    using GameStore.Data.Models;

    public class RequirementsService : IRequirementsService
    {
        private readonly GameStoreDbContext data;

        public RequirementsService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public Requirements CreateRequirements(string CPU, string GPU, int RAM, int VRAM, int storage, string OS)
            => new Requirements
            {
                CPU = CPU,
                GPU = GPU,
                RAM = RAM,
                VRAM = VRAM,
                Storage = storage,
                OS = OS
            };
    }
}

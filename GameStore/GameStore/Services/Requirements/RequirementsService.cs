namespace GameStore.Services.Requirements
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using System.Collections.Generic;
    using System.Linq;

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

        public Requirements GetRequirementsById(int id)
            => this.data
            .Requirements
            .FirstOrDefault(r => r.Id == id);
    }
}

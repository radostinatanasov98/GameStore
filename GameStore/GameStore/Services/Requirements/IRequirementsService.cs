namespace GameStore.Services.Requirements
{
    using GameStore.Data.Models;

    public interface IRequirementsService
    {
        public Requirements CreateRequirements(string CPU, string GPU, int RAM, int VRAM, int storage, string OS);

        public Requirements GetRequirementsById(int id);
    }
}

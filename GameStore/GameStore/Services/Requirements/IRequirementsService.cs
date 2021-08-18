namespace GameStore.Services.Requirements
{
    using GameStore.Data.Models;

    public interface IRequirementsService
    {
        public Requirements CreateRequirements(string CPU, string GPU, string RAM, string VRAM, string storage, string OS);

        public Requirements GetRequirementsById(int id);
    }
}

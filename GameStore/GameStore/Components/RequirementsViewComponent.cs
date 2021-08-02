namespace GameStore.Components
{
    using GameStore.Data;
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    [ViewComponent(Name = "Requirements")]
    public class RequirementsViewComponent : ViewComponent
    {
        private readonly GameStoreDbContext data;

        public RequirementsViewComponent(GameStoreDbContext data)
        {
            this.data = data;
        }

        public IViewComponentResult Invoke(int minId, int recId)
        {
            var minimumRequirements = this.data
                .Requirements
                .Where(r => r.Id == minId)
                .Select(r => new RequirementsViewModel
                {
                    CPU = r.CPU,
                    GPU = r.GPU,
                    VRAM = r.VRAM,
                    RAM = r.RAM,
                    Storage = r.Storage,
                    OS = r.OS
                })
                .FirstOrDefault();

            var recommendedRequirements = this.data
                .Requirements
                .Where(r => r.Id == recId)
                .Select(r => new RequirementsViewModel
                {
                    CPU = r.CPU,
                    GPU = r.GPU,
                    VRAM = r.VRAM,
                    RAM = r.RAM,
                    Storage = r.Storage,
                    OS = r.OS
                })
                .FirstOrDefault();

            var requirements = new RequirementsViewModel[2]
            {
                minimumRequirements,
                recommendedRequirements
            };

            return View(requirements);
        }
    }
}

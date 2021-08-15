namespace GameStore.Tests.Services
{
    using GameStore.Data.Models;
    using GameStore.Services.Requirements;
    using GameStore.Tests.Mocks;
    using Xunit;

    public class RequirementsServiceTest
    {
        private const string cpu = "testCpu";
        private const string gpu = "testCpu";
        private const string os = "testCpu";
        private const int ram = 5;
        private const int vram = 5;
        private const int storage = 5;
        [Fact]
        public void CreateRequirementsCreatesCorrectEntity()
        {
            // Arrange
            var requirementsService = new RequirementsService(null);

            // Act
            var result = requirementsService.CreateRequirements(cpu,
                gpu,
                ram,
                vram,
                storage,
                os);

            // Assert
            Assert.Equal(cpu, result.CPU);
            Assert.Equal(gpu, result.GPU);
            Assert.Equal(ram, result.RAM);
            Assert.Equal(vram, result.VRAM);
            Assert.Equal(storage, result.Storage);
            Assert.Equal(os, result.OS);
        }

        [Fact]
        public void GetRequirementsByIdShouldReturnCorrectEntity()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Requirements
                .Add(new Requirements
                {
                    Id = 1,
                    CPU = cpu,
                    GPU = gpu,
                    OS = os,
                    RAM = ram,
                    VRAM = vram,
                    Storage = storage
                });

            data.SaveChanges();

            var requirementsService = new RequirementsService(data);

            // Act
            var result = requirementsService.GetRequirementsById(1);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(cpu, result.CPU);
            Assert.Equal(gpu, result.GPU);
            Assert.Equal(ram, result.RAM);
            Assert.Equal(vram, result.VRAM);
            Assert.Equal(storage, result.Storage);
            Assert.Equal(os, result.OS);
        }
    }
}

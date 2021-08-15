namespace GameStore.Tests.Mocks
{
    using GameStore.Data;
    using Microsoft.EntityFrameworkCore;
    using System;

    public static class DatabaseMock
    {
        public static GameStoreDbContext Instance
        {
            get
            {
                var dbContextOptions = new DbContextOptionsBuilder<GameStoreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                return new GameStoreDbContext(dbContextOptions);
            }
        }
    }
}

namespace GameStore.Tests.Mocks
{
    using GameStore.Services.Clients;
    using Moq;

    public static class ClientServieMock
    {
        public static IClientService Instance
        {
            get
            {
                var clientServiceMock = new Mock<IClientService>();

                return null;
            }
        }
    }
}

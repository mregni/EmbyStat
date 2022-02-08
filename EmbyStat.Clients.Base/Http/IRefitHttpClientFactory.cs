namespace EmbyStat.Clients.Base.Http
{
    public interface IRefitHttpClientFactory<T>
    {
        T CreateClient(string baseAddressKey);
    }
}

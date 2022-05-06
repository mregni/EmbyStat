namespace EmbyStat.Clients.Base.Http;

public interface IRefitHttpClientFactory<out T>
{
    T CreateClient(string baseAddressKey);
}
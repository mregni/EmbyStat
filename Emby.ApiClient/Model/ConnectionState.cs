namespace Emby.ApiClient.Model
{
    public enum ConnectionState
    {
        Unavailable = 1,
        ServerSignIn = 2,
        SignedIn = 3,
        ServerSelection = 4,
        ConnectSignIn = 5,
        OfflineSignIn = 6,
        OfflineSignedIn = 7
    }
}
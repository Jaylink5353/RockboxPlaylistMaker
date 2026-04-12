using System;
using System.IO;
using System.Threading.Tasks;

using SpotifyAPI.Web;

namespace SpotiFechLib
{
    internal class SpotiLib
    {
        readonly static private string spotClientId = "9fc851105c9e4d94a81f5e246276001a";
        readonly static private string spotClientSecret = "2cc79a4c68fe48d099b4d5ebe78ed3f3";
        static string? AccessToken;
        static private async Task GetAccToken()
        {
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(spotClientId, spotClientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

            AccessToken = spotify.ToString();
        }


    }
}

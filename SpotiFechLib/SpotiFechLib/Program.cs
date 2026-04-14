using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using SpotifyAPI.Web;
using System.ComponentModel.Design;

namespace SpotiFechLib
{
    internal class SpotiLib
    {
        private record TrackInfo(
            int position,
            string title,
            string album,
            string artists,
            bool IsLocal
        );
        readonly static private string spotClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new Exception("ID NULL");
        private static SpotifyClient? spotify;
        private List<TrackInfo> tracks = new List<TrackInfo>();
        FullPlaylist? playlist;
        static async Task Main(string[] args)
        {
            await Auth();
            var playlistId = await getPlaylistID(args);

            var app = new SpotiLib();
            await app.fetchMetadata(playlistId);
            await app.fetchSongs(playlistId);
            app.displayResults();
        }

        static async Task Auth()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            var tcs = new TaskCompletionSource<string>();
            var http = new System.Net.HttpListener();

            http.Prefixes.Add("http://127.0.0.1:5000/callback/");
            http.Start();

            _ = Task.Run(async () =>
            {
                var context = await http.GetContextAsync();
                var code = context.Request.QueryString["code"] ?? "";
                var response = context.Response;
                string responseText = "<html><body>Authentication Successful</body></html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                response.OutputStream.Close();
                http.Stop();
                tcs.SetResult(code);
            });

            var loginRequest = new LoginRequest(
                new Uri("http://127.0.0.1:5000/callback/"),
                spotClientId,
                LoginRequest.ResponseType.Code
            )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };

            Console.WriteLine("Auth with Spotify...");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = loginRequest.ToUri().ToString(), UseShellExecute = true });

            var code = await tcs.Task;

            var tokenResponse = await new OAuthClient().RequestToken(
                new PKCETokenRequest(spotClientId, code, new Uri("http://127.0.0.1:5000/callback/"), verifier)
            );

            spotify = new SpotifyClient(tokenResponse.AccessToken);



        }

        static Task<string> getPlaylistID(string[] input)
        {
            string playlistId = input.Length > 0
                ? parseURL(input[0])
                : PromptForPlaylistId();

            return Task.FromResult(playlistId);
        }

        private static string parseURL(string input)
        {
            if (input.Contains("spotify.com/playlist/"))
            {
                var uri = new Uri(input.Split('?')[0]);
                var segs = uri.AbsolutePath.Split('/');
                return segs[^1];
            }
            return input.Trim();
        }

        private async Task fetchMetadata(string id)
        {
            if (spotify == null) throw new InvalidOperationException("Not authenticated");

            try
            {
                playlist = await spotify.Playlists.Get(id);
            }
            catch (APIException ex)
            {
                Console.Error.WriteLine($"Could not fetch playlist: {ex.Message}");
                throw;
            }
        }
        private static void prompt()
        {
            return;
        }

        private async Task fetchSongs(string id)
        {
            if (spotify == null) throw new InvalidOperationException("Not authenticated");

            Paging<PlaylistTrack<IPlayableItem>>? firstPage = null;
            
            try
            {
                firstPage = await spotify.Playlists.GetPlaylistItems(id, new PlaylistGetItemsRequest
                {
                    Market = "from_token"
                });
                //firstPage = await spotify.Playlists.GetPlaylistItems(id);
            }
            catch (APIException ex) { Console.WriteLine(ex); };
            
            if (firstPage == null || firstPage.Items == null || firstPage.Items.Count == 0)
            {
                Console.WriteLine("Empty Playlist!");
                return;
            }

            await foreach (var result in spotify.Paginate(firstPage))
            {
                var playable = result.Track ?? result.Item;

                if (playable is FullTrack track)
                {
                    tracks.Add(new TrackInfo(
                        position: tracks.Count + 1,
                        title: track.Name,
                        artists: string.Join(", ", track.Artists.ConvertAll(a => a.Name)),
                        album: track.Album.Name,
                        IsLocal: track.IsLocal
                    ));
                }
                else if (playable is FullEpisode)
                {
                    Console.WriteLine("This playlist contains a podcast. Don't use these lol");
                    return;
                }
            }
        }

        private void displayResults()
        {
            Console.WriteLine($"{"#",-5} {"Song",-45} {"Artist",-30} {"Album",-35}");
            Console.WriteLine(new string('-', 115));

            foreach (var t in tracks)
            {
                string localTag = t.IsLocal ? " [local]" : "";
                Console.WriteLine(
                    $"{t.position,-5} " +
                    $"{Truncate(t.title + localTag, 43),-45} " +
                    $"{Truncate(t.artists, 28),-30} " +
                    $"{Truncate(t.album, 33),-35}"
                );
            }

            Console.WriteLine();
            Console.WriteLine($"Tracks found: {tracks.Count}");
        }
        private static string PromptForPlaylistId()
        {
            Console.Write("Enter Spotify Playlist URL or ID: ");
            return parseURL(Console.ReadLine()?.Trim() ?? string.Empty);
        }

        private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..(maxLength - 1)] + "…";
    }
}

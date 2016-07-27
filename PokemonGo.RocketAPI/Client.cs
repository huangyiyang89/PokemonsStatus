#region

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Login;
using static PokemonGo.RocketAPI.GeneratedCode.Response.Types;

#endregion

namespace PokemonGo.RocketAPI
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private string _apiUrl;
        private AuthType _authType = AuthType.Google;
        private Request.Types.UnknownAuth _unknownAuth;
        Random rand = null;

        public Client(ISettings settings)
        {
           Settings = settings;
           SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);
           

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            };
            _httpClient = new HttpClient(new RetryHandler(handler));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Niantic App");
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/x-www-form-urlencoded");
        }

        

        public ISettings Settings { get; }
        public string AccessToken { get; set; }

        public double CurrentLat { get; private set; }
        public double CurrentLng { get; private set; }
        public double CurrentAltitude { get; private set; }



        public async Task DoGoogleLogin()
        {
            _authType = AuthType.Google;

            string googleRefreshToken = string.Empty;
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Configs\\GoogleAuth.ini"))
            {
                googleRefreshToken = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Configs\\GoogleAuth.ini");
            }

            GoogleLogin.TokenResponseModel tokenResponse;
            if (googleRefreshToken != string.Empty)
            {
                tokenResponse = await GoogleLogin.GetAccessToken(googleRefreshToken);
                AccessToken = tokenResponse?.id_token;
            }

            if (AccessToken == null)
            {
                var deviceCode = await GoogleLogin.GetDeviceCode();
                tokenResponse = await GoogleLogin.GetAccessToken(deviceCode);
                googleRefreshToken = tokenResponse?.refresh_token;
                try
                {
                    File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\GoogleAuth.ini", googleRefreshToken);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                AccessToken = tokenResponse?.id_token;
            }
        }

        public async Task DoPtcLogin(string username, string password)
        {
            AccessToken = await PtcLogin.GetAccessToken(username, password);
            _authType = AuthType.Ptc;
        }

        public async Task<GetInventoryResponse> GetInventory()
        {
            var inventoryRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                RequestType.GET_INVENTORY);
            return
                await
                    _httpClient.PostProtoPayload<Request, GetInventoryResponse>($"https://{_apiUrl}/rpc",
                        inventoryRequest);
        }

        

        private void CalcNoisedCoordinates(double lat, double lng, out double latNoise, out double lngNoise)
        {
            double mean = 0.0;// just for fun
            double stdDev = 2.09513120352; //-> so 50% of the noised coordinates will have a maximal distance of 4 m to orginal ones

            if (rand == null)
            {
                rand = new Random();
            }
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double u3 = rand.NextDouble();
            double u4 = rand.NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;
            double randStdNormal2 = Math.Sqrt(-2.0 * Math.Log(u3)) * Math.Sin(2.0 * Math.PI * u4);
            double randNormal2 = mean + stdDev * randStdNormal2;

            latNoise = lat + randNormal / 100000.0;
            lngNoise = lng + randNormal2 / 100000.0;
        }

        private void SetCoordinates(double lat, double lng, double altitude)
        {
            if (double.IsNaN(lat) || double.IsNaN(lng)) return;

            double latNoised = 0.0;
            double lngNoised = 0.0;
            CalcNoisedCoordinates(lat, lng, out latNoised, out lngNoised);
            CurrentLat = latNoised;
            CurrentLng = lngNoised;
            CurrentAltitude = altitude;
            
        }


        public async Task SetServer()
        {
            var serverRequest = RequestBuilder.GetInitialRequest(AccessToken, _authType, CurrentLat, CurrentLng,
                CurrentAltitude,
                RequestType.GET_PLAYER, RequestType.GET_HATCHED_OBJECTS, RequestType.GET_INVENTORY,
                RequestType.CHECK_AWARDED_BADGES, RequestType.DOWNLOAD_SETTINGS);
            var serverResponse = await _httpClient.PostProto(Resources.RpcUrl, serverRequest);

            if (serverResponse.Auth == null)
                throw new AccessTokenExpiredException();

            _unknownAuth = new Request.Types.UnknownAuth
            {
                Unknown71 = serverResponse.Auth.Unknown71,
                Timestamp = serverResponse.Auth.Timestamp,
                Unknown73 = serverResponse.Auth.Unknown73
            };

            _apiUrl = serverResponse.ApiUrl;
        }

       
    }
}

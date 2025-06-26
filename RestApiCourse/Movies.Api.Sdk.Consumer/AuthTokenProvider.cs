using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Threading;

namespace Movies.Api.Sdk.Consumer
{
    public class AuthTokenProvider
    {
        private readonly HttpClient _httpClient;
        private readonly object _lock = new();
        private Lazy<Task<string>> _tokenLazy;

        public AuthTokenProvider(HttpClient httpClien)
        {
            _httpClient = httpClien;
            _tokenLazy = CreateTokenLazy();
        }

        public async Task<string> GetTokenAsync()
        {
            var token = await _tokenLazy.Value;

            if (IsTokenExpired(token))
            {
                lock (_lock)
                {
                    // Only one thread will recreate Lazy
                    if (IsTokenExpired(_tokenLazy.Value.Result))
                    {
                        _tokenLazy = CreateTokenLazy();
                    }
                }

                token = await _tokenLazy.Value;
            }

            return token;
        }

        private Lazy<Task<string>> CreateTokenLazy()
        {
            return new Lazy<Task<string>>(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
                {
                    userId = "450181d1-faf2-4238-ac5a-e57216dd6a62",
                    email = "tes@email.com",
                    customClaims = new Dictionary<string, object>
                    {
                        { "admin", true },
                        { "trusted_member", true }
                    }
                });

                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }, isThreadSafe: true);
        }

        private static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return true;

            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var exp = jwt.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;

                if (exp == null) return true;

                var expiry = UnixTimeStampToDateTime(long.Parse(exp));
                return expiry <= DateTime.UtcNow.AddSeconds(30);
            }
            catch
            {
                return true;
            }
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        }
    }
}

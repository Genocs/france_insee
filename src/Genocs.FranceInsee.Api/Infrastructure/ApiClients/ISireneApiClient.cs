using Refit;

namespace Genocs.FranceInsee.Infrastructure.ApiClients
{
    public interface ISireneApiClient
    {
        [Get("/entreprises/sirene/V3/siret/{id}")]
        Task<object> GetSiretAsync([AliasAs("id")] string id, [Header("Authorization")] string authorization);

        [Get("/entreprises/sirene/V3/siren/{id}")]
        Task<object> GetSirenAsync([AliasAs("id")] string id, [Header("Authorization")] string authorization);

        [Post("/token")]
        Task<AuthResponse> PostTokenAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);
    }

    public class AuthResponse
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}





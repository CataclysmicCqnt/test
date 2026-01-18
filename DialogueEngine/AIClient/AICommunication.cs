using DTOModel;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace AIClient
{
    /// <summary>
    /// Klasa odpowiedzialna za komunikację z lokalnym serwerem AI.
    /// Umożliwia uruchamianie serwera, sprawdzanie jego stanu oraz wysyłanie żądań POST
    /// do generowania odpowiedzi NPC i ładowania scen.
    /// </summary>
    public class AICommunication : IDisposable
    {
        /// <summary>
        /// Klient HTTP używany do komunikacji z serwerem AI.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Konstruktor klasy. Uruchamia serwer Pythona i inicjalizuje klienta HTTP.
        /// </summary>
        public AICommunication()
        {
            RunPythonServer(); // TODO

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");
        }

        /// <summary>
        /// Metoda odpowiedzialna za uruchamianie lokalnego serwera Pythona.
        /// </summary>
        private void RunPythonServer()
        {

        }

        /// <summary>
        /// Sprawdza, czy serwer AI jest dostępny poprzez wykonanie żądania GET na endpoint /health.
        /// </summary>
        /// <returns>True jeśli serwer działa, false w przeciwnym przypadku.</returns>
        private async Task<bool> IsServerRunningAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("/health");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Wysyła standardowe żądanie POST z JSON-em do określonego endpointa.
        /// </summary>
        /// <param name="json">Dane w formacie JSON do wysłania.</param>
        /// <param name="endpoint">Endpoint serwera (relatywny do BaseAddress).</param>
        /// <returns>Odpowiedź serwera w formacie string lub null, jeśli serwer nie działa lub wystąpił błąd.</returns>
        private async Task<string> StandardPostAsync(string json, string endpoint)
        {
            if (await IsServerRunningAsync() == false) return null;

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                return null;
            }
        }

        /// <summary>
        /// Generuje odpowiedź NPC na podstawie przesłanego obiektu <see cref="NPCRequestDTO"/>.
        /// </summary>
        /// <param name="requestDTO">Obiekt DTO z danymi zapytania do NPC.</param>
        /// <returns>Odpowiedź NPC w formacie JSON lub null jeśli serwer nie działa.</returns>
        public async Task<string> GenerateNPCResponseAsync(NPCRequestDTO requestDTO)
        {
            string json = JsonConvert.SerializeObject(requestDTO);
            var response = await StandardPostAsync(json, "/npc/chat");
            var npcResponse = JsonConvert.DeserializeObject<NPCResponseDTO>(response);
            return JsonConvert.SerializeObject(npcResponse);
        }

        /// <summary>
        /// Generuje nową scenę na serwerze AI na podstawie przesłanego obiektu <see cref="SceneDTO"/>.
        /// </summary>
        /// <param name="sceneDTO">Obiekt DTO z danymi sceny.</param>
        /// <returns>Odpowiedź serwera w formacie JSON lub null jeśli serwer nie działa.</returns>
        public async Task<string> GenerateNewSceneAsync(SceneDTO sceneDTO)
        {
            string json = JsonConvert.SerializeObject(sceneDTO);
            return await StandardPostAsync(json, "/npc/load");
        }

        public async Task<string> GenerateNpcVerdictAsync(string npcName)
        {
            accusedName = npcName;

            string json = JsonConvert.SerializeObject(accusedName);
            return await StandardPostAsync(json, "/npc/verdict");

        }


        /// <summary>
        /// Metoda wywoływana przy zwalnianiu zasobów.
        /// Ma zakończyć serwer Pythona i posprzątać zasoby.
        /// </summary>
        public void Dispose()
        {
            // kill python AI server
        }
    }
}

using DTOModel;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace AIClient
{
    public class AICommunication : IDisposable
    {
        private readonly HttpClient _httpClient;

        public AICommunication()
        {
            RunPythonServer(); // TODO

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri ("http://127.0.0.1:8000");
        }

        private void RunPythonServer()
        {

        }

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

        public async Task<string> GenerateNPCResponseAsync(NPCRequestDTO requestDTO)
        {
            string json = JsonConvert.SerializeObject(requestDTO);
            var response = await StandardPostAsync(json, "/npc/chat");
            var npcResponse = JsonConvert.DeserializeObject<NPCResponseDTO>(response);
            return JsonConvert.SerializeObject(npcResponse);
        }

        public async Task<string> GenerateNewSceneAsync(SceneDTO sceneDTO)
        {
            string json = JsonConvert.SerializeObject(sceneDTO);
            return await StandardPostAsync(json, "/npc/load");
        }

        public void Dispose()
        {
            // kill python AI server
        }
    }
}

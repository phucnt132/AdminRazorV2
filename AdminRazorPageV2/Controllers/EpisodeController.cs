using DTOs.EpisodeDTOs.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using DTOs.ServiceResponseDTOs;

namespace AdminRazorPageV2.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string ManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";

        public EpisodeController()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ManagementApiUrl = "http://localhost:44384/api/Movies";
            AuthApiUrl = "http://localhost:44388/api/Auth";
            EpisodeManagementApiUrl = "http://localhost:44384/api/Episodes";
        }
        // GET: All Episode
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(EpisodeManagementApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<EpisodeResponse>> listEpisodes = JsonSerializer.Deserialize<ServiceResponse<List<EpisodeResponse>>>(strData, options);
                IEnumerable<EpisodeResponse> episodeResponses = listEpisodes.Data;
                return View(episodeResponses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Episode by Id
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{EpisodeManagementApiUrl}/id?id={id}");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return View("NoContent");
            }

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<EpisodeResponse> episodeResponse = JsonSerializer.Deserialize<ServiceResponse<EpisodeResponse>>(strData, options);


            if (episodeResponse == null)
            {
                return NotFound();
            }

            return View(episodeResponse.Data);
        }

        // GET: Create Episode
        public async Task<IActionResult> Create()
        {
            return View();
        }
    }
}

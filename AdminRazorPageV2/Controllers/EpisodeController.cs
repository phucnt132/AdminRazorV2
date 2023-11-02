using DTOs.EpisodeDTOs.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using DTOs.ServiceResponseDTOs;
using System.Text;
using DTOs.EpisodeDTOs.RequestDTO;
using AutoMapper;
using AdminRazorPageV2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using DTOs.MovieDTOs.ResponseDTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdminRazorPageV2.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string MovieManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public EpisodeController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            MovieManagementApiUrl = "https://localhost:5003/apigateway/Movies";
            AuthApiUrl = "https://localhost:5003/apigateway/Auth";
            EpisodeManagementApiUrl = "https://localhost:5003/apigateway/Episodes";
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Helper function: Get Session
        public string GetSessionValue(string key)
        {
            var session = _contextAccessor.HttpContext.Session;
            return session.GetString(key);
        }

        // Error
        public async Task<IActionResult> Error()
        {
            return View();
        }


        // GET: All Episode
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(EpisodeManagementApiUrl);
                HttpResponseMessage movieResponse = await _httpClient.GetAsync(MovieManagementApiUrl);
                
                string strData = await response.Content.ReadAsStringAsync();
                string movieStrData = await movieResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<EpisodeResponse>> listEpisodes = JsonSerializer.Deserialize<ServiceResponse<List<EpisodeResponse>>>(strData, options);
                ServiceResponse<List<MovieResponse>> listMovies = JsonSerializer.Deserialize<ServiceResponse<List<MovieResponse>>>(movieStrData, options);
                // Get MovieId of Episode base on MovieId of Movie
                var movieLookup = listMovies.Data.ToDictionary(m => m.MovieId, m => m.MovieName);
                if (listEpisodes != null)
                {
                    foreach (var episode in listEpisodes.Data)
                    {
                        episode.MovieName = movieLookup[episode.MovieId];
                    }
                }
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
            HttpResponseMessage movieResponse = await _httpClient.GetAsync(MovieManagementApiUrl);
            string movieStrData = await movieResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<List<MovieResponse>> listMovies = JsonSerializer.Deserialize<ServiceResponse<List<MovieResponse>>>(movieStrData, options);
            ViewBag.MovieData = listMovies.Data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEpisodeDto episode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var episodeJson = JsonSerializer.Serialize(episode);
                    var content = new StringContent(episodeJson, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                   
                    // Check Authorization
                    if(_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PostAsync($"{EpisodeManagementApiUrl}/Create", content);
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Index");
        }

        // Edit Episode
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{EpisodeManagementApiUrl}/id?id={id}");

            // loaded episode data
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

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EpisodeId,EpisodeName,Description,IsActive,MediaLink")] UpdateEpisodeDto episode)
        {
            UpdateEpisodeDto afterUpdate = new UpdateEpisodeDto();
            ServiceResponse<EpisodeResponse> episodeResponse;
            try
            {
                afterUpdate.EpisodeId = episode.EpisodeId;
                afterUpdate.EpisodeName = episode.EpisodeName;
                afterUpdate.Description = episode.Description;
                afterUpdate.MediaLink = episode.MediaLink;
                afterUpdate.IsActive = episode.IsActive;
                var episodeJson = JsonSerializer.Serialize(afterUpdate);
                var content = new StringContent(episodeJson, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                // Check Authorization
                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }
                HttpResponseMessage response = await _httpClient.PutAsync($"{EpisodeManagementApiUrl}/Update", content);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                episodeResponse = JsonSerializer.Deserialize<ServiceResponse<EpisodeResponse>>(strData, options);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        // Delete Episode
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{EpisodeManagementApiUrl}/id?id={id}");

            // loaded episode data
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

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("EpisodeId")] DeleteEpisodeDto episode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DeleteEpisodeDto afterUpdate = new DeleteEpisodeDto();
                    afterUpdate.EpisodeId = episode.EpisodeId;
                    var episodeJson = JsonSerializer.Serialize(afterUpdate);
                    var content = new StringContent(episodeJson, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PutAsync($"{EpisodeManagementApiUrl}/Delete?id={episode.EpisodeId}", content);
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Index");
        }

    }
}

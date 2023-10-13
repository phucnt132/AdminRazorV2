using AutoMapper;
using CategoryServices.DTOs.ResponseDTO;
using DTOs.EpisodeDTOs.ResponseDTO;
using DTOs.MovieDTOs.ResponseDTO;
using DTOs.ServiceResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AdminRazorPageV2.Controllers
{
    public class MovieController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string ManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string CategoryManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public MovieController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ManagementApiUrl = "http://localhost:44384/api/Movies";
            AuthApiUrl = "http://localhost:44388/api/Auth";
            EpisodeManagementApiUrl = "http://localhost:44384/api/Episodes";
            CategoryManagementApiUrl = "http://localhost:44386/api/Categories";
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Helper function: Get Session
        public string GetSessionValue(String key)
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
                HttpResponseMessage response = await _httpClient.GetAsync(ManagementApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<MovieResponse>> listMovies = JsonSerializer.Deserialize<ServiceResponse<List<MovieResponse>>>(strData, options);
                IEnumerable<MovieResponse> movieResponses = listMovies.Data;
                return View(movieResponses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Movie by Id
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{ManagementApiUrl}/id?id={id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<MovieResponse> movieResponse = JsonSerializer.Deserialize<ServiceResponse<MovieResponse>>(strData, options);

            if (movieResponse == null)
            {
                return NotFound();
            }

            for (int i = 0; i < movieResponse.Data.Categories.Count; i++)
            {
                var category = movieResponse.Data.Categories[i];
                response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}/id?id={int.Parse(category)}");
                strData = await response.Content.ReadAsStringAsync();

                options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<CategoryResponse> categoryResponse = JsonSerializer.Deserialize<ServiceResponse<CategoryResponse>>(strData, options);

                var newCategory = categoryResponse.Data.CategoryName;

                // Gán lại giá trị của category bằng giá trị mới
                movieResponse.Data.Categories[i] = newCategory;
            }

            return View(movieResponse.Data);
        }
    }
}

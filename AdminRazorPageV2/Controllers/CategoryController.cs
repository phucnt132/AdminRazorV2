using DTOs.CategoryDTOs.ResponseDTOs;
using APIS.DTOs.AuthenticationDTOs.ResponseDto;
using AutoMapper;
using DTOs.MovieDTOs.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AdminRazorPageV2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string ManagementApiUrl = "";
        private string CategoryManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public CategoryController(IMapper mapper, IHttpContextAccessor contextAccessor)
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
                HttpResponseMessage response = await _httpClient.GetAsync(CategoryManagementApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<CategoryResponse>> listCategories = JsonSerializer.Deserialize<ServiceResponse<List<CategoryResponse>>>(strData, options);
                IEnumerable<CategoryResponse> categoriesResponses = listCategories.Data;
                return View(categoriesResponses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

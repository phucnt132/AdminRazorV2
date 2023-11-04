using AdminRazorPageV2.DTOs.UserDTOs.RequestDTO;
using AdminRazorPageV2.DTOs.UserDTOs.ResponseDTO;
using APIS.DTOs.AuthenticationDTOs.ResponseDto;
using AutoMapper;
using DTOs.CommentDTOs.RequestDTO;
using DTOs.EpisodeDTOs.RequestDTO;
using DTOs.EpisodeDTOs.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AdminRazorPageV2.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string MovieApiUrl = "";
        private string CategoryManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            MovieApiUrl = "https://localhost:5003/apigateway/Movies";
            AuthApiUrl = "https://localhost:5003/apigateway/Auth";
            EpisodeManagementApiUrl = "https://localhost:5003/apigateway/Episodes";
            CategoryManagementApiUrl = "https://localhost:5003/apigateway/Categories";
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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

                // Check Authorization
                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }
                //get User list
                HttpResponseMessage response = await _httpClient.GetAsync(AuthApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<UserResponse>> listUsers = JsonSerializer.Deserialize<ServiceResponse<List<UserResponse>>>(strData, options);
                IEnumerable<UserResponse> usersResponses = listUsers.Data;
                foreach (var user in usersResponses)
                {
                    if (user.RoleId == 1)
                    {
                        user.RoleName = "Admin";
                    }
                    else if (user.RoleId == 2)
                    {
                        user.RoleName = "User";
                    }
                }
                return View(usersResponses);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

            // Check Authorization
            if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
            {
                ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                return View("Error");
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{AuthApiUrl}/id?id={id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<UserResponse> userServiceResponse = JsonSerializer.Deserialize<ServiceResponse<UserResponse>>(strData, options);

            if (userServiceResponse == null)
            {
                return NotFound();
            }

            UserResponse userResponse = userServiceResponse.Data;
            if (userResponse.RoleId == 1)
            {
                userResponse.RoleName = "Admin";
            }
            else if (userResponse.RoleId == 2)
            {
                userResponse.RoleName = "User";
            }

            return View(userServiceResponse.Data);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

            // Check Authorization
            if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
            {
                ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                return View("Error");
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{AuthApiUrl}/id?id={id}");

            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<UserResponse> userResponse = JsonSerializer.Deserialize<ServiceResponse<UserResponse>>(strData, options);
            if (userResponse == null)
            {
                return NotFound();
            }

            return View(userResponse.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("UserId")]  DeleteUserRequest deleteUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DeleteUserRequest afterUpdate = new DeleteUserRequest();
                    afterUpdate.UserId = deleteUser.UserId;
                    var episodeJson = JsonSerializer.Serialize(afterUpdate);
                    var content = new StringContent(episodeJson, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.DeleteAsync($"{AuthApiUrl}/id?id={deleteUser.UserId}");
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

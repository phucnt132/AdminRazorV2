
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using DTOs.ServiceResponseDTOs;
using System.Text;
using AutoMapper;
using AdminRazorPageV2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using AdminRazorPageV2.DTOs.CategoryDtos.ResponseDTO;
using AdminRazorPageV2.DTOs.CategoryDtos.RequestDTO;

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
            ManagementApiUrl = "https://localhost:5003/apigateway/Movies";
            AuthApiUrl = "https://localhost:5003/apigateway/Auth";
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


        // GET: All Categories
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
                IEnumerable<CategoryResponse> categoryResponses = listCategories.Data;
                return View(categoryResponses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Category by Id
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}/id?id={id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<CategoryResponse> categoryResponse = JsonSerializer.Deserialize<ServiceResponse<CategoryResponse>>(strData, options);

            if (categoryResponse == null)
            {
                return NotFound();
            }

            return View(categoryResponse.Data);
        }

        // GET: Create Category
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryDto category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoryJson = JsonSerializer.Serialize(category);
                    var content = new StringContent(categoryJson, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PostAsync($"{CategoryManagementApiUrl}/Create", content);
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Index");
        }

        // Edit Category
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}/id?id={id}");

            // loaded category data
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<CategoryResponse> categoryResponse = JsonSerializer.Deserialize<ServiceResponse<CategoryResponse>>(strData, options);

            if (categoryResponse == null)
            {
                return NotFound();
            }

            return View(categoryResponse.Data);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,IsActive")] UpdateCategoryDto category)
        {
            UpdateCategoryDto afterUpdate = new UpdateCategoryDto();
            ServiceResponse<CategoryResponse> categoryResponse;
            try
            {
                afterUpdate.CategoryId = category.CategoryId;
                afterUpdate.CategoryName = category.CategoryName;
                afterUpdate.IsActive = category.IsActive;
                var categoryJson = JsonSerializer.Serialize(afterUpdate);
                var content = new StringContent(categoryJson, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                // Check Authorization
                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }
                HttpResponseMessage response = await _httpClient.PutAsync($"{CategoryManagementApiUrl}/Update", content);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                categoryResponse = JsonSerializer.Deserialize<ServiceResponse<CategoryResponse>>(strData, options);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        // Delete Category
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}/id?id={id}");

            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<CategoryResponse> categoryResponse = JsonSerializer.Deserialize<ServiceResponse<CategoryResponse>>(strData, options);

            if (categoryResponse == null)
            {
                return NotFound();
            }

            return View(categoryResponse.Data);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int categoryId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoryJson = JsonSerializer.Serialize(categoryId);
                    var content = new StringContent(categoryJson, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PutAsync($"{CategoryManagementApiUrl}/Delete?id={categoryId}", content);
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

using DTOs.CommentDTOs.ResponseDTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using DTOs.ServiceResponseDTOs;
using DTOs.CommentDTOs.RequestDTO;
using AdminRazorPageV2.Models;
using DTOs.EpisodeDTOs.RequestDTO;

namespace AdminRazorPageV2.Controllers
{
    public class CommentController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string CommentApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public CommentController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            CommentApiUrl = "https://localhost:5003/apigateway/Comment";
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


        // GET: All Comments
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
                HttpResponseMessage response = await _httpClient.GetAsync(CommentApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<CommentResponse>> listComments = JsonSerializer.Deserialize<ServiceResponse<List<CommentResponse>>>(strData, options);
                IEnumerable<CommentResponse> commentResponses = listComments.Data;
                return View(commentResponses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{CommentApiUrl}/id?id={id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<CommentResponse> commentResponse = JsonSerializer.Deserialize<ServiceResponse<CommentResponse>>(strData, options);

            if (commentResponse == null)
            {
                return NotFound();
            };

            return View(commentResponse.Data);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("CommentId")] DeleteCommentDto commentDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DeleteCommentDto afterUpdate = new DeleteCommentDto();
                    afterUpdate.CommentId = commentDto.CommentId;
                    var commentJson = JsonSerializer.Serialize(afterUpdate);
                    var content = new StringContent(commentJson, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PutAsync($"{CommentApiUrl}/Delete?id={commentDto.CommentId}", content);
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

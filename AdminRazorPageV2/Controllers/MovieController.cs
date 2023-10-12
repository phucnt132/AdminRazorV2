using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AdminRazorPageV2.Models;
using AutoMapper.Execution;
using System.Text.Json;
using System.Net.Http.Headers;
using AutoMapper;
using DTOs.EpisodeDTOs.ResponseDTO;
using DTOs.MovieDTOs.ResponseDTO;
using APIS.DTOs.AuthenticationDTOs.ResponseDto;

namespace AdminRazorPageV2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string MovieManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public MovieController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            MovieManagementApiUrl = "http://localhost:7113/api/Movies";
            AuthApiUrl = "http://localhost:44388/api/Auth";
            EpisodeManagementApiUrl = "http://localhost:44384/api/Episodes";
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Helper function: Get Session
        public string GetSessionValue(String key)
        {
            var session = _contextAccessor.HttpContext.Session;
            return session.GetString(key);
        }

        // GET: All Movie
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(MovieManagementApiUrl);
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
    }
}

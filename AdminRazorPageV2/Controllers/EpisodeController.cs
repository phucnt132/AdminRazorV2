﻿using DTOs.EpisodeDTOs.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using DTOs.ServiceResponseDTOs;
using System.Text;
using DTOs.EpisodeDTOs.RequestDTO;
using AutoMapper;
using AdminRazorPageV2.Models;

namespace AdminRazorPageV2.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string ManagementApiUrl = "";
        private string EpisodeManagementApiUrl = "";
        private string AuthApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public EpisodeController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ManagementApiUrl = "http://localhost:44384/api/Movies";
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEpisodeDto episode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var episodeMap = _mapper.Map<AddEpisodeDto>(episode);
                    var episodeJson = JsonSerializer.Serialize(episodeMap);
                    var content = new StringContent(episodeJson, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    HttpResponseMessage response = await _httpClient.PostAsync($"{EpisodeManagementApiUrl}/Create", content);
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return View(episode);
        }

    }
}

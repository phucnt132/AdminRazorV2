using AdminRazorPageV2.DTOs.StatisticDTO.RequestDTO;
using AdminRazorPageV2.DTOs.StatisticDTO.ResponseDTO;
using AdminRazorPageV2.DTOs.UserDTOs.ResponseDTO;
using AdminRazorPageV2.Models;
using APIS.DTOs.AuthenticationDTOs.ResponseDto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AdminRazorPageV2.Controllers
{
    public class StatisticController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string AuthApiUrl = "";
        private string StatisticApiUrl = "";
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public StatisticController(IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            AuthApiUrl = "https://localhost:5003/apigateway/Auth";
            StatisticApiUrl = "https://localhost:5003/apigateway/Statistic";
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

        public async Task<IActionResult> Index()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }
                HttpResponseMessage response = await _httpClient.GetAsync(StatisticApiUrl);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<StatisticResponse>> listStatistic = JsonSerializer.Deserialize<ServiceResponse<List<StatisticResponse>>>(strData, options);
                IEnumerable<StatisticResponse> statisticResponses = listStatistic.Data;
                return View(statisticResponses);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> StatictisByDate([Bind("StartDate,EndDate")] SortStatistic sortStatistic)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }

                string startDate = sortStatistic.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                string endDate = sortStatistic.EndDate.ToString("yyyy-MM-dd HH:mm:ss");

                var statisticJson = JsonSerializer.Serialize(sortStatistic);
                var content = new StringContent(statisticJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{StatisticApiUrl}", content);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ServiceResponse<List<StatisticResponse>> listStatistic = JsonSerializer.Deserialize<ServiceResponse<List<StatisticResponse>>>(strData, options);
                IEnumerable<StatisticResponse> statisticResponses = listStatistic.Data;
                return View("Index", statisticResponses);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

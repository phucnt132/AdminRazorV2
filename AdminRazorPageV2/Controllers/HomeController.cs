﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Drawing;
using AdminRazorPageV2.Models;
using Newtonsoft.Json.Linq;

namespace HighFlixAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient = null;
        private string ManagementApiUrl = "";
        private string AuthApiUrl = "";

        public HomeController()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ManagementApiUrl = "http://localhost:7113/api/Movies";
            AuthApiUrl = "http://localhost:7111/api/Auth";
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                try
                {
                    user.Username = username;
                    user.Password = password;
                    var userJson = JsonSerializer.Serialize(user);
                    var content = new StringContent(userJson, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _httpClient.PostAsync($"{AuthApiUrl}/login", content);
                    string userResponse = await response.Content.ReadAsStringAsync();


                    // Parse the JSON string
                    JObject jsonObject = JObject.Parse(userResponse);

                    // Extract the accessToken and refreshToken
                    string accessToken = jsonObject["accessToken"].ToString();
                    string refreshToken = jsonObject["refreshToken"].ToString();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    User mem = JsonSerializer.Deserialize<User>(userResponse, options);
                    if (response.IsSuccessStatusCode)
                    {
                        HttpContext.Session.SetString("AccessToken", accessToken);
                        HttpContext.Session.SetString("RefreshToken", refreshToken);
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetInt32("UserId", user.UserId);

                        return View("Index");
                    }

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid input data.");
                        return View("Index");
                    }
                    return View("Index");
                }
                catch (Exception)
                {
                    return View("Index");
                }
            }

            return View("Index");
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_token_here");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(AuthApiUrl)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "your_token_here");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Xử lý dữ liệu trả về
                }
                else
                {
                    // Xử lý lỗi
                }
            }

            return View();
        }

    }
}
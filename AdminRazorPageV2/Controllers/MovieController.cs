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

namespace AdminRazorPageV2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly HttpClient client = null;
        private string MovieApiUrl = "";

        public MovieController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            MovieApiUrl = "http://localhost:7113/api/Movies";
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(MovieApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<Movie> listMovie = JsonSerializer.Deserialize<List<Movie>>(strData, options);
            return View(listMovie);
        }
    }
}

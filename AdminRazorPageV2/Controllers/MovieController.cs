using AdminRazorPageV2.Models;
using AutoMapper;
using AdminRazorPageV2.DTOs.CategoryDtos.ResponseDTO;
using DTOs.EpisodeDTOs.RequestDTO;
using DTOs.EpisodeDTOs.ResponseDTO;
using DTOs.MovieDTOs.RequestDto;
using DTOs.MovieDTOs.ResponseDTO;
using DTOs.ServiceResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

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
            ManagementApiUrl = "https://localhost:5003/apigateway/Movies";
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

            return View(movieResponse.Data);
        }

        //GET: Movie/Create
        public async Task<IActionResult> Create()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            ServiceResponse<List<CategoryResponse>> category = JsonSerializer.Deserialize<ServiceResponse<List<CategoryResponse>>>(strData, options);
            IEnumerable<CategoryResponse> categories = category.Data;

            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddMovieDto movie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    movie.PostedByUser = (int)HttpContext.Session.GetInt32("UserId");
                    var movieJson = JsonSerializer.Serialize(movie);
                    var content = new StringContent(movieJson, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));

                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PostAsync($"{ManagementApiUrl}/Create", content);
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Index");
        }

        //GET: Movie/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{CategoryManagementApiUrl}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            ServiceResponse<List<CategoryResponse>> category = JsonSerializer.Deserialize<ServiceResponse<List<CategoryResponse>>>(strData, options);
            IEnumerable<CategoryResponse> categories = category.Data;

            ViewBag.Categories = categories;

            if (id == null)
            {
                return NotFound();
            }
            response = await _httpClient.GetAsync($"{ManagementApiUrl}/update/id?id={id}");

            // loaded movie data
            strData = await response.Content.ReadAsStringAsync();

            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ServiceResponse<MovieResponse> movieResponse = JsonSerializer.Deserialize<ServiceResponse<MovieResponse>>(strData, options);

            UpdateMovieDto updateMovie = new UpdateMovieDto { 
                MovieId = movieResponse.Data.MovieId,
                MovieName = movieResponse.Data.MovieName,
                Categories = movieResponse.Data.Categories.Select(int.Parse).ToList(),
                MovieThumnailImage = movieResponse.Data.MovieThumnailImage,
                MoviePoster = movieResponse.Data.MoviePoster,
                TotalEpisodes = movieResponse.Data.TotalEpisodes,
                Description = movieResponse.Data.Description,
                ReleasedYear = movieResponse.Data.ReleasedYear,
                AliasName = movieResponse.Data.AliasName,
                Director = movieResponse.Data.Director,
                MainCharacters = movieResponse.Data.MainCharacters,
                Trailer = movieResponse.Data.Trailer,
            };

            if (updateMovie == null)
            {
                return NotFound();
            }

            return View(updateMovie);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMovieDto movie)
        {
            ServiceResponse<MovieResponse> movieResponse;
            try
            {
                var movieJson = JsonSerializer.Serialize(movie);
                var content = new StringContent(movieJson, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                // Check Authorization
                if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                {
                    ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                    return View("Error");
                }
                HttpResponseMessage response = await _httpClient.PutAsync($"{ManagementApiUrl}/Update", content);
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                movieResponse = JsonSerializer.Deserialize<ServiceResponse<MovieResponse>>(strData, options);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{ManagementApiUrl}/update/id?id={id}");
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

        // POST: Movie/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int movieId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var movieJson = JsonSerializer.Serialize(movieId);
                    var content = new StringContent(movieJson, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetSessionValue("AccessToken"));
                    // Check Authorization
                    if (_httpClient.DefaultRequestHeaders.Authorization.Parameter == null)
                    {
                        ViewData["AuthorizationMessage"] = "You do not have permission to do this action!";
                        return View("Error");
                    }
                    HttpResponseMessage response = await _httpClient.PutAsync($"{ManagementApiUrl}/Delete?id={movieId}", content);
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

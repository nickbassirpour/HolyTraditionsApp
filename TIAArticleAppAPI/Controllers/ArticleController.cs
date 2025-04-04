using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIAArticleAppAPI.Models;
using TIAArticleAppAPI.Services;

namespace TIAArticleAppAPI.Controllers
{
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticleController(IArticleService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("add_new_article")]
        public async Task<IActionResult> AddArticle([FromBody] ArticleModel article)
        {
            try
            {
                var response = await _service.AddNewArticle(article);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return StatusCode(201, new { id = success});
                    },
                    error =>
                    {
                        return StatusCode(500, "Error adding article.");
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_articles_by_category")]
        public async Task<IActionResult> GetArticlesByCategory(string category, int limit)
        {
            try
            {
                var response = await _service.GetArticleListByCategory(category, limit);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return Ok(success);
                    },
                    error =>
                    {
                        return StatusCode(500, "Error finding articles.");
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_articles_by_subcategory")]
        public async Task<IActionResult> GetArticlesBySubcategory(string subcategory, int limit)
        {
            try
            {
                var response = await _service.GetArticleListBySubcategory(subcategory, limit);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return Ok(success);
                    },
                    error =>
                    {
                        return StatusCode(500, "Error finding articles.");
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_article_by_id")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            try
            {
                var response = await _service.GetArticleById(articleId);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return Ok(success);
                    },
                    error =>
                    {
                        return StatusCode(500, "Error finding article.");
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_article_by_url")]
        public async Task<IActionResult> GetArticleByUrl(string articleUrl)
        {
            try
            {
                var response = await _service.GetArticleByUrl(articleUrl);
                return response.Match<IActionResult>
                    (
                    success => { return Ok(success);},
                    error => { return StatusCode(500, "Error finding article.");}
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_all_authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var response = await _service.GetAllAuthors();
                return response.Match<IActionResult>
                    (
                    success => { return Ok(success); },
                    error => { return StatusCode(500, "Error getting authors."); }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_all_categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var response = await _service.GetAllCategories();
                return response.Match<IActionResult>
                    (
                    success => { return Ok(success); },
                    error => { return StatusCode(500, "Error getting categories."); }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_all_subcategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var response = await _service.GetAllSubCategories();
                return response.Match<IActionResult>
                    (
                    success => { return Ok(success); },
                    error => { return StatusCode(500, "Error getting subcategories."); }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

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
    }
}

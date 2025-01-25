﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("add_new_article")]
        public async Task<IActionResult> AddArticle(ArticleModel article)
        {
            try
            {
                var response = await _service.AddNewArticle(article);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return Statusc
                    })
            }
        }
    }
}

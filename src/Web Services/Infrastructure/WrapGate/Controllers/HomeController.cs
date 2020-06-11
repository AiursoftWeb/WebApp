﻿using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Wrapgate.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Aiursoft Wrapgate!");
        }
    }
}
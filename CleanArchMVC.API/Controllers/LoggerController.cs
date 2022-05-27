using CleanArchMVC.Application.DTOs;
using CleanArchMVC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchMVC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        public LoggerController()
        {

        }
        [HttpGet("nlog")]
        public IActionResult GetNLog(CancellationToken ct)
        {

            var logger = LogManager.GetCurrentClassLogger();

            logger.Info()
                .Message("Log Erro Rodrigo")
                .Property("Erropayload", "Teste teste teste teste")
                .Write();

            return Ok("Deu Certo !! AEEe !");
        }
    }
}

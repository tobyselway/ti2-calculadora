﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Calculadora.Models;
using Microsoft.AspNetCore.Http;

namespace Calculadora.Controllers
{
    public class HomeController : Controller
    {

        private static bool SHOW_FULL = true;
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Responde a um pedido GET feito a / com a view da calculadora com o valor 0
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Index()
        {
            HttpContext.Session.SetString("first_num", "");
            HttpContext.Session.SetString("second_num", "");
            HttpContext.Session.SetString("op_selected", "");
            HttpContext.Session.SetString("last_expression", "");

            String first = HttpContext.Session.GetString("first_num");
            ViewBag.display = (first=="" ? "0" : first) + " " +
                              HttpContext.Session.GetString("op_selected") + " " +
                              HttpContext.Session.GetString("second_num");
            
            ViewBag.lastDisplay = HttpContext.Session.GetString("last_expression");
            
            return View();
        }
        
        /// <summary>
        /// Responde a um pedido POST feito a / com a view da calculadora com o valor calculado
        /// de acordo com o botão carregado
        /// </summary>
        /// <param name="type">O tipo de botão carregado</param>
        /// <param name="value">O valor do botão carregado</param>
        /// <returns>View</returns>
        [HttpPost]
        public IActionResult Index(String type, String value)
        {
            switch (type)
            {
                case "digit":
                    if (HttpContext.Session.GetString("op_selected") != "")
                    {
                        HttpContext.Session.SetString("second_num", HttpContext.Session.GetString("second_num") + value);
                    }
                    else
                    {
                        HttpContext.Session.SetString("first_num", HttpContext.Session.GetString("first_num") + value);
                    }
                    break;
                case "operation":
                    if (HttpContext.Session.GetString("first_num") == "")
                        HttpContext.Session.SetString("first_num", "0");
                    HttpContext.Session.SetString("op_selected", value);
                    break;
                case "function":
                    switch (value)
                    {
                        case "separator":
                            if (HttpContext.Session.GetString("op_selected") != "")
                            {
                                String secondNum = HttpContext.Session.GetString("second_num");
                                if(!secondNum.Contains("."))
                                    HttpContext.Session.SetString("second_num", (secondNum == "" ? "0" : secondNum) + ".");
                            }
                            else
                            {
                                String firstNum = HttpContext.Session.GetString("first_num");
                                if(!firstNum.Contains("."))
                                    HttpContext.Session.SetString("first_num", (firstNum == "" ? "0" : firstNum) + ".");
                            }
                            break;
                        case "negative":
                            if (HttpContext.Session.GetString("op_selected") != "")
                            {
                                double secondVal = Convert.ToDouble(HttpContext.Session.GetString("second_num") == "" ? "0" : HttpContext.Session.GetString("second_num"));
                                secondVal *= -1;
                                HttpContext.Session.SetString("second_num", "" + secondVal);
                            }
                            else
                            {
                                double firstVal = Convert.ToDouble(HttpContext.Session.GetString("first_num") == "" ? "0" : HttpContext.Session.GetString("first_num"));
                                firstVal *= -1;
                                HttpContext.Session.SetString("first_num", "" + firstVal);
                            }
                            break;
                        case "clear":
                            return Index();
                        case "equals":
                            double first = Convert.ToDouble(HttpContext.Session.GetString("first_num"));
                            double second = Convert.ToDouble(HttpContext.Session.GetString("second_num") == "" ? "0" : HttpContext.Session.GetString("second_num"));
                            double result = 0;
                            switch (HttpContext.Session.GetString("op_selected"))
                            {
                                case ":":
                                    result = first / second;
                                    break;
                                case "x":
                                    result = first * second;
                                    break;
                                case "-":
                                    result = first - second;
                                    break;
                                case "+":
                                    result = first + second;
                                    break;
                                default:
                                    result = first;
                                    break;
                            }
                            HttpContext.Session.SetString("last_expression", HttpContext.Session.GetString("first_num") + " " +
                                                                             HttpContext.Session.GetString("op_selected") + " " +
                                                                             HttpContext.Session.GetString("second_num") + " = ");
                            HttpContext.Session.SetString("first_num", "" + result);
                            HttpContext.Session.SetString("second_num", "");
                            HttpContext.Session.SetString("op_selected", "");
                            break;
                    }
                    break;
            }

            if (!SHOW_FULL)
            {
                if (HttpContext.Session.GetString("op_selected") != "" && HttpContext.Session.GetString("second_num") != "")
                {
                    ViewBag.display = HttpContext.Session.GetString("second_num");
                }
                else
                {
                    ViewBag.display = HttpContext.Session.GetString("first_num");
                }
            }
            else
            {
                ViewBag.display = HttpContext.Session.GetString("first_num") + " " +
                                  HttpContext.Session.GetString("op_selected") + " " +
                                  HttpContext.Session.GetString("second_num");
            }

            ViewBag.lastDisplay = HttpContext.Session.GetString("last_expression");
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}

﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Employee360.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Employee360.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyf;
        private readonly string _AddNewEmpAPI;



        public HomeController(IConfiguration configuration, INotyfService notyf)
        {
            _configuration = configuration;
            _notyf = notyf;
            _AddNewEmpAPI = _configuration["AddDataAPI"];
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ToAdminDashboard(AdminLoginModel model)
        {
            var email = _configuration["AdminCredentials:Email"];
            var password = _configuration["AdminCredentials:Password"];

            if (model.Email == email && model.Password == password)
            {
                // Authentication successful
                return View("AdminDashboard");
            }

            // Authentication failed
            _notyf.Error("Invalid Credentials");
            return View("Login");

        }

        public IActionResult ToNewEmployeeForm()
        {

            return View("NewEmployeeForm");
        }
        public async Task <IActionResult> AddNewEmpPloyeeToDb(EmpDataViewModel empdata)
        {
            var EmployeeData = new EmpDataViewModel
            {
                Name = empdata.Name,
                FatherName = empdata.FatherName,
                MotherName = empdata.MotherName,
                Email = empdata.Email,
                Phone = empdata.Phone,
                Address = empdata.Address,
                Dob = empdata.Dob,
                Post = empdata.Post,
                Salary = empdata.Salary,
                Incperiod = empdata.Incperiod 
            };

            string jsonData = JsonConvert.SerializeObject(EmployeeData);


            var EmployeeDataContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_AddNewEmpAPI, EmployeeDataContent);

                if (response.IsSuccessStatusCode)
                {
                    
                    Console.WriteLine("Successfull");
                    _notyf.Success("Employee Added");
                    return View("AdminDashboard");

                }
                else
                {
                    _notyf.Error("Error");
                    Console.WriteLine("Failed");
                    return View("NewEmployeeForm");
                }
            }
 
        }
        public IActionResult ShowEmpployeeList()
        {
            return View("EmployeeListView");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
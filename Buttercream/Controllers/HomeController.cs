using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Buttercream.Models;
using Buttercream.Core.Entities;
using Buttercream.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Buttercream.Core.Interfaces;

namespace Buttercream.Controllers
{
    public class HomeController : Controller
    {
        IRestService _restService;

        public HomeController(IRestService restService)
        {
            _restService = restService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet]
        public IActionResult MakePayment()
        {
            ViewData["Message"] = "Payment Page";
            var payment = new Payment
            {
                Banks = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Access Bank", Value = "044"},
                    new SelectListItem {Text = "Access Bank (Diamond)", Value = "063"},
                    new SelectListItem {Text = "ALAT by WEMA", Value = "035A"},
                    new SelectListItem {Text = "ASO Savings and Loans", Value = "401"},
                    new SelectListItem {Text = "Citibank Nigeria", Value = "023"},
                    new SelectListItem {Text = "Ecobank Nigeria", Value = "050"},
                    new SelectListItem {Text = "Ekondo Microfinance Bank", Value = "562"},
                    new SelectListItem {Text = "Enterprise Bank", Value = "084"},
                    new SelectListItem {Text = "Fidelity Bank", Value = "070"},
                    new SelectListItem {Text = "First Bank of Nigeria", Value = "011"},
                    new SelectListItem {Text = "First City Monument Bank", Value = "214"},
                    new SelectListItem {Text = "Guaranty Trust Bank", Value = "058"},
                    new SelectListItem {Text = "Heritage Bank", Value = "030"},
                    new SelectListItem {Text = "Jaiz Bank", Value = "301"},
                    new SelectListItem {Text = "Keystone Bank", Value = "082"},
                    new SelectListItem {Text = "MainStreet Bank", Value = "014"},
                    new SelectListItem {Text = "Parallex Bank", Value = "526"},
                    new SelectListItem {Text = "Polaris Bank", Value = "076"},
                    new SelectListItem {Text = "Providus Bank", Value = "101"},
                    new SelectListItem {Text = "Stanbic IBTC Bank", Value = "221"},
                    new SelectListItem {Text = "Standard Chartered Bank", Value = "068"},
                    new SelectListItem {Text = "Sterling Bank", Value = "232"},
                    new SelectListItem {Text = "Suntrust Bank", Value = "100"},
                    new SelectListItem {Text = "Union Bank of Nigeria", Value = "032"},
                    new SelectListItem {Text = "United Bank For Africa", Value = "033"},
                    new SelectListItem {Text = "Unity Bank", Value = "215"},
                    new SelectListItem {Text = "Wema Bank", Value = "035"},
                    new SelectListItem {Text = "Zenith Bank", Value = "057"}
                }
            };
            ViewData["Banks"] = payment.Banks;
            return View(payment);
        }

        [HttpPost]
        public IActionResult MakePayment(IFormCollection payment)
        {
            var paymentItem = new Payment();
            var banks = ViewData["Banks"] as List<SelectListItem>;
            if (!ModelState.IsValid)
            {
                paymentItem.ErrorMessage = "Error: Validation Failed";
                paymentItem.DestinationAccountNo = payment["DestinationAccountNo"];
                return View(paymentItem);
            }

            var destAcct = payment["DestinationAccountNo"].ToString();
            var destBankCode = payment["DestinationBankCode"].ToString();

            // verify account number
            var res = _restService.ResolveAccountNumber(destAcct, destBankCode);
            if (res != null)
            {
                paymentItem.CustomerName = res.Account_name;
                paymentItem.DestinationAccountNo = res.Account_number;
                paymentItem.DestinationBankCode = res.BankCode;
                return View("InitiateTransfer", paymentItem);
            }

            paymentItem.Banks = banks;
            paymentItem.ErrorMessage = "Failed to verify customer's details";
            paymentItem.DestinationAccountNo = payment["DestinationAccountNo"];
            return View(payment);
        }

        [HttpGet]
        public IActionResult InitiateTransfer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InitiateTransfer(Payment payment)
        {
            if (!ModelState.IsValid)
            {     
                return View();
            }
            // create transfer recipient
            // initiate transfer recipient
            var res = _restService.InitiateTransfer(payment);
            if (res != null)
            {
                payment.ErrorMessage = res.responseMessage;
                return View(payment);
            }
            
            payment.ErrorMessage = "Payment Failed";
            return View(payment);
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
    }
}

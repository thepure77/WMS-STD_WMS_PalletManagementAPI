using MasterDataBusiness;
using MasterDataBusiness.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PalletmanageBusiness.BusinessUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Hosting;
using PalletmanageBusiness.pallet;
using System.Net.Http;
using System.Net;
using OnhandSummaryBusiness;

namespace PalletmanageAPI.Controllers
{
    [Route("api/onhandsummary")]
    public class OnhandsummaryController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public OnhandsummaryController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] JObject body)
        {
            try
            {
                var service = new OnhandSummaryService();
                var Models = new OnhandSummaryViewModel();
                Models = JsonConvert.DeserializeObject<OnhandSummaryViewModel>(body.ToString());
                var result = service.Filter(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] JObject body)
        {
            string result = "";
            try
            {
                var service = new OnhandSummaryService();
                var Models = new OnhandSummaryViewModel();
                Models = JsonConvert.DeserializeObject<OnhandSummaryViewModel>(body.ToString());
                result = service.generate_excel(Models, _hostingEnvironment.ContentRootPath);
                if (!System.IO.File.Exists(result))
                {
                    return NotFound();
                }
                return File(System.IO.File.ReadAllBytes(result), "application/octet-stream");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            finally
            {
                System.IO.File.Delete(result);
            }
        }

    }
}

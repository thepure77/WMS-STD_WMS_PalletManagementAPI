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

namespace PalletmanageAPI.Controllers
{
    [Route("api/pallets")]
    public class palletController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public palletController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] JObject body)
        {
            try
            {
                var service = new PalletService();
                var Models = new PalletFilterViewModel();
                Models = JsonConvert.DeserializeObject<PalletFilterViewModel>(body.ToString());
                var result = service.Filter(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("Savepallet")]
        public IActionResult SaveAddpallet([FromBody] JObject body)
        {
            try
            {
                var service = new PalletService();
                var Models = new PalletViewModel();
                Models = JsonConvert.DeserializeObject<PalletViewModel>(body.ToString());
                var result = service.SaveAddPallet(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("Deletepallet")]
        public IActionResult Deletepallet([FromBody] PalletViewModel Models)
        {
            try
            {
                var service = new PalletService();
                var result = service.DeletePallet(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("FilterSum")]
        public IActionResult FilterSum()
        {
            try
            {
                var service = new PalletService();
                var result = service.FilterSum();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }        

        [HttpPost("printPallet")]
        public IActionResult printPallet([FromBody] JObject body)
        {
            string localFilePath = "";
            try
            {
                var service = new PalletService();
                var Models = new PalletResultViewModel();
                Models = JsonConvert.DeserializeObject<PalletResultViewModel>(body.ToString());
                localFilePath = service.printPallet(Models, _hostingEnvironment.ContentRootPath);
                if (!System.IO.File.Exists(localFilePath))
                {
                    return NotFound();
                }
                return File(System.IO.File.ReadAllBytes(localFilePath), "application/octet-stream");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            finally
            {
                System.IO.File.Delete(localFilePath);
            }
        }

        [HttpPost]
        [Route("ExportExcel")]
        public IActionResult ExportExcel([FromBody] JObject body)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            string StockMovementPath = "";
            try
            {
                PalletService _appService = new PalletService();
                var Models = new PalletFilterViewModel();
                Models = JsonConvert.DeserializeObject<PalletFilterViewModel>(body.ToString());
                StockMovementPath = _appService.ExportExcel(Models, _hostingEnvironment.ContentRootPath);

                if (!System.IO.File.Exists(StockMovementPath))
                {
                    return NotFound();
                }
                return File(System.IO.File.ReadAllBytes(StockMovementPath), "application/octet-stream");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                System.IO.File.Delete(StockMovementPath);
            }
        }

    }
}

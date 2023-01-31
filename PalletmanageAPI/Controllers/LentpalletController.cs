using Business.Models;
using MasterDataBusiness;
using MasterDataBusiness.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PalletmanageBusiness.BusinessUnit;
using PalletmanageBusiness.pallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PalletmanageAPI.Controllers
{   

    [Route("api/lentpallets")]
    public class LentpalletController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public LentpalletController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] JObject body)
        {
            try
            {
                var service = new LentPalletService();
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

        [HttpPost("SaveLentpallet")]
        public IActionResult SaveAddLentpallet([FromBody] JObject body)
        {
            try
            {
                var service = new LentPalletService();
                var Models = new LentPalletViewModel();
                Models = JsonConvert.DeserializeObject<LentPalletViewModel>(body.ToString());
                var result = service.SaveLentPallet(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("DeleteLentpallet")]
        public IActionResult DeleteLentpallet([FromBody] LentPalletViewModel Models)
        {
            try
            {
                var service = new LentPalletService();
                var result = service.DeleteLentPallet(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("printLentPallet")]
        public IActionResult printLentPallet([FromBody] JObject body)
        {
            string localFilePath = "";
            try
            {
                var service = new LentPalletService();
                var Models = new LentPalletResultViewModel();
                Models = JsonConvert.DeserializeObject<LentPalletResultViewModel>(body.ToString());
                localFilePath = service.printLentPallet(Models, _hostingEnvironment.ContentRootPath);
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
                LentPalletService _appService = new LentPalletService();
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

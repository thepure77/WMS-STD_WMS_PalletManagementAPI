using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using MasterDataBusiness;
using MasterDataBusiness.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PalletmanageAPI.Controllers
{
    [Route("api/Dropdown")]
    public class VehicleTypeController : Controller
    {
        [HttpPost("vehicleTypedropdown")]
        public IActionResult vehicleTypedropdown([FromBody] JObject body)
        {
            try
            {
                var service = new DropdownService();
                var Models = new VehicleTypeViewModel();
                Models = JsonConvert.DeserializeObject<VehicleTypeViewModel>(body.ToString());
                var result = service.vehicleTypedropdown(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("documentTypefilter")]
        public IActionResult documentTypefilter([FromBody] JObject body)

        {
            try
            {
                var service = new DropdownService();
                var Models = new DocumentTypeViewModel();
                Models = JsonConvert.DeserializeObject<DocumentTypeViewModel>(body.ToString());
                var result = service.documentTypefilter(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("vendordropdown")]
        public IActionResult vendordropdown([FromBody] JObject body)
        {
            try
            {
                var service = new DropdownService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.vendordropdown(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}

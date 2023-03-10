using MasterDataBusiness;
using MasterDataBusiness.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalletmanageAPI.Controllers
{
    [Route("api/Autocomplete")]
    public class AutocompleteController : Controller
    {
        #region autoProduct
        [HttpPost("autoProduct")]
        public IActionResult autoProduct([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoProduct(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region autoSku
        [HttpPost("autoSku")]
        public IActionResult autoSku([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoSku(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region autoLocation
        [HttpPost("autoLocation")]
        public IActionResult autoLocation([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoLocation(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region autoProductId
        [HttpPost("autoProductId")]
        public IActionResult autoProductId([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoProductId(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region autoVendor
        [HttpPost("autoVendor")]
        public IActionResult autoVendor([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoVendor(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region autoDocument
        [HttpPost("autoDocument")]
        public IActionResult autoDocument([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoDocument(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion
        
        #region autoVehicleType
        [HttpPost("autoVehicleType")]
        public IActionResult autoVehicleType([FromBody] JObject body)
        {
            try
            {
                var service = new AutocompleteService();
                var Models = new ItemListViewModel();
                Models = JsonConvert.DeserializeObject<ItemListViewModel>(body.ToString());
                var result = service.autoVehicleType(Models);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion
    }
}

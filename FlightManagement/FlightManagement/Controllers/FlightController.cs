
using System.Web.Http.Description;
using FlightManagement.Common.Constant;
using FlightManagement.Common.DTO;
using FlightManagement.Common.Enums;
using FlightManagement.Common.OperationDTO;
using FlightManagement.Services.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagment.Controllers
{
    [Route("api/Flight")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFlightDataService _flightDataService;
        private readonly IFlightScheduleCheckerService _flightScheduleCheckerService;
        public FlightController(IFlightDataService flightDataService, IFlightScheduleCheckerService flightScheduleCheckerService,
            IConfiguration configuration)
        {
            _flightDataService = flightDataService;
            _flightScheduleCheckerService = flightScheduleCheckerService;
            _configuration = configuration;
        }


        [HttpGet("ProcessFlightData")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> ProcessFlightData()
        {
            string? fileLocation = _configuration.GetSection("FileLocation")?.GetSection("csvFileLocation")?.Value;
            if (string.IsNullOrEmpty(fileLocation))
            {
                return new ResponseMessage
                {
                    ResponseCode = (int)AppEnums.StatusCode.ERROR,
                    Message = AppConstants.Message.FileLocationNotConfigured
                };
            }
            return await _flightDataService.ProcessFlightData(fileLocation);
        }


        [HttpGet("GetFlightData")]
        [ProducesResponseType(typeof(FlightData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<string> GetFlightData()
        {
            string fileLocation = Path.Combine(AppContext.BaseDirectory[..AppContext.BaseDirectory.IndexOf("bin")], "jsonData.txt");
            if (!System.IO.File.Exists(fileLocation))
            {
                return AppConstants.Message.NoJsonDataFound;
            }

            try
            {
                return await _flightDataService.GetFlightData(fileLocation);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        [HttpGet("GetInconsistentFlights")]
        [ProducesResponseType(typeof(FlightData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<string> GetInconsistentFlights()
        {
            try
            {
                string fileLocation = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "jsonData.txt";
                if (!System.IO.File.Exists(fileLocation))
                {
                    throw new FileNotFoundException(AppConstants.Message.NoJsonDataFoundForInconsistancyCheck);
                }

                string jsonData = await System.IO.File.ReadAllTextAsync(fileLocation);
                return await _flightScheduleCheckerService.GetInconsistentFlights(jsonData);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}

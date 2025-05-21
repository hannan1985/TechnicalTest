
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


        /// <summary>
        /// Retrieves inconsistent flight data from the JSON file.
        /// </summary>
        /// <returns>
        /// A string containing inconsistent flight data or an error message if the file is not found or an exception occurs.
        /// </returns>
        /// <response code="200">Returns the inconsistent flight data.</response>
        /// <response code="404">JSON file not found for inconsistency check.</response>
        /// <response code="500">An error occurred while processing the request.</response>
        [HttpGet("GetInconsistentFlights")]
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

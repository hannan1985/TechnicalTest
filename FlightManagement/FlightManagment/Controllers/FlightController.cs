using FlightManagement.Common.Constant;
using FlightManagement.Common.DTO;
using FlightManagement.Common.Enums;
using FlightManagement.Common.OperationDTO;
using FlightManagement.Services.ApiServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
            ResponseMessage objResponseMessage = new ResponseMessage();
            string? fileLocation = _configuration.GetSection("FileLocation")?.GetSection("csvFileLocation")?.Value;

            if (string.IsNullOrEmpty(fileLocation))
            {
                objResponseMessage.ResponseCode = (int)AppEnums.StatusCode.ERROR;
                objResponseMessage.Message = AppConstants.Message.FileLocationNotConfigured;
                return objResponseMessage;
            }

            objResponseMessage = await _flightDataService.ProcessFlightData(fileLocation);
            return objResponseMessage;
        }


        [HttpGet("GetFlightData")]
        public async Task<string> GetFlightData()
        {
            string finalResult = string.Empty;
            string fileLocation = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "jsonData.txt";
            try
            {
                if (!System.IO.File.Exists(fileLocation))
                    throw new FileNotFoundException(AppConstants.Message.NoJsonDataFound);

                finalResult = await _flightDataService.GetFlightData(fileLocation);
            }
            catch (Exception ex)
            {
                finalResult = ex.Message;
            }

            return finalResult;
        }


        [HttpGet("GetInconsistentFlights")]
        public async Task<string> GetInconsistentFlights()
        {
            try
            {
                string fileLocation = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "jsonData.txt";
                if (!System.IO.File.Exists(fileLocation))
                    throw new FileNotFoundException(AppConstants.Message.NoJsonDataFoundForInconsistancyCheck);

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

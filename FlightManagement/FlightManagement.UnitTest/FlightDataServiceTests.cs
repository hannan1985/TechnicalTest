
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightManagement.Common.Constant;
using FlightManagement.Common.DTO;
using FlightManagement.Common.Enums;
using FlightManagement.Common.OperationDTO;
using FlightManagement.Services.ApiServices;
using Moq;
using Xunit;

namespace FlightManagement.Services.Tests.ApiServices
{
    public class FlightDataServiceTests
    {
        private readonly FlightDataService _flightDataService;

        public FlightDataServiceTests()
        {
            _flightDataService = new FlightDataService();
        }

        [Fact]
        public async Task GetFlightData_ShouldReturnJsonData_WhenFileExists()
        {
            // Arrange  
            string filePath = "test.json";
            string expectedJson = "{\"key\":\"value\"}";
            await File.WriteAllTextAsync(filePath, expectedJson);

            // Act  
            string result = await _flightDataService.GetFlightData(filePath);

            // Assert  
            Assert.Equal(expectedJson, result);

            // Cleanup  
            File.Delete(filePath);
        }

        [Fact]
        public async Task ProcessFlightData_ShouldReturnError_WhenNoCsvFilesFound()
        {
            // Arrange  
            string directoryPath = "EmptyDirectory";
            Directory.CreateDirectory(directoryPath);

            // Act  
            var response = await _flightDataService.ProcessFlightData(directoryPath);

            // Assert  
            Assert.Equal((int)AppEnums.StatusCode.ERROR, response.ResponseCode);
            Assert.Contains(AppConstants.Message.NoCSVFilesFound, response.Message);

            // Cleanup  
            Directory.Delete(directoryPath);
        }

        [Fact]
        public async Task ProcessFlightData_ShouldReturnSuccess_WhenCsvIsProcessed()
        {
            // Arrange  
            string directoryPath = "TestDirectory";
            string csvFilePath = Path.Combine(directoryPath, "test.csv");
            Directory.CreateDirectory(directoryPath);
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,ABC123,Boeing737,FL123,ATL,2023-10-01T10:00:00,NYC,2023-10-01T12:00:00";
            await File.WriteAllTextAsync(csvFilePath, csvContent);

            // Act  
            var response = await _flightDataService.ProcessFlightData(directoryPath);

            // Assert  
            Assert.Equal((int)AppEnums.StatusCode.SUCCESS, response.ResponseCode);
            Assert.Equal(AppConstants.Message.CSVProcessedSuccessfully, response.Message);

            // Cleanup  
            Directory.Delete(directoryPath, true);
        }

        [Fact]
        public async Task ReadFlightsFromCsvAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange  
            string filePath = "nonexistent.csv";

            // Act & Assert  
            await Assert.ThrowsAsync<FileNotFoundException>(() => _flightDataService.ReadFlightsFromCsvAsync(filePath));
        }

        [Fact]
        public async Task ValidateAndWriteCsvWithErrorsAsync_ShouldReturnFalse_WhenInvalidRowsExist()
        {
            // Arrange  
            string inputFilePath = "input.csv";
            string outputFilePath = "output.csv";
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,ABC123,Boeing737,FL123,ATL,InvalidDate,NYC,2023-10-01T12:00:00";
            await File.WriteAllTextAsync(inputFilePath, csvContent);

            // Act  
            bool isValid = await _flightDataService.ValidateAndWriteCsvWithErrorsAsync(inputFilePath, outputFilePath);

            // Assert  
            Assert.False(isValid);
            Assert.True(File.Exists(outputFilePath));

            // Cleanup  
            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }
    }
}

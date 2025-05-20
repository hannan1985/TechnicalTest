using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightManagement.Common.DTO;
using FlightManagement.Common.OperationDTO;
using FlightManagement.Services.ApiServices;
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
        public async Task GetFlightData_ValidFile_ReturnsJsonData()
        {
            // Arrange  
            string filePath = "testFile.json";
            string expectedJson = "[{\"id\":1,\"aircraft_registration_number\":\"ABC123\",\"aircraft_type\":\"Boeing\",\"flight_number\":\"FL123\",\"departure_airport\":\"JFK\",\"departure_datetime\":\"2023-10-01T10:00:00\",\"arrival_airport\":\"LAX\",\"arrival_datetime\":\"2023-10-01T14:00:00\"}]";
            await File.WriteAllTextAsync(filePath, expectedJson);

            // Act  
            string result = await _flightDataService.GetFlightData(filePath);

            // Assert  
            Assert.Equal(expectedJson, result);

            // Cleanup  
            File.Delete(filePath);
        }

        [Fact]
        public async Task ProcessFlightData_ValidCsv_ReturnsSuccessResponse()
        {
            // Arrange  
            string filePath = "testFile.csv";
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,ABC123,Boeing,FL123,JFK,2023-10-01T10:00:00,LAX,2023-10-01T14:00:00";
            await File.WriteAllTextAsync(filePath, csvContent);

            // Act  
            ResponseMessage response = await _flightDataService.ProcessFlightData(filePath);

            // Assert  
            Assert.Equal(200, response.ResponseCode);
            Assert.Equal("CSV processed successfully.", response.Message);

            // Cleanup  
            File.Delete(filePath);
        }

        [Fact]
        public async Task ProcessFlightData_InvalidCsv_ReturnsErrorResponse()
        {
            // Arrange  
            string filePath = "testFile.csv";
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,,Boeing,FL123,JFK,InvalidDate,LAX,2023-10-01T14:00:00";
            await File.WriteAllTextAsync(filePath, csvContent);

            // Act  
            ResponseMessage response = await _flightDataService.ProcessFlightData(filePath);

            // Assert  
            Assert.Equal(500, response.ResponseCode);
            Assert.Contains("Error processing CSV", response.Message);

            // Cleanup  
            File.Delete(filePath);
        }

        [Fact]
        public async Task ReadFlightsFromCsvAsync_ValidCsv_ReturnsFlightDataList()
        {
            // Arrange  
            string filePath = "testFile.csv";
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,ABC123,Boeing,FL123,JFK,2023-10-01T10:00:00,LAX,2023-10-01T14:00:00";
            await File.WriteAllTextAsync(filePath, csvContent);

            // Act  
            List<FlightData> result = await _flightDataService.ReadFlightsFromCsvAsync(filePath);

            // Assert  
            Assert.Single(result);
            Assert.Equal(1, result[0].id);
            Assert.Equal("ABC123", result[0].aircraft_registration_number);

            // Cleanup  
            File.Delete(filePath);
        }

        [Fact]
        public async Task ValidateAndWriteCsvWithErrorsAsync_InvalidCsv_WritesErrorFile()
        {
            // Arrange  
            string inputFilePath = "testFile.csv";
            string outputFilePath = "errorFile.csv";
            string csvContent = "id,aircraft_registration_number,aircraft_type,flight_number,departure_airport,departure_datetime,arrival_airport,arrival_datetime\n" +
                                "1,,Boeing,FL123,JFK,InvalidDate,LAX,2023-10-01T14:00:00";
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

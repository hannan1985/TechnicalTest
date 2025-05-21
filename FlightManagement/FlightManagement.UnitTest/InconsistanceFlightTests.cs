using FlightManagement.Common.DTO;
using FlightManagement.Services.ApiServices;
using Newtonsoft.Json;

namespace FlightManagement.Test
{
    public class InconsistanceFlightTester
    {
        [Fact]
        public async void GetInconsistentFlightsFromJson_ReturnsInconsistencies()
        {
            // Arrange
            var flights = new List<FlightData>
        {
            new FlightData
            {
                flight_number = "AY123",
                departure_airport = "HEL",
                arrival_airport = "LHR",
                departure_datetime = new DateTime(2024, 6, 1, 8, 0, 0)
            },
            new FlightData
            {
                flight_number = "AY123",
                departure_airport = "LHR",
                arrival_airport = "CDG",
                departure_datetime = new DateTime(2024, 6, 1, 12, 0, 0)
            },
            new FlightData
            {
                flight_number = "AY123",
                departure_airport = "HEL", // Inconsistent: should be CDG
                arrival_airport = "FRA",
                departure_datetime = new DateTime(2024, 6, 1, 18, 0, 0)
            }
        };
            string json = JsonConvert.SerializeObject(flights);

            var service = new FlightScheduleCheckerService();

            // Act
            string resultJson = await service.GetInconsistentFlights(json);
            var inconsistencies = JsonConvert.DeserializeObject<List<FlightData>>(resultJson);

            // Assert
            Assert.Single(inconsistencies);
            Assert.Equal("HEL", inconsistencies[0].departure_airport);
            Assert.Equal("AY123", inconsistencies[0].flight_number);
        }
    }
}
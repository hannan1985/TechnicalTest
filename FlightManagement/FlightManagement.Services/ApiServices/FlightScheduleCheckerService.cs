using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagement.Common.DTO;
using Newtonsoft.Json;

namespace FlightManagement.Services.ApiServices
{
    /// <summary>
    /// Service for checking flight schedule inconsistencies.
    /// </summary>
    public class FlightScheduleCheckerService : IFlightScheduleCheckerService
    {

        /// <summary>
        /// Analyzes flight data for schedule inconsistencies based on mismatches 
        /// between arrival and subsequent departure airports for the same flight number.
        /// </summary>
        /// <param name="jsonData">The JSON string containing a list of flight records.</param>
        /// <returns>A JSON string of inconsistent flight records.</returns>
        public async Task<string> GetInconsistentFlights(string jsonData)
        {

            List<FlightData> lstFlightData = JsonConvert.DeserializeObject<List<FlightData>>(jsonData) ?? new();
            // Ensure flights are ordered by aircraft and time (assuming you have a date/time property, e.g., departure_time)
            lstFlightData = lstFlightData.OrderBy(f => f.flight_number)
                                         .ThenBy(f => f.departure_datetime)
                                         .ToList();



            List<string> lstFlightNumber = lstFlightData.Select(f => f.flight_number).Distinct().ToList();
            List<FlightData> lstInconsistencies = new();
            foreach (var flightNumber in lstFlightNumber)
            {
                var lstSpecificFlights = lstFlightData.Where(f => f.flight_number == flightNumber).OrderBy(f => f.departure_datetime).ToList();
                FlightData? previousFlight = null;

                foreach (var flight in lstSpecificFlights)
                {
                    if (previousFlight != null)
                    {
                        if (!string.Equals(previousFlight.arrival_airport, flight.departure_airport, StringComparison.OrdinalIgnoreCase))
                        {
                            lstInconsistencies.Add(flight);
                        }
                    }
                    previousFlight = flight;
                }
            }

            string finalMessage = string.Empty;

            if (lstInconsistencies.Count() > 0)
            {
                finalMessage = JsonConvert.SerializeObject(lstInconsistencies, Formatting.Indented);
            }
            else
            {
                finalMessage = "No inconsistent flights found.";
            }


            return finalMessage;
        }
    }


    public interface IFlightScheduleCheckerService
    {
        Task<string> GetInconsistentFlights(string jsonData);
    }

}

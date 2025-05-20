using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightManagement.Common.Constant;
using FlightManagement.Common.DTO;
using FlightManagement.Common.Enums;
using FlightManagement.Common.OperationDTO;
using Microsoft.Extensions.Configuration;


namespace FlightManagement.Services.ApiServices
{

    /// <summary>
    /// Service class for managing flight data from CSV files.
    /// </summary>
    public class FlightDataService : IFlightDataService
    {

        /// <summary>
        /// Reads a JSON file from the specified location.
        /// </summary>
        /// <param name="fileLocation">The full file path of the JSON file.</param>
        /// <returns>A string containing the contents of the JSON file.</returns>
        public async Task<string> GetFlightData(string fileLocation)
        {
            string jsonData = await System.IO.File.ReadAllTextAsync(fileLocation);

            return jsonData;
        }

        /// <summary>
        /// Processes a CSV file by reading it, validating the data, converting it to JSON,
        /// and saving the result to a file.
        /// </summary>
        /// <param name="fileLocation">The full file path of the CSV file.</param>
        /// <returns>A response message indicating success or failure.</returns>
        public async Task<ResponseMessage> ProcessFlightData(string fileLocation)
        {
            ResponseMessage objResponseMessage = new ResponseMessage();
            string fileSaveLocation = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "jsonData.txt";

            try
            {
                var csvData = await ReadFlightsFromCsvAsync(fileLocation);

                // Convert the list of string arrays to a JSON string
                string jsonString = JsonSerializer.Serialize(csvData);

                // Store the JSON string in memory
                System.IO.File.WriteAllText(fileSaveLocation, jsonString);

                objResponseMessage.ResponseCode = (int)AppEnums.StatusCode.SUCCESS;
                objResponseMessage.Message = AppConstants.Message.CSVProcessedSuccessfully;
            }
            catch (Exception ex)
            {
                objResponseMessage.ResponseCode = (int)AppEnums.StatusCode.ERROR;
                objResponseMessage.Message = AppConstants.Message.CSVProcessError + ex.Message;
            }

            return objResponseMessage;
        }


        /// <summary>
        /// Reads flight records from a CSV file and returns them as a list of FlightData objects.
        /// Throws an exception if validation fails.
        /// </summary>
        /// <param name="filePath">The CSV file path.</param>
        /// <returns>A list of valid flight data records.</returns>
        public async Task<List<FlightData>> ReadFlightsFromCsvAsync(string filePath)
        {
            List<FlightData> lstFlightData = new List<FlightData>();

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist.");
            }
            string outputFilePath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "errorData.csv";

            if (!await ValidateAndWriteCsvWithErrorsAsync(filePath, outputFilePath))
            {
                throw new FileNotFoundException(AppConstants.Message.InvalidRecordsFound + outputFilePath);
            }

            using (var reader = new StreamReader(filePath))
            {
                string? headerLine = await reader.ReadLineAsync(); // Read header

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');

                    // Adjust indexes based on your CSV structure
                    var flight = new FlightData();
                    flight.id = int.TryParse(values[0], out var idVal) ? idVal : throw new Exception("Invalid id");
                    flight.aircraft_registration_number = string.IsNullOrWhiteSpace(values[1]) ? throw new Exception("Missing registration number") : values[1];
                    flight.aircraft_type = string.IsNullOrWhiteSpace(values[2]) ? throw new Exception("Missing aircraft type") : values[2];
                    flight.flight_number = string.IsNullOrWhiteSpace(values[3]) ? throw new Exception("Missing flight number") : values[3];
                    flight.departure_airport = string.IsNullOrWhiteSpace(values[4]) ? throw new Exception("Missing departure airport") : values[4];
                    flight.departure_datetime = DateTime.TryParse(values[5], out var depDt) ? depDt : throw new Exception("Invalid departure datetime");
                    flight.arrival_airport = string.IsNullOrWhiteSpace(values[6]) ? throw new Exception("Missing arrival airport") : values[6];
                    flight.arrival_datetime = DateTime.TryParse(values[7], out var arrDt) ? arrDt : throw new Exception("Invalid arrival datetime");

                    lstFlightData.Add(flight);

                }
            }
            return lstFlightData;
        }


        /// <summary>
        /// Validates each row in a CSV file and writes invalid records to a new file with error messages.
        /// </summary>
        /// <param name="inputFilePath">The input CSV file path.</param>
        /// <param name="outputFilePath">The output CSV file path for invalid rows.</param>
        /// <returns>True if all rows are valid; otherwise, false.</returns>
        public async Task<bool> ValidateAndWriteCsvWithErrorsAsync(string inputFilePath, string outputFilePath)
        {
            bool isValid = true;

            if (!System.IO.File.Exists(inputFilePath))
                throw new FileNotFoundException($"The file {inputFilePath} does not exist.");

            using var reader = new StreamReader(inputFilePath);
            using var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);

            string? headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
                throw new Exception(AppConstants.Message.CSVFileIsEmpty);

            // Write header with ErrorMessage column
            await writer.WriteLineAsync(headerLine + ",ErrorMessage");

            int lineNumber = 1;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    await writer.WriteLineAsync(",".PadLeft(headerLine.Split(',').Length, ',') + "Empty line");
                    continue;
                }

                var values = line.Split(',');
                string errorMessage = ValidateCsvRow(values);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    isValid = false;
                    // Write the original row plus the error message
                    await writer.WriteLineAsync(line + $",{errorMessage}");
                }

            }

            return isValid;
        }


        /// <summary>
        /// Validates a single row of CSV data based on expected field formats.
        /// </summary>
        /// <param name="values">The array of string values from a CSV row.</param>
        /// <returns>An error message if validation fails; otherwise, an empty string.</returns>
        private string ValidateCsvRow(string[] values)
        {
            if (values.Length != 8)
                return "Invalid column count";

            if (!int.TryParse(values[0], out _))
                return "Invalid id";
            if (string.IsNullOrWhiteSpace(values[1]))
                return "Missing registration number";
            if (string.IsNullOrWhiteSpace(values[2]))
                return "Missing aircraft type";
            if (string.IsNullOrWhiteSpace(values[3]))
                return "Missing flight number";
            if (string.IsNullOrWhiteSpace(values[4]))
                return "Missing departure airport";
            if (!DateTime.TryParse(values[5], out _))
                return "Invalid departure datetime";
            if (string.IsNullOrWhiteSpace(values[6]))
                return "Missing arrival airport";
            if (!DateTime.TryParse(values[7], out _))
                return "Invalid arrival datetime";

            return ""; // No error
        }

    }


    public interface IFlightDataService
    {
        Task<ResponseMessage> ProcessFlightData(string fileLocation);
        Task<string> GetFlightData(string fileLocation);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.Constant
{
    public class AppConstants
    {
        public static class Message
        {
            public const string CSVProcessedSuccessfully = "CSV processed successfully.";
            public const string CSVProcessError = "Error processing CSV:";
            public const string InvalidRecordsFound = "This file has invalid records. Moreover, the error message is also mentioned there, Please check ";
            public const string FileLocationNotFoundInConfiguration = "File location not found in configuration.";
            public const string FileLocationNotConfigured = "File location is not configured.";
            public const string NoJsonDataFoundForInconsistancyCheck = "No data found for the inconsistency check";
            public const string NoJsonDataFound = "No data found";
            public const string CSVFileIsEmpty = "CSV file is empty.";
            public const string UnexpectedError = "An unexpected error occurred.";
            public const string NoInconsistentFlightFound = "No inconsistent flights found.";
            public const string InvalidColumnCount = "Invalid column count";
            public const string InvalidId = "Invalid flight id";
            public const string MissingRegistrationNumber = "Missing registration number";
            public const string MissingAircraftType = "Missing aircraft type";
            public const string MissingFlightNumber = "Missing flight number";
            public const string MissingDepartureAirport = "Missing departure airport";
            public const string InvalidDepartureDatetime = "Invalid departure datetime";
            public const string MissingArrivalAirport = "Missing arrival airport";
            public const string InvalidArrivalDatetime = "Invalid arrival datetime";
            public const string NoCSVFilesFound = "No CSV files found in: ";


        }
    }
}

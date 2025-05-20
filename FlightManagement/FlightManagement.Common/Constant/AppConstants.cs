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
            public const string NoJsonDataFoundForInconsistancyCheck = "No jason data found for inconsistancy check";
            public const string NoJsonDataFound = "No jason data found";
            public const string CSVFileIsEmpty = "CSV file is empty.";
            public const string UnexpectedError = "An unexpected error occurred.";

        }
    }
}

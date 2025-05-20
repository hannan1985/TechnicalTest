using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.DTO
{
    public class FlightData
    {
        public int id { get; set; }

        public string aircraft_registration_number { get; set; }

        public string aircraft_type { get; set; }

        public string flight_number { get; set; }

        public string departure_airport { get; set; }

        public DateTime departure_datetime { get; set; }

        public string arrival_airport { get; set; }

        public DateTime arrival_datetime { get; set; }
        public string ErrorMessage { get; set; }
    }
}

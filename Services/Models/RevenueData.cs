using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class Revenue
    {
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public class PaymentDayData
    {
        public float DayOfWeek { get; set; }
        public float Month { get; set; }
        public float Year { get; set; }
        public float PaymentAmount { get; set; }
    }
    
    public class PaymentMonthData
    {
        public float Month { get; set; }
        public float Year { get; set; }
        public float PaymentAmount { get; set; }
    }
    
    public class PaymentYearData
    {
        public float Year { get; set; }
        public float PaymentAmount { get; set; }
    }

    public class PaymentPrediction
    {
        [ColumnName("Score")]
        public float PredictedAmount { get; set; }
    }
}

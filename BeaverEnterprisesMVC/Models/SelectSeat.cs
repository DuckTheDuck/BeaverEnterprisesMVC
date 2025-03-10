using Microsoft.AspNetCore.Mvc;

namespace BeaverEnterprisesMVC.Models
{
    public class SeatPosition
    {
        public int Number { get; set; } 
        public char Status { get; set; }  

        public SeatPosition(int Number, char Status)
        {
            this.Number = Number;
            this.Status = Status;
        }

    }
}

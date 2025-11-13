namespace LibraryManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalBorrowers { get; set; }
        public int ActiveBorrows { get; set; }
        public int OverdueBooks { get; set; }
    }
}

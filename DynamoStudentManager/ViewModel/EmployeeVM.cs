namespace DynamoStudentManager.ViewModel
{
    public class EmployeeVM
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public int? Technology_ID { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
    }
}

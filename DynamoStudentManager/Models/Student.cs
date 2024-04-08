using Amazon.DynamoDBv2.DataModel;

namespace DynamoStudentManager.Models
{
    [DynamoDBTable("student")]
    public class Student
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("first_name")]
        public string? FirstName { get; set; }

        [DynamoDBProperty("last_name")]
        public string? LastName { get; set; }

        [DynamoDBProperty("class")]
        public int Class { get; set; }

        [DynamoDBProperty("country")]
        public string? Country { get; set; }
    }

    [DynamoDBTable("country")]
    public class Country
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("name")]
        public string? Name { get; set; }

        [DynamoDBProperty("country_code")]
        public string? CountryCode { get; set; }
    }



    [DynamoDBTable("states")]
    public class State
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("country_id")]
        public int? CountryId { get; set; }

        [DynamoDBProperty("name")]
        public string? Name { get; set; }

    }
    [DynamoDBTable("cities")]
    public class City
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("state_id")]
        public int? StateId { get; set; }

        [DynamoDBProperty("country_id")]
        public int? CountryId { get; set; }

        [DynamoDBProperty("name")]
        public string? Name { get; set; }

    }


    [DynamoDBTable("employees")]
    public class Employee
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("name")]
        public string? Name { get; set; }

        [DynamoDBProperty("email")]
        public string? Email { get; set; }

        [DynamoDBProperty("phone")]
        public string? Phone { get; set; }

        [DynamoDBProperty("city")]
        public string? City { get; set; }

        [DynamoDBProperty("technology_id")]
        public int? Technology_ID { get; set; }

        [DynamoDBProperty("description")]
        public string? Description { get; set; }

        [DynamoDBProperty("file_name")]
        public string? FileName { get; set; }
    }
}

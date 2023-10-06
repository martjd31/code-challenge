using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange


            var compensation = new Compensation()
            {
                Employee = new Employee
                {
                    EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
                    Department = "Engineering",
                    FirstName = "Pete",
                    LastName = "Best",
                    Position = "Developer II",
                },
                EffectiveDate = new DateTime(2023, 10, 05),
                Salary = 85000.00,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(compensation.Employee.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        }

        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = new Employee
                {
                    EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
                    Department = "Engineering",
                    FirstName = "Pete",
                    LastName = "Best",
                    Position = "Developer II",
                },
                EffectiveDate = new DateTime(2023, 10, 05),
                Salary = 85000.00
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Add compensation 
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);


            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{compensation.Employee.EmployeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.Salary, createdCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, createdCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Employee.FirstName, createdCompensation.Employee.FirstName);
        }

        [TestMethod]
        public void GetCompensationById_Returns_NotFound()
        {
            // Arrange
            var employeeId = "339bb795-451a-4f95-a902-080a401409c5";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}

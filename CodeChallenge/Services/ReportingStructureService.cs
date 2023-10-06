using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services {
    public class ReportingStructureService : IReportingStructureService {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService (ILogger<ReportingStructureService> logger, IEmployeeRepository employeeRepository) {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }
        public ReportingStructure GetReportingStructureById (string id) {

            if (!String.IsNullOrEmpty (id)) {
                Employee employee = GetByEmployeeIdWithReports (id);

                if (employee == null) {
                    return null;
                }

                ReportingStructure reportingStructure = new ReportingStructure ();
                reportingStructure.NumberOfReports = FindNumberOfReports (employee);
                reportingStructure.Employee = employee;

                return reportingStructure;
            }

            return null;

        }

        private Employee GetByEmployeeIdWithReports (string employeeId) {
            if (!String.IsNullOrEmpty (employeeId)) {
                return _employeeRepository.GetByIdWithReports (employeeId);
            }

            return null;
        }

        private int FindNumberOfReports (Employee employee) {
            Stack<Employee> employeeStack = new Stack<Employee> ();
            int count = 0;

            employeeStack.Push (employee);

            while (employeeStack.Count != 0) {
                Employee boss = employeeStack.Pop ();

                if (boss.DirectReports != null) {
                    foreach (var reportee in boss.DirectReports) {
                        employeeStack.Push (GetByEmployeeIdWithReports (reportee.EmployeeId));
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
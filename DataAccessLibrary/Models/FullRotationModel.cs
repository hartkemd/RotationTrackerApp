﻿using RotationLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class FullRotationModel
    {
        public BasicRotationModel BasicInfo { get; set; } = new ();
        public ObservableCollection<EmployeeModel> RotationOfEmployees { get; set; } = new ();
        public ObservableCollection<CoverageModel> Coverages { get; set; } = new ();
        public ObservableCollection<CoverageReadModel> CoveragesDisplay { get; set; } = new();

        public string CurrentEmployeeName => GetCurrentEmployeesName();

        public bool AnEmployeeIsOnCalendar()
        {
            bool output = false;

            foreach (EmployeeModel employee in RotationOfEmployees)
            {
                if (employee.OnCalendar == true)
                {
                    output = true;
                    break;
                }
            }

            return output;
        }

        public bool AllEmployeesAreOnCalendar()
        {
            bool output = false;

            foreach (EmployeeModel employee in RotationOfEmployees)
            {
                if (employee.OnCalendar == false)
                {
                    return output;
                }
            }

            output = true;
            return output;
        }

        private string GetCurrentEmployeesName()
        {
            if (RotationOfEmployees.Count > 0)
            {
                return RotationOfEmployees[0].FullName;
            }
            else
            {
                return null;
            }
        }

        public void PopulateNextStartDateTimesOfEmployees()
        {
            if (RotationOfEmployees.Count > 0)
            {
                if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
                {
                    for (int i = 1; i < RotationOfEmployees.Count; i++) // skip the currently up employee, until further down
                    {
                        if (i == 1)
                        {
                            RotationOfEmployees[i].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances;
                        }
                        else
                        {
                            RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddDays(7);
                        }
                    }

                    RotationOfEmployees[0].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances.AddDays(-7);
                }
                else if (BasicInfo.RotationRecurrence == RecurrenceInterval.WeeklyWorkWeek)
                {
                    for (int i = 0; i < RotationOfEmployees.Count; i++)
                    {
                        if (i == 0)
                        {
                            RotationOfEmployees[i].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances.AddDays(-4);
                        }
                        else
                        {
                            RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddDays(7);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < RotationOfEmployees.Count; i++)
                    {
                        if (i == 0)
                        {
                            RotationOfEmployees[i].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances;
                        }
                        else
                        {
                            switch (BasicInfo.RotationRecurrence)
                            {
                                case RecurrenceInterval.BiweeklyOnDay:
                                    RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddDays(14);
                                    break;
                                case RecurrenceInterval.MonthlyOnDay:
                                    RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddMonths(1);
                                    break;
                                case RecurrenceInterval.BimonthlyOnDay:
                                    RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddMonths(2);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void PopulateNextEndDateTimesOfEmployees()
        {
            if (RotationOfEmployees.Count > 0)
            {
                for (int i = 0; i < RotationOfEmployees.Count; i++)
                {
                    if (i == 0)
                    {
                        if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
                        {
                            RotationOfEmployees[i].NextEndDateTime = BasicInfo.NextDateTimeRotationAdvances.AddMinutes(-1);
                        }
                        else if (BasicInfo.RotationRecurrence == RecurrenceInterval.WeeklyWorkWeek)
                        {
                            RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i].NextStartDateTime.AddDays(4);
                        }
                        else if (BasicInfo.RotationRecurrence == RecurrenceInterval.BiweeklyOnDay)
                        {
                            RotationOfEmployees[i].NextEndDateTime = BasicInfo.NextDateTimeRotationAdvances;
                        }
                    }
                    else
                    {
                        switch (BasicInfo.RotationRecurrence)
                        {
                            case RecurrenceInterval.Weekly:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddDays(7);
                                break;
                            case RecurrenceInterval.WeeklyWorkWeek:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddDays(7);
                                break;
                            case RecurrenceInterval.BiweeklyOnDay:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddDays(14);
                                break;
                        }
                    }
                }
            }
        }

        public void AdvanceRotation()
        {
            if (RotationOfEmployees.Count > 0)
            {
                EmployeeModel employeeWhoWent = RotationOfEmployees[0];
                RotationOfEmployees.RemoveAt(0);
                RotationOfEmployees.Add(employeeWhoWent);
            }
        }

        public void ReverseRotation()
        {
            if (RotationOfEmployees.Count > 0)
            {
                int index = RotationOfEmployees.Count - 1;
                EmployeeModel employeeToPutFirst = RotationOfEmployees[index];
                RotationOfEmployees.RemoveAt(index);
                RotationOfEmployees.Insert(0, employeeToPutFirst);
            }
        }

        public void SetNextDateTimeRotationAdvances()
        {
            if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(7);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.WeeklyWorkWeek)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(7);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.BiweeklyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(14);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.MonthlyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(1);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.BimonthlyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(2);
            }
        }
    }
}

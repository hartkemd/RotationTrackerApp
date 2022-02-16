﻿using DataAccessLibrary.Models;
using RotationLibrary;
using RotationTracker.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for EditRotation.xaml
    /// </summary>
    public partial class EditRotation : Window
    {
        private MainWindow _parentWindow;
        private RotationUIModel _rotationUIModel;
        private FullRotationModel _rotation;
        private ListBox _listBox;
        private Label _rotationNameLabel;
        private TextBlock _currentEmployeeTextBlock;
        private RotationChangedUIModel _rotationChangedUIModel = new ();

        public EditRotation(MainWindow parentWindow, RotationUIModel rotationUIModel)
        {
            InitializeComponent();

            _parentWindow = parentWindow;
            _rotationUIModel = rotationUIModel;
            _rotation = _rotationUIModel.FullRotationModel;
            _listBox = _rotationUIModel.RotationListBox;
            _rotationNameLabel = _rotationUIModel.RotationNameLabel;
            _currentEmployeeTextBlock = _rotationUIModel.CurrentEmployeeTextBlock;

            PopulateControls();
            _rotationChangedUIModel.RotationChanged += RotationChangedUIModel_RotationChanged;
        }

        private void RotationChangedUIModel_RotationChanged(object sender, string e)
        {
            UncheckAllOnCalendarInListView();
            messageTextBlock.Text = e;
        }

        private void UncheckAllOnCalendarInListView()
        {
            foreach (var item in employeeListView.Items)
            {
                EmployeeModel employee = (EmployeeModel)item;
                employee.OnCalendar = false;
                _parentWindow.UpdateOnCalendar(_rotation.BasicInfo, employee, false);
            }
        }

        private void PopulateControls()
        {
            rotationNameLabel.Content = $"{_rotation.BasicInfo.RotationName}:";
            employeeListView.ItemsSource = _rotation.RotationOfEmployees;
            rotationNameTextBox.Text = _rotation.BasicInfo.RotationName;
            notesTextBox.Text = _rotation.BasicInfo.Notes;
            GetRotationRecurrence();

            if (_rotation.BasicInfo.NextDateTimeRotationAdvances == DateTime.MinValue)
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = DateTime.Today;
            }
            else
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = _rotation.BasicInfo.NextDateTimeRotationAdvances;
            }

            hourRotationAdvancesTextBox.Text = _rotation.BasicInfo.NextDateTimeRotationAdvances.Hour.ToString();

            if (_rotation.BasicInfo.AdvanceAutomatically == true)
            {
                advanceAutomaticallyCheckBox.IsChecked = true;
            }
            else
            {
                advanceAutomaticallyCheckBox.IsChecked = false;
            }
        }

        private void GetRotationRecurrence()
        {
            switch (_rotation.BasicInfo.RotationRecurrence)
            {
                case RecurrenceInterval.Weekly:
                    weeklyRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.WeeklyWorkWeek:
                    weeklyWorkWeekRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.BiweeklyOnDay:
                    biweeklyRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.MonthlyOnDay:
                    monthlyRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.BimonthlyOnDay:
                    bimonthlyRadioButton.IsChecked = true;
                    break;
            }
        }

        private void RefreshListBoxes()
        {
            _rotation.PopulateNextStartDateTimesOfEmployees();
            _rotation.PopulateNextEndDateTimesOfEmployees();
            employeeListView.RefreshContents(_rotation.RotationOfEmployees);
            _listBox.RefreshContents(_rotation.RotationOfEmployees);
        }

        private void SetRotationRecurrence()
        {
            if (weeklyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Weekly;
            }
            else if (weeklyWorkWeekRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.WeeklyWorkWeek;
            }
            else if (biweeklyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.BiweeklyOnDay;
            }
            else if (monthlyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.MonthlyOnDay;
            }
            else if (bimonthlyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.BimonthlyOnDay;
            }
        }

        private bool SetNextDateTimeRotationAdvances()
        {
            bool dateTimeSetSuccessfully = true;

            if (nextDateRotationAdvancesDatePicker.SelectedDate.HasValue)
            {
                _rotation.BasicInfo.NextDateTimeRotationAdvances = nextDateRotationAdvancesDatePicker.SelectedDate.Value;
                if (string.IsNullOrWhiteSpace(hourRotationAdvancesTextBox.Text) == false)
                {
                    bool isValidInt = int.TryParse(hourRotationAdvancesTextBox.Text, out int hoursToAdd);
                    if (isValidInt && hoursToAdd >= 0 && hoursToAdd <= 23)
                    {
                        _rotation.BasicInfo.NextDateTimeRotationAdvances = _rotation.BasicInfo.NextDateTimeRotationAdvances.AddHours(hoursToAdd);
                    }
                    else
                    {
                        WPFHelper.ShowErrorMessageBoxAndResetTextBox("That was not a valid number of hours.",
                            hourRotationAdvancesTextBox);
                        dateTimeSetSuccessfully = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("No date was selected.");
                dateTimeSetSuccessfully = false;
            }

            return dateTimeSetSuccessfully;
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListView.SelectedIndex;

            if (selectedIndex > 0)
            {
                _rotation.RotationOfEmployees.Insert(selectedIndex - 1, (EmployeeModel)employeeListView.Items[selectedIndex]);
                _rotation.RotationOfEmployees.RemoveAt(selectedIndex + 1);

                RefreshListBoxes();
                employeeListView.SelectedIndex = selectedIndex - 1;
                _rotationChangedUIModel.RotationHasChanged = true;
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListView.SelectedIndex;

            if (selectedIndex < employeeListView.Items.Count - 1 && selectedIndex != -1)
            {
                _rotation.RotationOfEmployees.Insert(selectedIndex + 2, (EmployeeModel)employeeListView.Items[selectedIndex]);
                _rotation.RotationOfEmployees.RemoveAt(selectedIndex);

                RefreshListBoxes();
                employeeListView.SelectedIndex = selectedIndex + 1;
                _rotationChangedUIModel.RotationHasChanged = true;
            }
        }

        private void CopyEmployeesToRotation_Click(object sender, RoutedEventArgs e)
        {
            _rotation.RotationOfEmployees = _parentWindow.employees;
            employeeListView.RefreshContents(_rotation.RotationOfEmployees);
            _rotationChangedUIModel.RotationHasChanged = true;
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeListView.SelectedIndex != -1)
            {
                _rotation.RotationOfEmployees.Remove((EmployeeModel)employeeListView.SelectedItem);
                employeeListView.RefreshContents(_rotation.RotationOfEmployees);
                _rotationChangedUIModel.RotationHasChanged = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _listBox.RefreshContents(_rotation.RotationOfEmployees);

            _rotation.BasicInfo.RotationName = rotationNameTextBox.Text;
            _rotationNameLabel.Content = $"{_rotation.BasicInfo.RotationName}:";

            _currentEmployeeTextBlock.Text = $"Currently Up: {_rotation.CurrentEmployeeName}";

            _rotation.BasicInfo.Notes = notesTextBox.Text;
            _rotationUIModel.RotationNotesTextBox.Text = _rotation.BasicInfo.Notes;

            SetRotationRecurrence();
            SetRotationAdvanceAutomatically();

            bool keepGoing = SetNextDateTimeRotationAdvances();
            if (keepGoing == true)
            {
                _parentWindow.UpdateRotationBasicInfoInDB(_rotation.BasicInfo);
                _parentWindow.RecreateRotationInDB(_rotation);
                _parentWindow.PopulateRotationListBox(_rotation, _listBox);
                _rotationUIModel.DateTimeTextBlock.Text = $"{_rotation.BasicInfo.NextDateTimeRotationAdvances:g}";
                Close();
            }
        }

        private void SetRotationAdvanceAutomatically()
        {
            if (advanceAutomaticallyCheckBox.IsChecked == true)
            {
                _rotation.BasicInfo.AdvanceAutomatically = true;
            }
            else
            {
                _rotation.BasicInfo.AdvanceAutomatically = false;
            }
        }

        private void DisableAdvanceAutomaticallyCheckBox()
        {
            if (advanceAutomaticallyCheckBox != null)
            {
                advanceAutomaticallyCheckBox.IsChecked = false;
                advanceAutomaticallyCheckBox.IsEnabled = false;
            }
        }

        private void EnableAdvanceAutomaticallyCheckBox()
        {
            if (advanceAutomaticallyCheckBox != null)
            {
                advanceAutomaticallyCheckBox.IsChecked = true;
                advanceAutomaticallyCheckBox.IsEnabled = true;
            }
        }

        private void MonthlyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DisableAdvanceAutomaticallyCheckBox();
        }

        private void BimonthlyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DisableAdvanceAutomaticallyCheckBox();
        }

        private void WeeklyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableAdvanceAutomaticallyCheckBox();
        }

        private void BiweeklyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableAdvanceAutomaticallyCheckBox();
        }

        private void WeeklyWorkWeekRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            EnableAdvanceAutomaticallyCheckBox();
        }

        private void UpdateOnCalendar(object sender)
        {
            CheckBox checkBox = (CheckBox)sender;
            EmployeeModel employee = (EmployeeModel)checkBox.DataContext;
            _parentWindow.UpdateOnCalendar(_rotation.BasicInfo, employee, (bool)checkBox.IsChecked);
        }

        private void OnCalendarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateOnCalendar(sender);
        }

        private void OnCalendarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateOnCalendar(sender);
        }

        private void CreateCalendarEvents_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in employeeListView.Items)
            {
                EmployeeModel employee = (EmployeeModel)item;
                CreateCalendarEvent(employee);
            }
        }

        private void CreateCalendarEvent(EmployeeModel employee)
        {
            throw new NotImplementedException();
        }
    }
}

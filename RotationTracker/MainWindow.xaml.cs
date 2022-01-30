﻿using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using RotationLibrary;
using RotationLibrary.Models;
using RotationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISqliteData _db;
        public List<EmployeeModel> employees = new ();
        public List<FullRotationModel> rotations = new ();
        public List<RotationUIModel> rotationUIModels = new ();
        private string notificationMessage;
        List<string> admins = new();
        private string currentUser;
        private bool currentUserIsAdmin = false;

        public MainWindow(ISqliteData db)
        {
            InitializeComponent();
            _db = db;

            GetCurrentUser();
            WriteAdminsToFile();
            ReadAdminsFromFile();
            CheckIfCurrentUserIsAdmin();
            //ShowControlsIfUserIsAdmin();
            DisplayCurrentUser();

            ReadEmployees();

            PopulateControls();

            //AdvanceRotationsIfDateTimeHasPassed();

            DisplayNotificationsAsync();
            CreateTimer();
        }

        public void ReadEmployees()
        {
            employees = _db.GetAllEmployees();
        }

        public void CreateEmployee(string employeeName)
        {
            _db.CreateEmployee(employeeName);
        }

        public void DeleteEmployee(int id)
        {
            _db.DeleteEmployee(id);
        }

        private void CreateRotation(FullRotationModel rotation)
        {
            _db.CreateRotation(rotation);
        }

        private void WriteAdminsToFile()
        {
            admins.Add("Mark");
            //TextFileIO.WriteListToFile(configFilePath, admins);
        }

        private void ReadAdminsFromFile()
        {
            //admins = TextFileIO.ReadListFromFile(configFilePath);
        }

        private void ShowControlsIfUserIsAdmin()
        {
            if (currentUserIsAdmin == true)
            {
                editEmployeesButton.Visibility = Visibility.Visible;
                //advanceRotation1Button.Visibility = Visibility.Visible;
                //editRotation1Button.Visibility = Visibility.Visible;
            }
        }

        private void CheckIfCurrentUserIsAdmin()
        {
            foreach (string user in admins)
            {
                if (currentUser == user)
                {
                    currentUserIsAdmin = true;
                }
            }
        }

        private void GetCurrentUser()
        {
            currentUser = Environment.UserName;
        }

        private void DisplayCurrentUser()
        {
            userNameTextBlock.Text = currentUser;
        }

        private void CreateTimer()
        {
            System.Timers.Timer refreshAppTimer = new();
            refreshAppTimer.Interval = 15 * 60 * 1000; // 15 minutes
            //refreshAppTimer.Elapsed += new ElapsedEventHandler(RefreshAppTimer_Elapsed);
            refreshAppTimer.Start();
        }

        //private void RefreshAppTimer_Elapsed(object sender, EventArgs e)
        //{
        //    AdvanceRotationsIfDateTimeHasPassed();
        //}

        private async void DisplayNotificationsAsync()
        {
            if (notificationMessage != "")
            {
                notificationTextBlock.Text = notificationMessage;
                notificationStackPanel.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                notificationStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void PopulateControls()
        {
            employeeListBox.ItemsSource = employees;
        }

        //private void AdvanceRotationsIfDateTimeHasPassed()
        //{
        //    AdvanceRotationIfDateTimeHasPassed(rotation1, rotation1CurrentEmployeeTextBlock, rotation1ListBox);
        //}

        private void AdvanceRotationIfDateTimeHasPassed(RotationModel rotation, TextBlock textBlock, ListBox listBox)
        {
            if (rotation.NextDateTimeRotationAdvances != DateTime.MinValue)
            {
                DateTime now = DateTime.Now;

                if (now > rotation.NextDateTimeRotationAdvances)
                {
                    rotation.AdvanceRotation();
                    rotation.SetNextDateTimeRotationAdvances();
                    if (rotation.Rotation.Count > 0)
                    {
                        notificationMessage += $"{rotation.Rotation.Last()} took their turn for {rotation.RotationName} Rotation.\n";
                    }
                    textBlock.Text = rotation.CurrentEmployee;
                    listBox.RefreshContents(rotation.Rotation);
                    SaveRotation(rotation);
                }
            }
        }

        private static void SaveRotation(RotationModel rotation)
        {
            //rotation.SaveToJSON(rotation.FilePath, rotation.FileName);
        }

        private static void AdvanceRotationAndRefreshControls(RotationUIModel rotationUIModel)
        {
            rotationUIModel.FullRotationModel.AdvanceRotation();
            rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployee}";

            rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.FullRotationModel.RotationOfEmployees);
            //SaveRotation(rotation);
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);
            editEmployees.ShowDialog();
        }

        private void AdvanceRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            AdvanceRotationAndRefreshControls(rotationUIModel);
        }

        private void EditRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            EditRotation editRotation = new(this, rotationUIModel);
            editRotation.ShowDialog();
        }

        private void RemoveRotationButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRotation removeRotation = new(this);
            removeRotation.ShowDialog();
        }

        private void AddRotationButton_Click(object sender, RoutedEventArgs e)
        {
            RotationUIModel rotationUIModel = new ();

            FullRotationModel rotation = new ();
            rotation.BasicInfo.RotationName = $"Rotation {rotations.Count + 1}";
            rotation.BasicInfo.NextDateTimeRotationAdvances = DateTime.Now.AddDays(7).ToString();
            rotation.RotationOfEmployees = employees;

            rotationUIModel.FullRotationModel = rotation;
            rotations.Add(rotation);

            CreateRotation(rotation);

            GroupBox groupBox = new GroupBox();
            groupBox.Margin = new Thickness(5);
            Label label = new Label();
            label.Content = $"{rotation.BasicInfo.RotationName}:";
            rotationUIModel.RotationNameLabel = label;
            groupBox.Header = label;
            
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            ListBox listBox = new ListBox();
            listBox.Margin = new Thickness(5, 0, 5, 0);
            listBox.ItemsSource = rotation.RotationOfEmployees;
            listBox.DisplayMemberPath = "FullName";
            rotationUIModel.RotationListBox = listBox;

            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Vertical;
            stackPanel2.Margin = new Thickness(5);
            TextBlock currentlyUpTextBlock = new TextBlock();
            currentlyUpTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            currentlyUpTextBlock.Text = $"Currently Up: {rotation.CurrentEmployee}";
            rotationUIModel.CurrentEmployeeTextBlock = currentlyUpTextBlock;
            stackPanel2.Children.Add(currentlyUpTextBlock);

            StackPanel stackPanel3 = new StackPanel();
            stackPanel3.Orientation = Orientation.Horizontal;
            stackPanel3.HorizontalAlignment = HorizontalAlignment.Center;
            Button advanceButton = new Button();
            advanceButton.DataContext = rotationUIModel;
            advanceButton.Margin = new Thickness(0, 5, 0, 5);
            advanceButton.Width = 80;
            advanceButton.Content = "Advance";
            advanceButton.Click += AdvanceRotationButton_Click;
            Button editButton = new Button();
            editButton.DataContext = rotationUIModel;
            editButton.Margin = new Thickness(5);
            editButton.Width = 45;
            editButton.Content = "Edit";
            editButton.Click += EditRotationButton_Click;
            stackPanel3.Children.Add(advanceButton);
            stackPanel3.Children.Add(editButton);

            Label notesLabel = new Label();
            notesLabel.Content = "Notes:";
            TextBox notesTextBox = new TextBox();
            rotationUIModel.RotationNotesTextBox = notesTextBox;
            notesTextBox.IsReadOnly = true;
            notesTextBox.TextWrapping = TextWrapping.Wrap;
            notesTextBox.Height = 60;
            notesTextBox.MaxWidth = 215;
            notesTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            notesTextBox.Text = rotation.BasicInfo.Notes;

            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(stackPanel2);
            stackPanel.Children.Add(stackPanel3);
            stackPanel.Children.Add(notesLabel);
            stackPanel.Children.Add(notesTextBox);

            groupBox.Content = stackPanel;

            rotationUIModels.Add(rotationUIModel);
            rotationsWrapPanel.Children.Add(groupBox);
        }
    }
}

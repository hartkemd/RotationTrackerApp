﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WPFHelperLibrary
{
    public static class WPFHelper
    {
        public static void RefreshContents<T>(this ListBox listBox, List<T> list)
        {
            listBox.ItemsSource = null;
            listBox.ItemsSource = list;
        }

        public static void RefreshContents<T>(this ListBox listBox, ObservableCollection<T> observableCollection)
        {
            listBox.ItemsSource = null;
            listBox.ItemsSource = observableCollection;
        }

        public static void RefreshContents<T>(this ListView listView, ObservableCollection<T> observableCollection)
        {
            listView.ItemsSource = null;
            listView.ItemsSource = observableCollection;
        }

        public static MessageBoxResult ShowYesNoExclamationMessageBox(string message, string caption)
        {
            MessageBoxResult output;

            output = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            return output;
        }

        public static void ShowErrorMessageBoxAndResetTextBox(string message, TextBox textBox)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            textBox.Clear();
            textBox.Focus();
        }

        // template for TextBox validation
        public static void IfTextBoxTextIsValid(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) == false)
            {

            }
        }
    }
}

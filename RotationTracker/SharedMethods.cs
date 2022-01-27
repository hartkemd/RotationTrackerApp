﻿using RotationLibrary;
using RotationLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RotationTracker
{
    internal static class SharedMethods
    {
        public static void SetRotationControlsOnMainWindow(this RotationModel rotation,
            Label label, ListBox listBox, TextBlock textBlock, TextBox textBox)
        {
            label.Content = $"{rotation.RotationName} Rotation:";
            listBox.ItemsSource = rotation.Rotation;
            textBlock.Text = rotation.CurrentEmployee;
            textBox.Text = rotation.Notes;
        }
    }
}

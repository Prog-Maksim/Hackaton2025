﻿using System.Windows;
using Microsoft.Win32;
using report.Scripts;

namespace report;

public partial class AddFile : Window
{
    private string? fileNameEnergy;
    private string? fileNameConfigure;
    
    public AddFile()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (fileNameEnergy == null)
        {
            MessageBox.Show("Необходимо выбрать файл с почасовым потреблением энергии", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            fileNameEnergy = OpenDialogMenu("Выберите файл с потреблением энергии");
        }
        else if (fileNameConfigure == null)
        {
            MessageBox.Show("Необходимо выбрать конфигурационный файл", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            fileNameConfigure = OpenDialogMenu("Выберите файл конфигурации");
        }
        else
        {
            MainButton.IsEnabled = false;
            MainButton.Content = "Загрузка...";
            _ = Start(fileNameConfigure);
        }
    }

    private async Task Start(string filePath)
    {
        try
        {
            var result = await GetDataRequest.GetDataRequestMethod(filePath);

            double powerCost = 0;
            // double.TryParse(TextBox1.Text, out powerCost);

            double energyCost1 = 0, energyCost2 = 0, energyCost3 = 0;

            // Console.WriteLine(double.TryParse(TextBox2.Text, out energyCost1));
            // Console.WriteLine(double.TryParse(TextBox3.Text, out energyCost2));
            // Console.WriteLine(double.TryParse(TextBox4.Text, out energyCost3));

            MainWindow mainWindow = new MainWindow(result, powerCost, (energyCost1, energyCost2, energyCost3));
            mainWindow.Show();
            Close();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            MainButton.IsEnabled = true;
            MainButton.Content = "Произвести расчет";
        }
    }

    private void AddFileEnergy_OnClick(object sender, RoutedEventArgs e)
    {
        fileNameEnergy = OpenDialogMenu("Выберите файл с потреблением энергии");
    }

    private string? OpenDialogMenu(string title)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = title;
        openFileDialog.Filter = "Текстовые файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            return filePath;
        }
        
        return null;
    }

    private void AddFileConfig_OnClick(object sender, RoutedEventArgs e)
    {
        fileNameConfigure = OpenDialogMenu("Выберите файл конфигурации");
    }
}
using System.Windows;
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
        if (fileNameEnergy != null)
            _ = Start(fileNameEnergy);
        else
        {
            var result = OpenDialogMenu("Выберите файл с потреблением энергии");
            if (result != null)
                _ = Start(result);
            else
                MessageBox.Show("Требуется указать путь к файлу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private async Task Start(string filePath)
    {
        var result = await GetDataRequest.GetDataRequestMethod(filePath);

        double powerCost = 0;
        double.TryParse(TextBox1.Text, out powerCost);

        double energyCost1 = 0, energyCost2 = 0, energyCost3 = 0;
        
        double.TryParse(TextBox2.Text, out powerCost);
        double.TryParse(TextBox3.Text, out powerCost);
        double.TryParse(TextBox4.Text, out powerCost);
        
        MainWindow mainWindow = new MainWindow(result, powerCost, (energyCost1, energyCost2, energyCost3));
        mainWindow.Show();
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

    private void ChangeMode_OnSelected(object sender, RoutedEventArgs e)
    {
        if (ChangeMode.SelectedIndex == 0)
        {
            TextBlock1.Visibility = Visibility.Collapsed;
            TextBox1_1.Visibility = Visibility.Collapsed;
        }
        else if (ChangeMode.SelectedIndex == 1)
        {
            
        }
    }
}
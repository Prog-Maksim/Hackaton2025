using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace report;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public class PriceData
    {
        public required string PriceCategory { get; set; }
        public required string Price { get; set; }
        public string? Tag { get; set; }
    }
    
    private ObservableCollection<PriceData> prices = new ();
    
    public MainWindow()
    {
        InitializeComponent();

        List<PriceData> datas = new()
        {
            new PriceData { PriceCategory = "Ценовая категория 1", Price = "100"},
            new PriceData { PriceCategory = "Ценовая категория 2", Price = "125"},
            new PriceData { PriceCategory = "Ценовая категория 3", Price = "99", Tag = "Выгоднее всего"},
            new PriceData { PriceCategory = "Ценовая категория 4", Price = "130"},
            new PriceData { PriceCategory = "Ценовая категория 5", Price = "170"},
            new PriceData { PriceCategory = "Ценовая категория 6", Price = "199"},
        };

        AddPriceData(datas);
    }
    
    public void AddPriceData(List<PriceData> priceData)
    {
        foreach (var data in priceData)
            prices.Add(new PriceData { PriceCategory = data.PriceCategory, Price = data.Price + '\u20bd', Tag = data.Tag });
        
        PriceCategory.ItemsSource = prices;
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kurs
{
    public partial class PersonalAccount : Window
    {
        ObservableCollection<OrderObject> listGrid;

        class OrderObject 
        {
            public string Type { get; set; }
            public double Cost { get; set; }
            public int Workers { get; set; }
            public DateTime Finish { get; set; }
        }

        public PersonalAccount(Acc acc)
        {
            InitializeComponent();
            string? name = acc.Login;
            Label.Content = $"Welcome back \"{name}\"!";
            int IDClient = MainWindow.DataBase.Client.Where(x => x.Name == name).First().ID;
            var sourse = MainWindow.DataBase.Order.Where(x => x.ID_Client == IDClient).ToList();
            
            listGrid = new ObservableCollection<OrderObject>();
            foreach (var item in sourse)
                listGrid.Add(new OrderObject()
                {
                    Type = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                    Cost = item.Cost,
                    Finish = item.Order_date.AddDays(item.Total_time),
                    Workers = item.ID_Special.Split(' ').Count() - 1
                });

            DataGrid.ItemsSource = listGrid;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace kurs.Pages
{
    /// <summary>
    /// Interaction logic for SelectOrder.xaml
    /// </summary>
    public partial class SelectOrder : Page
    {
        private ObservableCollection<string> workersList;
        private double time = 0, cost = 0;
        private string workerDiscription = "";

        public SelectOrder()
        {
            InitializeComponent();

            var typeWork = new ObservableCollection<string>(MainWindow.DataBase.Serves.Select(x => x.Name));
            var itemSourse = new ObservableCollection<string>();
            workersList = new ObservableCollection<string>();

            Services.ItemsSource = typeWork;
            Specialist.ItemsSource = itemSourse;
            WorkersList.ItemsSource = workersList;

            Services.SelectionChanged += (o, e) =>
            {
                var job = MainWindow.DataBase.Serves.Where(x => x.ID == Services.SelectedIndex).Select(x => x.ID_job).ToList();
                var workers = MainWindow.DataBase.Special.Where(x => x.ID_job == job[0]).ToList();
                itemSourse.Clear();

                onChangeText();

                var allWorkers = new List<int>();
                var sourseWorkers = MainWindow.DataBase.Order.Select(x => x.ID_Special).ToList();
                foreach (var item in sourseWorkers)
                    foreach (var item2 in item.Split(' '))
                        if (item2 != "")
                            allWorkers.Add(Convert.ToInt32(item2));

                foreach (var item in workers)
                    if (!allWorkers.Contains(item.ID))
                        itemSourse.Add(item.Name);
            };

            if(ClientWindow.Order != null) 
            {
                Services.SelectedIndex = ClientWindow.Order.ID_servese;
                
                string[] workersDraft = ClientWindow.Order.ID_Special.Split(' ');
                foreach (var worker in workersDraft)
                    if (worker != "")
                        workersList.Add(MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(worker)).Select(x => x.Name).ToArray()[0]);
            }

            onChangeText();
        }

        private void onChangeText() 
        {
            workerDiscription = "";
            try
            {
                time = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Services.SelectedItem).Select(x => x.Time_to_done).ToArray()[0];
                var costTemp = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Services.SelectedItem).Select(x => x.Cost).ToList();
                cost = costTemp[0] * workersList.Count;

                for (int i = 0; i < workersList.Count; i++)
                    time -= time * 0.15;

                foreach (var worker in workersList)
                    workerDiscription += worker + "\n";
            }
            catch { }

            TextDiscription.Text = $"Type of work: {(string)Services.SelectedItem}\n\nYour team: \n{workerDiscription}\nTotal time: {time}\nTotal cost: {cost}";
        }

        private void Next(object sender, SelectionChangedEventArgs e) 
        {
            if (cost != 0 && time != 0 && workerDiscription != "")
            {
                var workers = workerDiscription.Split('\n');
                string workersID = "";

                foreach (var worker in workers)
                    if (worker != "")
                        workersID += MainWindow.DataBase.Special.Where(x => x.Name == worker).Select(x => x.ID).ToArray()[0] + " ";

                int id = 1;
                try { id = MainWindow.DataBase.Order.Max(x => x.ID) + 1; } 
                catch { }

                ClientWindow.Order = new Order() {
                    ID = id,
                    Cost = cost,
                    Total_time = time,
                    ID_Special = workersID,
                    ID_Client = ClientWindow.Client.ID,
                    ID_servese = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Services.SelectedItem).Select(x => x.ID).ToArray()[0],
                    Order_date = DateTime.Now
                };
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var worker in workersList)
                if (worker == (string)Specialist.SelectedItem)
                    return;

            workersList.Add((string)Specialist.SelectedItem);
            onChangeText();
            Next(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClientWindow.Order = null;
            workersList.Clear();
            onChangeText();
            Next(null, null);
        }
    }
}

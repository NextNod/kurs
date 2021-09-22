using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace kurs
{
    public partial class AdminPanel : Window
    {
        private readonly string[] allTables = new string[] { "Clients", "Orders", "Workers", "Jobs", "Servises" };
        private ObservableCollection<string> tables;
        private int ID;

        public AdminPanel(int ID)
        {
            InitializeComponent();
            this.ID = ID;
            tables = new ObservableCollection<string>();
            SnackAdmin.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            Tables.ItemsSource = tables;

            if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.SysAdmin == 1).ToArray()[0])
            {
                foreach (string tableName in allTables)
                    tables.Add(tableName);
            }
            else
            {
                AccountButton.Visibility = Visibility.Hidden;
                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Job).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[3]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Order).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[1]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Serves).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[4]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Special).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[2]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Client).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[0]);
            }
        }

        private void TablesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isAdmin = MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.SysAdmin == 1).ToArray()[0];

            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    DataGrid.ItemsSource = MainWindow.DataBase.Client.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Client).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;

                case "Orders":
                    DataGrid.ItemsSource = MainWindow.DataBase.Order.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Order).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;

                case "Workers":
                    DataGrid.ItemsSource = MainWindow.DataBase.Special.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Special).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;

                case "Servises":
                    DataGrid.ItemsSource = MainWindow.DataBase.Serves.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Serves).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;

                case "Jobs":
                    DataGrid.ItemsSource = MainWindow.DataBase.job.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Job).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;
            }
        }

        private void DataGrid_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {
            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    e.NewItem = new Client()
                    {
                        ID = MainWindow.DataBase.Client.Max(x => x.ID) + 1,
                        Name = "Example"
                    };
                    MainWindow.DataBase.Client.Add((Client)e.NewItem);
                    break;

                case "Orders":
                    int idServes = MainWindow.DataBase.Serves.Max(x => x.ID);
                    e.NewItem = new Order()
                    {
                        ID = MainWindow.DataBase.Order.Max(x => x.ID) + 1,
                        Cost = MainWindow.DataBase.Serves.Where(x => x.ID == idServes).Select(x => x.Cost).ToArray()[0],
                        Total_time = MainWindow.DataBase.Serves.Where(x => x.ID == idServes).Select(x => x.Time_to_done).ToArray()[0],
                        Order_date = DateTime.Now,
                        ID_Client = MainWindow.DataBase.Client.Max(x => x.ID),
                        ID_servese = idServes,
                        ID_Special = Convert.ToString(MainWindow.DataBase.Special.Max(x => x.ID))
                    };
                    MainWindow.DataBase.Order.Add((Order)e.NewItem);
                    break;

                case "Workers":
                    e.NewItem = new Special()
                    {
                        ID = MainWindow.DataBase.Special.Max(x => x.ID) + 1,
                        Birthday = DateTime.MinValue,
                        ID_job = MainWindow.DataBase.job.Max(x => x.ID),
                        Expiriance = 0,
                        Name = "Example"
                    };
                    MainWindow.DataBase.Special.Add((Special)e.NewItem);
                    break;

                case "Servises":
                    e.NewItem = new Serves()
                    {
                        ID = MainWindow.DataBase.Serves.Max(x => x.ID) + 1,
                        Time_to_done = 0,
                        Cost = 0,
                        Name = "Example",
                        ID_job = MainWindow.DataBase.job.Max(x => x.ID)
                    };
                    MainWindow.DataBase.Serves.Add((Serves)e.NewItem);
                    break;

                case "Jobs":
                    e.NewItem = new job()
                    {
                        ID = MainWindow.DataBase.job.Max(x => x.ID) + 1,
                        Name_of_job = "Example"
                    };
                    MainWindow.DataBase.job.Add((job)e.NewItem);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var deletItem = DataGrid.SelectedItem;

            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    MainWindow.DataBase.Client.Remove((Client)deletItem);
                    break;

                case "Orders":
                    MainWindow.DataBase.Order.Remove((Order)deletItem);
                    break;

                case "Workers":
                    MainWindow.DataBase.Special.Remove((Special)deletItem);
                    break;

                case "Servises":
                    MainWindow.DataBase.Serves.Remove((Serves)deletItem);
                    break;

                case "Jobs":
                    MainWindow.DataBase.job.Remove((job)deletItem);
                    break;
            }

            TablesChanged(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.DataBase.SaveChanges();
            SnackAdmin.MessageQueue.Enqueue("Change saved!");
        }

        AccountsWindow? window;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new AccountsWindow();
                window.Closing += (o, e) => window = null;
                window.Show();
            }
        }
    }
}

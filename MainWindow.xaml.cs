using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;

namespace kurs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public static DB DataBase { get; } = new();
        public static Snackbar? MainSnackbar { set; get; }
        public MainWindow()
        {
            InitializeComponent();
            CheckDate();
            MainSnackbar = MainSnack;
            Login.Focus();
            MainSnack.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
        }

        public static void CheckDate() 
        {
            DataBase.Order.Where(x => x.Order_date.AddDays(x.Total_time) < DateTime.Now).ToList().ForEach(x => {
                DataBase.Order.Remove(x);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var login = Login.Text;
            if (login != "")
            {
                var acc = DataBase.Acc.Where(x => login == x.Login).ToList();
                if(acc.Count == 1)
                {
                    if (acc[0].Password == Password.Text) 
                    {
                        if (acc[0].SysAdmin != -1)
                        {
                            var window = new AdminPanel(acc[0].ID);
                            window.Closing += (o, e) => this.Show();
                            window.Show();
                            this.Hide();
                        }
                        else
                        {
                            var window = new PersonalAccount(acc[0]);
                            window.Closing += (o, e) => this.Show();
                            window.Show();
                            this.Hide();
                        }
                    }
                    else 
                    { 
                        MainSnackbar.MessageQueue.Enqueue("Wrong pass!"); 
                    }
                }
                else 
                {
                    MainSnackbar.MessageQueue.Enqueue("Wrong login!"); 
                }
            }
            else 
            {
                MainSnackbar.MessageQueue.Enqueue("Please, enter login and password!"); 
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var window = new ClientWindow();
            window.Closing += (o, e) => this.Show();
            window.Show();
            this.Hide();
        }

        private void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                Password.Focus();
        }

        private void Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Button_Click(null, null);
        }
    }
}

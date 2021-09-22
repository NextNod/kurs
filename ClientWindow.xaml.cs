using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace kurs
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private int pageIndex;
        public static Client? Client { get; set; }
        public static Order? Order { get; set; }
        public static ClientWindow? Window { set; get; }
        public static Snackbar? ClientSnackbar {  get; set; } 

        private readonly Uri[] pages = new Uri[] {
            new Uri("pack://application:,,,/Pages/NewClient.xaml"),
            new Uri("pack://application:,,,/Pages/SelectOrder.xaml"),
            new Uri("pack://application:,,,/Pages/FinishPage.xaml")
        };

        public ClientWindow()
        {
            InitializeComponent();
            FieldForPages.Source = pages[pageIndex];
            List<string> list = new string[] { "Personal data", "Type of order", "Finish" }.ToList();
            StepBar.ItemsSource = list;
            StepBar.SelectedIndex = pageIndex;
            Window = this;

            ClientSnackbar = snackbar;
            ClientSnackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrect = true;
            switch (pageIndex)
            {
                case 0:
                    isCorrect = Client != null;
                    break;

                case 1:
                    isCorrect = Order != null;
                    break;
            }

            if (pageIndex + 1 < pages.Length && isCorrect)
            {
                Back.IsEnabled = true;
                pageIndex++;
                FieldForPages.Source = pages[pageIndex];
                StepBar.SelectedIndex = pageIndex;
                Next.IsEnabled = pageIndex + 1 < pages.Length;
            }
            else { snackbar.MessageQueue.Enqueue("Incorrect data!"); }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (pageIndex - 1 >= 0)
            {
                Next.IsEnabled = true;
                pageIndex--;
                FieldForPages.Source = pages[pageIndex];
                StepBar.SelectedIndex = pageIndex;
                Back.IsEnabled = pageIndex - 1 >= 0;
            }
        }
    }
}

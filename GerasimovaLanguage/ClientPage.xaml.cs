using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GerasimovaLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        int itemsPerPage = 10; // Добавлена переменная для хранения выбранного значения
        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;

        public ClientPage()
        {
            InitializeComponent();
            ComboPage.SelectedIndex = 0; // Установка значения по умолчанию
            UpdateClient();
        }

        private void UpdateClient()
        {
            var currentClient = GerasimovaLanguageEntities.GetContext().Client.ToList();

            if (currentClient != null)
            {
                ClientListView.ItemsSource = currentClient;
                TableList = currentClient;
                ChangePage(0, 0);
            }
            else
            {
                // Обработка ситуации, когда данные не получены
                MessageBox.Show("Данные не получены.");
            }
        }

        private void ChangePage(int direction, int? selectedPage)
        {

            

            CurrentPageList.Clear();
            CountRecords = TableList.Count;



            // Рассчитываем количество страниц
            if (itemsPerPage == 0) // Режим "Все"
                CountPage = 1;
            else
                CountPage = (int)Math.Ceiling((double)CountRecords / itemsPerPage);

            bool Ifupdate = true;
            int startIndex = 0, endIndex = 0;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage < CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    startIndex = CurrentPage * itemsPerPage;
                    endIndex = itemsPerPage == 0 ?
                        CountRecords :
                        Math.Min(startIndex + itemsPerPage, CountRecords);
                }
            }
            else
            {
                switch (direction)
                {
                    case 1 when CurrentPage > 0:
                        CurrentPage--;
                        break;

                    case 2 when CurrentPage < CountPage - 1:
                        CurrentPage++;
                        break;

                    default:
                        Ifupdate = false;
                        break;
                }

                startIndex = CurrentPage * itemsPerPage;
                endIndex = itemsPerPage == 0 ?
                    CountRecords :
                    Math.Min(startIndex + itemsPerPage, CountRecords);
            }

            if (Ifupdate)
            {
                // Заполняем данные для текущей страницы
                for (int i = startIndex; i < endIndex; i++)
                    CurrentPageList.Add(TableList[i]);

                // Обновляем интерфейс
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                    PageListBox.Items.Add(i);

                PageListBox.SelectedIndex = CurrentPage;
                TBCount.Text = $" {CountRecords}";
                TBAllRecords.Text = $" из {CountRecords}";
                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
            }
        }

        // Обработчики событий
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (TableList != null)
            {
                if (ComboPage.SelectedItem is ComboBoxItem selectedItem)
                {
                    var content = selectedItem.Content.ToString();
                    itemsPerPage = content == "Все" ? 0 : int.Parse(content);
                    CurrentPage = 0;
                    ChangePage(0, 0);
                }
            }
            
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, PageListBox.SelectedIndex);
        }

        // Остальные обработчики остаются без изменений
        private void LeftDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(1, null);
        private void RigtDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(2, null);

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            var currentClientServices = GerasimovaLanguageEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("НЕвозможно выполнить удаление");
            else
            {


                if (MessageBox.Show("Вы точно хотите удалить?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        GerasimovaLanguageEntities.GetContext().Client.Remove(currentClient);
                        GerasimovaLanguageEntities.GetContext().SaveChanges();
                        ClientListView.ItemsSource = GerasimovaLanguageEntities.GetContext().Client.ToList();
                        UpdateClient();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }



        }
    }

}

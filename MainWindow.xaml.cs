using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Employee {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Database db;
        public List<WorkerExt> workers;

        public MainWindow() {
            db = new Database();
            InitializeComponent();
            RefreshView();
        }

        private void RefreshView() {
            workers = db.GetWorkers().ToList();
            DataGrid1.ItemsSource = workers;
            BlockView();
        }

        private void BlockView() {
            DataGrid1.CanUserAddRows = false;
            DataGrid1.CanUserSortColumns = true;
            DataGrid1.Columns[1].IsReadOnly = true;
            DataGrid1.Columns[2].IsReadOnly = true;
            DataGrid1.Columns[4].Visibility = Visibility.Visible;
            DataGrid1.Columns[2].Visibility = Visibility.Hidden;
        }

        private void SelectionGrid1(object sender, SelectionChangedEventArgs e) {
            var items = e.AddedItems;
            if (items.Count == 0) {
                return;
            }

            try {
                WorkerExt item = (WorkerExt)items[0];
                //delete unfinished row and unselect all
                for (int i = 0; i < workers.Count; i++) {
                    if (workers[i].Id == 0 && workers[i] != item) {
                        workers.RemoveAt(i);
                        i--;
                        continue;
                    }
                    workers[i].Selected = false;
                }
                //mark item as selected, fill details, block editing
                DataGrid1.ItemsSource = workers;
                if (item != null && item.Name != null && item.Surname != null) {
                    item.Selected = true;
                    DetailsId.Content = item.Id;
                    DetailsData.Content = item.Name.Trim() + " " + item.Surname.Trim();

                    DataGrid2.ItemsSource = db.GetTasks()
                                            .Where(x => x.WorkId == item.Id);
                    if (item.Id != 0) {
                        BlockView();
                    }
                }
            }
            catch (InvalidCastException ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddRow(object sender, MouseButtonEventArgs e) {
            DataGrid1.CancelEdit();
            DataGrid1.SelectedIndex = -1;
            DataGrid1.ItemsSource = null;
            DataGrid1.CanUserSortColumns = false;
            DataGrid1.ItemsSource = workers;
            DataGrid1.CanUserAddRows = true;
            DataGrid1.Columns[1].IsReadOnly = false;
            DataGrid1.Columns[2].IsReadOnly = false;
            DataGrid1.Columns[2].Visibility = Visibility.Visible;
            DataGrid1.Columns[4].Visibility = Visibility.Hidden;
        }

        private void DeleteRow(object sender, MouseButtonEventArgs e) {
            Worker worker = (Worker)((Image)sender).DataContext;
            db.DeleteWorker(worker.Id);

            RefreshView();
        }

        private void EditEnd(object sender, DataGridCellEditEndingEventArgs e) {
            string id = GetTextByColumn(e.Row, 0), name = GetTextByColumn(e.Row, 1), surname = GetTextByColumn(e.Row, 2);

            if (id == "0" && name != "" && surname != "") {
                db.InsertWorker(name, surname);
                RefreshView();
            }
        }

        private string GetTextByColumn(DataGridRow row, int column) {
            FrameworkElement element = DataGrid1.Columns[column].GetCellContent(row);
            string s = "";
            if (element != null) {
                if (element.GetType() == typeof(TextBlock)) {
                    s = ((TextBlock)element).Text;
                }
                if (element.GetType() == typeof(TextBox)) {
                    s = ((TextBox)element).Text;
                }
            }
            return s;
        }
    }
}

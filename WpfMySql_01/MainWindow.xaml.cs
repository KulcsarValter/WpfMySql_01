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
using MySql.Data.MySqlClient;
using System.Data;

namespace WpfMySql_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Server='localhost';Database='wpfmysql';User id='root';Password='';";
        private DataTable datatable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            //Adatbázis megnyitása, adatok lekérése.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM persons";

                    //Előállitja azt az objektumot, amely eléri az adatbázist és a táblát.
                    MySqlCommand command = new MySqlCommand(query, conn);

                    //Az adapter tudja lekérdezni az adatokat a táblából.
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                    //Feltölti az adatokat a datatable-ba.
                    datatable.Clear();
                    adapter.Fill(datatable);

                    //A datatable-t hozzákapcsoljuk a grid-hez.
                    datagrid.ItemsSource = datatable.DefaultView;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Hiba a betöltés közben:{ex.Message}");
                }
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            string name = txb_name.Text;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = $"INSERT INTO persons (name) VALUES ('{name}')";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    
                    command.ExecuteNonQuery();

                    LoadData();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Hiba beszúrás közben:{ex.Message}");
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = datagrid.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                //A kijelölt sorból kiszedem az adatokat.
                int id = Convert.ToInt32(selectedRow["id"]);
                string name = txb_name.Text;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = $"UPDATE persons SET name = @name WHERE id = @id";

                        MySqlCommand command = new MySqlCommand(query, conn);

                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                        LoadData();
                    }
                    catch (MySqlException ex)
                    {

                        MessageBox.Show($"Hiba a frissítés közben:{ex.Message}");
                    }
                }

            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectRow = datagrid.SelectedItem as DataRowView;

            if (selectRow != null)
            {
                int id = Convert.ToInt32(selectRow["id"]);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM persons WHERE id = @id";
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                        LoadData();
                    }
                    catch (MySqlException ex)
                    {

                        MessageBox.Show($"Az adat nem törlődött{ex.Message}");
                    }
                }
            }
        }
    }
}

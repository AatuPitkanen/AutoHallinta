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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AutoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
 
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        SqlDataAdapter ad;
        DataTable table;
        SqlCommandBuilder cmdbl;

        public MainWindow()
        {
            connect();
            InitializeComponent();
            loadVari();
            loadTyyppi();
            loadMalli();
            loadAll();
            checkVuosi();
        }
        public void connect()
        {
            try
            {
                this.conn = new SqlConnection(@"Data Source=(localdb)\Projects;Initial Catalog=auto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False");
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void checkVuosi()
        {
            try
            {
                string query = "select Vuosi FROM dbo.Vuosimalli where Vuosi = @Vuosi";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Vuosi", DateTime.Now.Year.ToString());

                SqlDataReader read = cmd.ExecuteReader();
                Console.WriteLine(cmd.CommandText);
                if (!read.Read())
                {
                    read.Close();
                    query = "INSERT INTO dbo.Vuosimalli VALUES(@Vuosi)";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Vuosi", DateTime.Now.Year);
                    cmd.ExecuteNonQuery();
                }
                else read.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void loadAll()
        {
            try
            {
                DataSet testi = new DataSet();
                ad = new SqlDataAdapter("SELECT t1.Reknro, t1.Merkki,t1.Malli, t2.Tyyppi, t3.Vari, t4.Vuosi, t1.Rekisterissa FROM Ajoneuvo AS t1 LEFT JOIN Ajoneuvotyyppi AS t2 ON t2.Id=t1.Tyyppi LEFT JOIN Vari AS t3 ON t3.Id=t1.Vari LEFT JOIN Vuosimalli AS t4 ON t4.Id=t1.Vuosimalli", this.conn); 
                table = new DataTable();
                ad.Fill(table);
                this.DataContext = table;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        } 
        public void loadAuto()
        {
            try
            {
                DataSet testi = new DataSet();
                ad = new SqlDataAdapter("select * from dbo.Ajoneuvo", this.conn);
                table = new DataTable();
                ad.Fill(table);
                this.DataContext = table;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void loadVari()
        {
            try
            {
                this.ComboVari.Items.Clear();
                this.ComboVari_Copy.Items.Clear();

                DataSet testi = new DataSet();
                ad = new SqlDataAdapter("select * from dbo.Vari", this.conn);
                table = new DataTable();
                ad.Fill(table);
                this.DataContext = table;
               
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    this.ComboVari.Items.Add((String)table.Rows[i][1]);
                    this.ComboVari_Copy.Items.Add((String)table.Rows[i][1]);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
     
        public void loadTyyppi()
        {
            try
            {
                this.ComboTyyppi.Items.Clear();
                this.ComboTyyppi_Copy.Items.Clear();
                DataSet testi = new DataSet();
                ad = new SqlDataAdapter("select * from dbo.Ajoneuvotyyppi", this.conn);
                table = new DataTable();
                ad.Fill(table);
                this.DataContext = table;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    this.ComboTyyppi.Items.Add((String)table.Rows[i][1]);
                    this.ComboTyyppi_Copy.Items.Add((String)table.Rows[i][1]);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void loadMalli()
        {
         
            try
            {
                this.ComboVm.Items.Clear();
                this.ComboVm_Copy.Items.Clear();
                DataSet testi = new DataSet();
                ad = new SqlDataAdapter("select * from dbo.Vuosimalli", this.conn);
                table = new DataTable();
                ad.Fill(table);
                this.DataContext = table;

                DateTime today = DateTime.Today;
                //int age = today.Year - .Year;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    this.ComboVm.Items.Add((int)table.Rows[i][1]);
                    this.ComboVm_Copy.Items.Add((int)table.Rows[i][1]);
                 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
      

        private void poistaAuto()
        {
            try
            {
                string query = "DELETE FROM dbo.Ajoneuvo WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", this.DeleteAuto.Text);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void lisaaAuto()
        {
          /*  Regex regex = new Regex(@"^[a-z,A-Z]{1,3}-\d{1,3}$");
            if (!regex.Match(Reknro.Text).Success)
            {
                MessageBox.Show("Rekisterinumero on väärää muotoa");
                return;
            }*/

            try {
                int temp = 0;

                if (this.RekisteriL.IsChecked == true)
                {
                    temp = 1;
                }

                string query = "insert into dbo.Ajoneuvo(Merkki, Malli, Reknro, Rekisterissa,Vuosimalli,Tyyppi,Vari) values(@merkki,@malli,@rekno,@rekisterissa,"
                     + "(select Id from dbo.Vuosimalli where Vuosi=@vm),(select Id from dbo.Ajoneuvotyyppi where Tyyppi=@tyyppi),"
                     + "(select Id from dbo.Vari where Vari=@vari))";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@merkki", this.Merkki.Text);
                cmd.Parameters.AddWithValue("@malli", this.Malli.Text);
                cmd.Parameters.AddWithValue("@rekno", this.Reknro.Text.ToUpper());
                cmd.Parameters.AddWithValue("@rekisterissa", temp) ;
                cmd.Parameters.AddWithValue("@vm", this.ComboVm.SelectedItem);
                cmd.Parameters.AddWithValue("@tyyppi", this.ComboTyyppi.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@vari", this.ComboVari.SelectedItem.ToString());


                Console.WriteLine(this.ComboVm);
                Console.WriteLine(this.ComboTyyppi);
                Console.WriteLine(this.ComboVari.SelectedItem.ToString());
                cmd.ExecuteNonQuery();
                loadAuto();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void updateAuto()
        {
            try
            {
                string query = "UPDATE dbo.Ajoneuvo SET Merkki=@Merkki, Malli=@Malli, Rekisterissa=@Rekisterissa," + "Vuosimalli=(select Id from dbo.Vuosimalli where Vuosi=@vm),Tyyppi=(select Id from dbo.Ajoneuvotyyppi where Tyyppi=@tyyppi),"
                + "Vari=(select Id from dbo.Vari where Vari=@vari) WHERE Reknro=@reknro";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@reknro", this.Reknro_Copy.Text);
                cmd.Parameters.AddWithValue("@merkki", this.Merkki_Copy.Text);
                cmd.Parameters.AddWithValue("@malli", this.Malli_Copy.Text);
                cmd.Parameters.AddWithValue("@rekisterissa", this.RekisteriE.IsChecked);
                cmd.Parameters.AddWithValue("@vm", this.ComboVm_Copy.SelectedItem);
                cmd.Parameters.AddWithValue("@tyyppi", this.ComboTyyppi_Copy.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@vari", this.ComboVari_Copy.SelectedItem.ToString());

                Console.WriteLine(this.ComboVm_Copy);
                Console.WriteLine(this.ComboTyyppi_Copy);
                Console.WriteLine(this.ComboVari_Copy);
                cmd.ExecuteNonQuery();
                loadAuto();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void poistaAjoneuvo()
        {
            try
            {
                string query = "DELETE FROM dbo.Ajoneuvo WHERE RekNro=@rekno";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rekno", this.DeleteAuto.Text);

                cmd.ExecuteNonQuery();
                loadAuto();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            poistaAjoneuvo();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmdbl = new SqlCommandBuilder(ad);
                ad.Update(table);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            loadVari();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            loadTyyppi();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            loadMalli();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            loadAll();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            loadAuto();
        }

        private void FilterTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(DataGrid.ItemsSource).Refresh();
            }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            lisaaAuto();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            updateAuto();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            updateAuto();
        }

        private void Reknro_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            updateAuto();
        }

        private void ComboTyyppi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        }
    }


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoForm
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        SqlDataAdapter ad;
        DataTable table;
        SqlCommandBuilder cmdbl;
        public Form1()
        {
            connect();
            InitializeComponent();
            loadVuosimalli();
            loadTyyppi();
            loadVari();
            loadAuto();
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

        public void loadAuto()
        {
            try
            {
                ad = new SqlDataAdapter("SELECT t1.Reknro, t1.Merkki,t1.Malli, t2.Tyyppi, t3.Vari, t4.Vuosi, t1.Rekisterissa FROM Ajoneuvo AS t1 LEFT JOIN Ajoneuvotyyppi AS t2 ON t2.Id=t1.Tyyppi LEFT JOIN Vari AS t3 ON t3.Id=t1.Vari LEFT JOIN Vuosimalli AS t4 ON t4.Id=t1.Vuosimalli", this.conn);
                table = new DataTable();
                ad.Fill(table);
                dataGridView1.DataSource = table;
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
                ad = new SqlDataAdapter("select * from dbo.Vari", this.conn);
                table = new DataTable();
                ad.Fill(table);
                dataGridView1.DataSource = table;

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
                ad = new SqlDataAdapter("select * from dbo.AjoneuvoTyyppi", this.conn);
                table = new DataTable();
                ad.Fill(table);
                dataGridView1.DataSource = table;
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
        public void loadVuosimalli()
        {
            try
            {
                this.ComboVm.Items.Clear();
                ad = new SqlDataAdapter("select * from dbo.Vuosimalli", this.conn);
                table = new DataTable();
                ad.Fill(table);
                dataGridView1.DataSource = table;
               
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
                    query = "insert into dbo.Vuosimalli values(@Vuosi)";
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
        private void lisaaAuto()
        {
             Regex regex = new Regex(@"^[a-z,A-Z]{1,3}-\d{1,3}$");
             if (!regex.Match(Reknro.Text).Success)
             {
                 MessageBox.Show("Rekisterinumero väärää muotoa");
                 return;
             }

            try
            {

                string query = "INSERT INTO dbo.Ajoneuvo(Reknro, Merkki,Malli,Rekisterissa,Tyyppi, Vuosimalli, Vari) VALUES (@Rekno,@Merkki,@Malli,@rekisterissa,"
                    +"(select Id from dbo.Ajoneuvotyyppi where Tyyppi=@tyyppi), (select Id from dbo.Vuosimalli where Vuosi=@vm)," + "(select Id from dbo.Vari where Vari=@vari))";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@merkki", this.Merkki.Text);
                cmd.Parameters.AddWithValue("@malli", this.Malli.Text);
                cmd.Parameters.AddWithValue("@rekno", this.Reknro.Text.ToUpper());
                cmd.Parameters.AddWithValue("@rekisterissa", this.RekisteriL.Checked);
                cmd.Parameters.AddWithValue("@vm", this.ComboVm.SelectedItem);
                cmd.Parameters.AddWithValue("@tyyppi", this.ComboTyyppi.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@vari", this.ComboVari.SelectedItem.ToString());

                Console.WriteLine(this.ComboVm);
                Console.WriteLine(this.ComboTyyppi);
                Console.WriteLine(this.ComboVari);
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
                cmd.Parameters.AddWithValue("@rekisterissa", this.RekisteriE.Checked);
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
        private void poistaAuto()
        {
            try
            {
                string query = "delete from dbo.Ajoneuvo where id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", this.AutoId.Text);

                cmd.ExecuteNonQuery();
                loadAuto();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void poistaTyyppi()
        {
            try
            {
                string query = "delete from dbo.Ajoneuvotyyppi where id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", this.TyyppiId.Text);

                cmd.ExecuteNonQuery();
                loadTyyppi();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void poistaVari()
        {
            try
            {
                string query = "delete from dbo.Vari where id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", this.VariId.Text);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void poistaVuosi()
        {
            try
            {
                string query = "delete from dbo.Vuosimalli where id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", this.VuosiId.Text);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private void ajoneuvoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadAuto();
        }

        private void tyyppiToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            loadTyyppi();
        }

        private void variToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            loadVari();
        }

        private void vuosimalliToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            loadVuosimalli();
        }

        private void PoistaAuto_Click(object sender, EventArgs e)
        {
            poistaAuto();
        }

        private void DeleteColour_Click(object sender, EventArgs e)
        {
            poistaVari();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            poistaTyyppi();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            poistaVuosi();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                cmdbl = new SqlCommandBuilder(ad);
                ad.Update(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'autoDataSet.Ajoneuvo' table. You can move, or remove it, as needed.
            this.ajoneuvoTableAdapter.Fill(this.autoDataSet.Ajoneuvo);

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateAuto();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            lisaaAuto();
        }
    }
}

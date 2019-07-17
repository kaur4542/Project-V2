using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Homepage
{
    public partial class Librarian : Form
    {
        public Librarian()
        {
            InitializeComponent();
        }
        private SqlConnection conn = new SqlConnection();
        private string connString = "Server=Laptop-TBE3GK6p\\SQLEXPRESS01; " +
                            "Database=Library; User=kaur4542; Password=JKhrt956";
        private SqlCommand cmd;

        private void Refresh()
        {
            conn.ConnectionString = connString;
            cmd = conn.CreateCommand();
            //string query = "select * from Members ";
            //cmd.CommandText = query;
            conn.Open();
            //cmd.ExecuteNonQuery();
            string status = "Requested to Return";
            string query2 = "select * from Members_Book where Status='" + status + "'";
            cmd.CommandText = query2;
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;

            cmbMembers.DisplayMember = "UserId";
            cmbMembers.ValueMember = "Member_Book_Id";
            cmbMembers.DataSource = dt;

            cmbBooks.DisplayMember = "ISBN_No";
            cmbBooks.ValueMember = "ISBN_No";
            cmbBooks.DataSource = dt;
            reader.Close();

            string query3 = "select * from Requested_Book";
            cmd.CommandText = query3;
            SqlDataReader reader1 = cmd.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(reader1);
            dataGridView2.DataSource = dt1;

            comboBox2.DisplayMember = "User_Id";
            comboBox2.ValueMember = "User_Id";
            comboBox2.DataSource = dt1;

            comboBox3.DisplayMember = "ISBN_No";
            comboBox3.ValueMember = "ISBN_No";
            comboBox3.DataSource = dt1;


            reader1.Close();
            conn.Close();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchLibrarian search = new SearchLibrarian();
            search.Show();
        }

        private void BtnExtension_Click(object sender, EventArgs e)
        {
            Extensions extensions = new Extensions();
            extensions.Show();
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            Reports reports = new Reports();
            reports.Show();
        }

        private void Librarian_Load(object sender, EventArgs e)
        {
            Refresh();
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            //update status in book_member
            //add book in database
            conn.ConnectionString = connString;
            cmd = conn.CreateCommand();
            int fid = Convert.ToInt32(cmbMembers.SelectedValue);
            int fid1 = Convert.ToInt32(cmbBooks.SelectedValue);
            //MessageBox.Show("fg", fid1.ToString());
            int copies;
            int copiesInc;
            string status = "Returned";
            try
            {

                string query = "update Members_Book set Status ='" + status + "'where Member_Book_Id=" + fid;
                cmd.CommandText = query;
                conn.Open();
                cmd.ExecuteScalar();
                string query2 = "select * from Books where ISBN_No='" + fid1 + "'";
                cmd.CommandText = query2;
                SqlDataReader reader1 = cmd.ExecuteReader();

                while (reader1.Read())
                {
                    copies = Convert.ToInt32(reader1["Copies_Available"]);
                    copiesInc = copies + 1;
                    LoginInfo.copies = copiesInc.ToString();

                }
                reader1.Close();
                string query3 = "update Books set Copies_Available ='" + LoginInfo.copies + "'where ISBN_No=" + fid1;
                cmd.CommandText = query3;
                cmd.ExecuteNonQuery();
                // MessageBox.Show("copies", LoginInfo.copies);

            }

            catch (Exception ex)
            {
                string msg = ex.Message;
                string caption = "error";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                cmd.Dispose();
                conn.Close();
                //Refresh();

            }



        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Refresh();
            
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            conn.ConnectionString = connString;
            cmd = conn.CreateCommand();
            int isbn = Convert.ToInt32(comboBox3.SelectedValue);//isbn
            int user = Convert.ToInt32(comboBox2.SelectedValue);//member
            string status = "Issued";
            //string userID = LoginInfo.id;
            string issueDate = dateTimePicker1.Value.ToShortDateString();
            string dueDate = dateTimePicker2.Value.ToShortDateString();

            try
            {
                string query = "Insert into Members_Book values('" + user +
                                   "','" + isbn + "','" + issueDate + "','" + dueDate + "','" + status + "');";

                cmd.CommandText = query;
                conn.Open();
                cmd.ExecuteNonQuery();

                string query2 = "delete from Requested_Book where User_Id='" +
                                 user + "' and ISBN_No='" + isbn + "';";


                cmd.CommandText = query2;
                cmd.ExecuteNonQuery();
                string msg = "Book Issued";
                string caption = "Info";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
                string caption = "error";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                cmd.Dispose();
                conn.Close();
                //Refresh();

            }

        }

    }
}

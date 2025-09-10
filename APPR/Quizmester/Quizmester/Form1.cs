using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Quizmester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=QuizDB;Trusted_Connection=True;";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                using (DataTable datatable = new DataTable())
                using (SqlCommand command = new SqlCommand("SELECT * FROM Questions WHERE QuestionText LIKE '%' + @name + '%'", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", txbSearch.Text);

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(datatable);

                    dgvInformation.DataSource = datatable;
                }
            }
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace quizGame
{
    public partial class ScoreboardForm : Form
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;

        public ScoreboardForm()
        {
            InitializeComponent();

            // Eventkoppelingen (kan ook via de Designer)
            this.Load += ScoreboardForm_Load;
            btnRefresh.Click += btnRefresh_Click;
            btnClose.Click += btnClose_Click;
        }

        // -------------------------------------------------------------
        // Wanneer het venster geladen wordt:
        // Stel de DataGridView stijl in en laad de Top 10 scores
        // -------------------------------------------------------------
        private void ScoreboardForm_Load(object sender, EventArgs e)
        {
            if (dgv != null)
            {
                dgv.ReadOnly = true;
                dgv.RowHeadersVisible = false;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            LoadTop10();
        }

        // -------------------------------------------------------------
        // Klik op “Vernieuwen” → herlaad het scorebord
        // -------------------------------------------------------------
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTop10();
        }

        // -------------------------------------------------------------
        // Klik op “Sluiten” → sluit het venster
        // -------------------------------------------------------------
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // -------------------------------------------------------------
        // Haal de Top 10 scores op uit de database
        // (velden: username_snapshot / score / created_at)
        // -------------------------------------------------------------
        private void LoadTop10()
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    "SELECT TOP 10 " +
                    "       username_snapshot AS Gebruiker, " +
                    "       score             AS Score, " +
                    "       created_at        AS Datum " +
                    "FROM dbo.scores " +
                    "ORDER BY score DESC, created_at ASC;", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    con.Open();
                    da.Fill(dt);

                    // Voeg een kolom “Rang” toe (positie)
                    var ranked = new DataTable();
                    ranked.Columns.Add("Rang", typeof(int));
                    ranked.Columns.Add("Gebruiker", typeof(string));
                    ranked.Columns.Add("Score", typeof(int));
                    ranked.Columns.Add("Datum", typeof(DateTime));

                    int rank = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        ranked.Rows.Add(
                            rank++,
                            row["Gebruiker"] == DBNull.Value ? "Onbekend" : row["Gebruiker"],
                            row["Score"] == DBNull.Value ? 0 : row["Score"],
                            row["Datum"] == DBNull.Value ? DateTime.MinValue : row["Datum"]
                        );
                    }

                    dgv.DataSource = ranked;

                    // Datum mooi opmaken (jjjj-MM-dd HH:mm)
                    if (dgv.Columns["Datum"] != null)
                        dgv.Columns["Datum"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het laden van het scorebord:\n" + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

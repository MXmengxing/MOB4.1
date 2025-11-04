using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace quizGame
{
    public partial class AdminForm : Form
    {
        // Connectionstring uit App.config
        private readonly string _cs = ConfigurationManager
            .ConnectionStrings["QuizDb"].ConnectionString;

        public AdminForm()
        {
            InitializeComponent();

            // Nettere start
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Beheer vragen (Administrator)";

            // UI-gedrag (mag ook in Designer)
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            dgvQuestions.MultiSelect = false;
            dgvQuestions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQuestions.RowHeadersVisible = false;
            dgvQuestions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Standaard 15s als het in bereik is
            if (numSeconds.Minimum <= 15 && numSeconds.Maximum >= 15)
                numSeconds.Value = 15;

            // Zorg dat een fallback-categorie bestaat
            EnsureDefaultCategory();

            // Eerst categorieën, dan vragen inladen
            LoadCategories();
            LoadQuestions();

            // Event-koppelingen (voorkom dubbele binding)
            dgvQuestions.SelectionChanged -= dgvQuestions_SelectionChanged;
            dgvQuestions.SelectionChanged += dgvQuestions_SelectionChanged;

            dgvQuestions.CellDoubleClick -= dgvQuestions_CellDoubleClick;
            dgvQuestions.CellDoubleClick += dgvQuestions_CellDoubleClick;
        }

        // ---------------------------------------------------------------------
        // DB-helpers
        // ---------------------------------------------------------------------

        /// <summary>
        /// Zorgt dat categorie 'Algemeen' bestaat zodat category_id nooit vastloopt.
        /// </summary>
        private void EnsureDefaultCategory()
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(@"
                    IF NOT EXISTS(SELECT 1 FROM dbo.categories WHERE name = N'Algemeen')
                        INSERT INTO dbo.categories(name) VALUES (N'Algemeen');", con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Niet fataal; UI blijft werken, maar inserts verwachten wél een geldige category_id
            }
        }

        /// <summary>
        /// Laad alle categorieën in de combobox.
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var da = new SqlDataAdapter(
                    "SELECT id, name FROM dbo.categories ORDER BY name;", con))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    cboCategory.DisplayMember = "name";
                    cboCategory.ValueMember = "id";
                    cboCategory.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Categorieën laden mislukt: " + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Laad het vragenoverzicht inclusief categorienaam (JOIN).
        /// </summary>
        private void LoadQuestions()
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var da = new SqlDataAdapter(@"
                    SELECT q.id,
                           q.question_text,
                           q.per_question_seconds,
                           q.category_id,
                           c.name AS category_name
                      FROM dbo.questions q
                      JOIN dbo.categories c ON c.id = q.category_id
                    ORDER BY q.id;", con))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvQuestions.AutoGenerateColumns = true;
                    dgvQuestions.DataSource = dt;

                    // Vriendelijkere kolomkoppen
                    if (dgvQuestions.Columns.Contains("question_text"))
                        dgvQuestions.Columns["question_text"].HeaderText = "Vraag";
                    if (dgvQuestions.Columns.Contains("per_question_seconds"))
                        dgvQuestions.Columns["per_question_seconds"].HeaderText = "Seconden";
                    if (dgvQuestions.Columns.Contains("category_name"))
                        dgvQuestions.Columns["category_name"].HeaderText = "Categorie";

                    // category_id hoef je zelden te tonen (verborgen houden is gebruikelijk)
                    if (dgvQuestions.Columns.Contains("category_id"))
                        dgvQuestions.Columns["category_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vragen laden mislukt: " + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------------
        // UI-events
        // ---------------------------------------------------------------------

        /// <summary>
        /// Als selectie verandert, vul de invoervelden met de rijgegevens.
        /// </summary>
        private void dgvQuestions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                // Schoon invoervelden bij geen selectie
                txtQuestion.Clear();
                if (numSeconds.Minimum <= 15 && numSeconds.Maximum >= 15)
                    numSeconds.Value = 15;
                return;
            }

            var row = dgvQuestions.SelectedRows[0];

            txtQuestion.Text = Convert.ToString(row.Cells["question_text"].Value) ?? "";

            int sec = 15;
            int.TryParse(Convert.ToString(row.Cells["per_question_seconds"].Value), out sec);
            sec = Math.Max((int)numSeconds.Minimum, Math.Min((int)numSeconds.Maximum, sec));
            numSeconds.Value = sec;

            // Selecteer bijbehorende categorie in de combobox (indien beschikbaar)
            try
            {
                if (row.Cells["category_id"].Value != null &&
                    row.Cells["category_id"].Value != DBNull.Value)
                {
                    int cid = Convert.ToInt32(row.Cells["category_id"].Value);
                    cboCategory.SelectedValue = cid;
                }
            }
            catch
            {
                // Stil falen voorkomt irritante popups bij edge cases
            }
        }

        /// <summary>
        /// Voeg een nieuwe vraag toe (category_id is verplicht).
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string text = txtQuestion.Text.Trim();
            int seconds = (int)numSeconds.Value;

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Voer een vraagtekst in.");
                return;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Kies eerst een categorie.");
                return;
            }

            int catId = Convert.ToInt32(cboCategory.SelectedValue);

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.questions(question_text, per_question_seconds, category_id)
                    VALUES(@q, @s, @c);", con))
                {
                    cmd.Parameters.AddWithValue("@q", text);
                    cmd.Parameters.AddWithValue("@s", seconds);
                    cmd.Parameters.AddWithValue("@c", catId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Reset invoer en herlaad
                txtQuestion.Clear();
                if (numSeconds.Minimum <= 15 && numSeconds.Maximum >= 15)
                    numSeconds.Value = 15;

                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toevoegen mislukt: " + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Werk een bestaande vraag bij (category_id is verplicht).
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteer eerst een vraag om te bewerken.");
                return;
            }

            int id = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["id"].Value);
            string newText = txtQuestion.Text.Trim();
            int newSec = (int)numSeconds.Value;

            if (string.IsNullOrWhiteSpace(newText))
            {
                MessageBox.Show("Vraagtekst mag niet leeg zijn.");
                return;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Kies eerst een categorie.");
                return;
            }
            int catId = Convert.ToInt32(cboCategory.SelectedValue);

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.questions
                       SET question_text = @q,
                           per_question_seconds = @s,
                           category_id = @c
                     WHERE id = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@q", newText);
                    cmd.Parameters.AddWithValue("@s", newSec);
                    cmd.Parameters.AddWithValue("@c", catId);
                    cmd.Parameters.AddWithValue("@id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bijwerken mislukt: " + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Verwijder de geselecteerde vraag (met bevestiging).
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteer eerst een vraag om te verwijderen.");
                return;
            }

            int id = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["id"].Value);

            if (MessageBox.Show("Weet je zeker dat je deze vraag wilt verwijderen?",
                    "Bevestigen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    "DELETE FROM dbo.questions WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verwijderen mislukt: " + ex.Message,
                    "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Vernieuw beide lijsten：categorie + vragen（防止只刷问题看不到新分类）
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
            LoadQuestions();
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        /// <summary>
        /// Dubbelklik op een rij opent het antwoord-beheer venster.
        /// </summary>
        private void dgvQuestions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int qid = Convert.ToInt32(dgvQuestions.Rows[e.RowIndex].Cells["id"].Value);
            string qtext = Convert.ToString(dgvQuestions.Rows[e.RowIndex].Cells["question_text"].Value);

            using (var f = new AnswerEditorForm(qid, qtext))
            {
                f.ShowDialog(this);
            }
        }
    }
}

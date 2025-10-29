using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace quizGame
{
    public partial class AdminForm : Form
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;

        public AdminForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Beheer vragen (Administrator)";

            // 数值默认 15s（如果你设计器里没设）
            if (numSeconds.Minimum <= 15 && numSeconds.Maximum >= 15)
                numSeconds.Value = 15;

            LoadQuestions();
        }

        // 载入题目列表
        private void LoadQuestions()
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    "SELECT id, question_text, per_question_seconds, category FROM dbo.questions ORDER BY id;", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvQuestions.DataSource = dt;

                    // 友好一些的列标题
                    if (dgvQuestions.Columns.Contains("question_text"))
                        dgvQuestions.Columns["question_text"].HeaderText = "Vraag";
                    if (dgvQuestions.Columns.Contains("per_question_seconds"))
                        dgvQuestions.Columns["per_question_seconds"].HeaderText = "Seconden";
                    if (dgvQuestions.Columns.Contains("category"))
                        dgvQuestions.Columns["category"].HeaderText = "Categorie";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Laden mislukt: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 选中一行时，把内容带到下方编辑区
        private void dgvQuestions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0) return;
            var row = dgvQuestions.SelectedRows[0];
            txtQuestion.Text = row.Cells["question_text"].Value?.ToString() ?? "";
            txtCategory.Text = row.Cells["category"].Value?.ToString() ?? "";

            int sec = 15;
            int.TryParse(row.Cells["per_question_seconds"].Value?.ToString(), out sec);
            sec = Math.Max((int)numSeconds.Minimum, Math.Min((int)numSeconds.Maximum, sec));
            numSeconds.Value = sec;
        }

        // 新增
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string text = txtQuestion.Text.Trim();
            int seconds = (int)numSeconds.Value;
            string cat = txtCategory.Text.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Voer een vraagtekst in.");
                return;
            }

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.questions(question_text, per_question_seconds, category) VALUES(@q, @s, @c);", con))
                {
                    cmd.Parameters.AddWithValue("@q", text);
                    cmd.Parameters.AddWithValue("@s", seconds);
                    cmd.Parameters.AddWithValue("@c", string.IsNullOrWhiteSpace(cat) ? (object)DBNull.Value : cat);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // 清空输入区并刷新
                txtQuestion.Clear();
                txtCategory.Clear();
                numSeconds.Value = 15;
                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toevoegen mislukt: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 更新
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
            string newCat = txtCategory.Text.Trim();

            if (string.IsNullOrWhiteSpace(newText))
            {
                MessageBox.Show("Vraagtekst mag niet leeg zijn.");
                return;
            }

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    @"UPDATE dbo.questions 
                      SET question_text=@q, per_question_seconds=@s, category=@c 
                      WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@q", newText);
                    cmd.Parameters.AddWithValue("@s", newSec);
                    cmd.Parameters.AddWithValue("@c", string.IsNullOrWhiteSpace(newCat) ? (object)DBNull.Value : newCat);
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bijwerken mislukt: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 删除
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteer eerst een vraag om te verwijderen.");
                return;
            }

            int id = Convert.ToInt32(dgvQuestions.SelectedRows[0].Cells["id"].Value);
            if (MessageBox.Show("Weet je zeker dat je deze vraag wilt verwijderen?", "Bevestigen",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand("DELETE FROM dbo.questions WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verwijderen mislukt: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 刷新
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadQuestions();
        }

        // 关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvQuestions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // 防止点到表头

            int qid = Convert.ToInt32(dgvQuestions.Rows[e.RowIndex].Cells["id"].Value);
            string qtext = Convert.ToString(dgvQuestions.Rows[e.RowIndex].Cells["question_text"].Value);

            using (var f = new AnswerEditorForm(qid, qtext))
            {
                f.ShowDialog(this);
            }
        }

    }
}

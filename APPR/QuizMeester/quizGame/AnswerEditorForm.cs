using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace quizGame
{
    /// <summary>
    /// 管理“某一道题”的 4 个答案。
    /// 规则：必须正好 4 条，且仅 1 条 is_correct = true。
    /// </summary>
    public class AnswerEditorForm : Form
    {
        private readonly int _questionId;
        private readonly string _questionText;
        private readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;

        private Label lblTitle;
        private DataGridView dgv;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnSave;
        private Button btnClose;

        public AnswerEditorForm(int questionId, string questionText)
        {
            _questionId = questionId;
            _questionText = questionText;

            // 基本窗体属性
            Text = "Antwoorden bewerken";
            StartPosition = FormStartPosition.CenterParent;
            Width = 800;
            Height = 500;

            BuildUi();
            LoadAnswers();
        }

        private void BuildUi()
        {
            lblTitle = new Label
            {
                AutoSize = true,
                Text = $"Vraag #{_questionId}: {_questionText}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(12, 12)
            };

            dgv = new DataGridView
            {
                Location = new Point(12, 45),
                Width = 760,
                Height = 360,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // 隐藏主键列
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "ID",
                DataPropertyName = "id",
                Visible = false
            };

            var colText = new DataGridViewTextBoxColumn
            {
                Name = "colText",
                HeaderText = "Antwoord",
                DataPropertyName = "answer_text",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colCorrect = new DataGridViewCheckBoxColumn
            {
                Name = "colCorrect",
                HeaderText = "Correct?",
                DataPropertyName = "is_correct",
                Width = 100
            };

            dgv.Columns.AddRange(colId, colText, colCorrect);

            btnAdd = new Button
            {
                Text = "Toevoegen",
                Location = new Point(12, 420),
                Width = 100
            };
            btnAdd.Click += BtnAdd_Click;

            btnDelete = new Button
            {
                Text = "Verwijderen",
                Location = new Point(118, 420),
                Width = 100
            };
            btnDelete.Click += BtnDelete_Click;

            btnSave = new Button
            {
                Text = "Opslaan",
                Location = new Point(672, 420),
                Width = 100
            };
            btnSave.Click += BtnSave_Click;

            btnClose = new Button
            {
                Text = "Sluiten",
                Location = new Point(566, 420),
                Width = 100
            };
            btnClose.Click += (s, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(dgv);
            Controls.Add(btnAdd);
            Controls.Add(btnDelete);
            Controls.Add(btnSave);
            Controls.Add(btnClose);
        }

        private void LoadAnswers()
        {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("answer_text", typeof(string));
            dt.Columns.Add("is_correct", typeof(bool));

            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(
                    @"SELECT id, answer_text, is_correct
                      FROM dbo.answers
                      WHERE question_id=@qid
                      ORDER BY id;", con))
                {
                    cmd.Parameters.Add("@qid", SqlDbType.Int).Value = _questionId;
                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            dt.Rows.Add(
                                Convert.ToInt32(r["id"]),
                                Convert.ToString(r["answer_text"]),
                                Convert.ToBoolean(r["is_correct"])
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Laden mislukt: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dgv.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dt = (DataTable)dgv.DataSource;
            if (dt.Rows.Count >= 4)
            {
                MessageBox.Show("Elke vraag moet precies 4 antwoorden hebben.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dt.Rows.Add(null, "", false);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            if (MessageBox.Show("Geselecteerde antwoord(en) verwijderen?", "Bevestigen",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                dgv.Rows.Remove(row);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            dgv.EndEdit(); // 提交网格编辑

            var dt = (DataTable)dgv.DataSource;

            // —— 校验：恰好 4 条、文本非空、仅 1 条正确 ——
            if (dt.Rows.Count != 4)
            {
                MessageBox.Show("Elke vraag moet precies 4 antwoorden hebben.", "Fout",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dt.AsEnumerable().Any(r => string.IsNullOrWhiteSpace(Convert.ToString(r["answer_text"]))))
            {
                MessageBox.Show("Antwoordtekst mag niet leeg zijn.", "Fout",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int correctCount = dt.AsEnumerable().Count(r => Convert.ToBoolean(r["is_correct"]));
            if (correctCount != 1)
            {
                MessageBox.Show("Er moet precies 1 juist antwoord zijn.", "Fout",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // —— 保存到数据库（简单稳妥：先删本题所有答案，再插入当前 4 条） ——
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var tx = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    con.Open();

                    // 删除本题所有答案
                    using (var del = new SqlCommand("DELETE FROM dbo.answers WHERE question_id=@qid;", con, tx))
                    {
                        del.Parameters.Add("@qid", SqlDbType.Int).Value = _questionId;
                        del.ExecuteNonQuery();
                    }

                    // 插入 4 个
                    using (var ins = new SqlCommand(
                        @"INSERT INTO dbo.answers(question_id, answer_text, is_correct)
                          VALUES(@qid, @t, @c);", con, tx))
                    {
                        ins.Parameters.Add("@qid", SqlDbType.Int).Value = _questionId;
                        var pText = ins.Parameters.Add("@t", SqlDbType.NVarChar, 200);
                        var pCorrect = ins.Parameters.Add("@c", SqlDbType.Bit);

                        foreach (DataRow r in dt.Rows)
                        {
                            pText.Value = Convert.ToString(r["answer_text"]).Trim();
                            pCorrect.Value = Convert.ToBoolean(r["is_correct"]);
                            ins.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }

                MessageBox.Show("Opgeslagen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opslaan mislukt: " + ex.Message, "Fout",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

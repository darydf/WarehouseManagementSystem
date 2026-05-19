using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Forms
{
    public partial class FormContractorCheck : Form
    {
        private ContractorCheckService _service;
        private TextBox txtInn;
        private Button btnCheck;
        private Panel panelResult;
        private Label lblStatus;
        private Button btnClose;

        public FormContractorCheck()
        {
            InitializeComponent();
            _service = new ContractorCheckService();
            CreateForm();
        }

        private void CreateForm()
        {
            this.Text = "Проверка контрагента";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Поле ИНН
            Label lblInn = new Label();
            lblInn.Text = "ИНН контрагента:";
            lblInn.Location = new Point(50, 40);
            lblInn.Size = new Size(100, 25);
            this.Controls.Add(lblInn);

            txtInn = new TextBox();
            txtInn.Location = new Point(50, 70);
            txtInn.Size = new Size(200, 25);
            this.Controls.Add(txtInn);

            // Кнопка "Проверить"
            btnCheck = new Button();
            btnCheck.Text = "Проверить";
            btnCheck.Location = new Point(270, 68);
            btnCheck.Size = new Size(100, 30);
            btnCheck.Click += btnCheck_Click;
            this.Controls.Add(btnCheck);

            // Панель результата
            panelResult = new Panel();
            panelResult.Location = new Point(50, 120);
            panelResult.Size = new Size(320, 100);
            panelResult.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelResult);

            // Лейбл статуса внутри панели
            lblStatus = new Label();
            lblStatus.Location = new Point(10, 10);
            lblStatus.Size = new Size(290, 80);
            panelResult.Controls.Add(lblStatus);

            // Кнопка закрытия
            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Location = new Point(270, 250);
            btnClose.Size = new Size(100, 35);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private async void btnCheck_Click(object sender, EventArgs e)
        {
            string inn = txtInn.Text.Trim();

            if (string.IsNullOrEmpty(inn) || (inn.Length != 10 && inn.Length != 12))
            {
                MessageBox.Show("Введите корректный ИНН (10 или 12 цифр)", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnCheck.Enabled = false;
            btnCheck.Text = "Проверяем...";

            try
            {
                var result = await _service.CheckByInn(inn, 1);

                if (result.Status == "RELIABLE")
                {
                    panelResult.BackColor = Color.LightGreen;
                    lblStatus.Text = $"✓ {result.Message}\nПроверено: {result.CheckedAt:dd.MM.yyyy HH:mm}";
                    if (result.IsFromCache)
                        lblStatus.Text += "\n(данные из кэша)";
                }
                else if (result.Status == "BLACKLISTED")
                {
                    panelResult.BackColor = Color.LightCoral;
                    lblStatus.Text = $"✗ {result.Message}";
                }
                else
                {
                    panelResult.BackColor = Color.LightGray;
                    lblStatus.Text = result.Message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheck.Enabled = true;
                btnCheck.Text = "Проверить";
            }
        }

    }
}

using Npgsql;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseManagementSystem.Helpers;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            comboBoxCurrency.Items.Clear();
            comboBoxCurrency.Items.AddRange(new string[] { "RUB", "USD", "EUR" });
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            Text = "Настройки";
            LoadSettings();
            LoadCurrencySettings();
        }

        private void LoadSettings()
        {
            try
            {
                string sql = "SELECT SettingKey, SettingValue FROM AppSettings";
                var data = DatabaseHelper.ExecuteQuery(sql);

                foreach (DataRow row in data.Rows)
                {
                    var key = row["SettingKey"].ToString();
                    var value = row["SettingValue"].ToString();

                    if (key == "DiscountPercentage")
                        numDiscountPercent.Value = Convert.ToInt32(value);
                    else if (key == "DiscountDaysBeforeExpiry")
                        numDiscountDays.Value = Convert.ToInt32(value);
                }
            }
            catch
            {
                numDiscountPercent.Value = 20;
                numDiscountDays.Value = 30;
            }
        }

        private void LoadCurrencySettings()
        {
            try
            {
                string sql = "SELECT SettingValue FROM AppSettings WHERE SettingKey = 'BaseCurrency'";
                var result = DatabaseHelper.ExecuteScalar(sql);

                if (result != null && comboBoxCurrency.Items.Contains(result.ToString()))
                    comboBoxCurrency.SelectedItem = result.ToString();
                else
                    comboBoxCurrency.SelectedItem = "RUB";
            }
            catch
            {
                comboBoxCurrency.SelectedItem = "RUB";
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Полностью очищаем таблицу
                DatabaseHelper.ExecuteNonQuery("DELETE FROM AppSettings");

                // Вставляем все три настройки заново
                string insertSql = "INSERT INTO AppSettings (SettingKey, SettingValue, Description) VALUES (@key, @value, '')";

                var p1 = new NpgsqlParameter("@key", "DiscountPercentage");
                var v1 = new NpgsqlParameter("@value", numDiscountPercent.Value.ToString());
                DatabaseHelper.ExecuteNonQuery(insertSql, new[] { p1, v1 });

                var p2 = new NpgsqlParameter("@key", "DiscountDaysBeforeExpiry");
                var v2 = new NpgsqlParameter("@value", numDiscountDays.Value.ToString());
                DatabaseHelper.ExecuteNonQuery(insertSql, new[] { p2, v2 });

                var p3 = new NpgsqlParameter("@key", "BaseCurrency");
                var v3 = new NpgsqlParameter("@value", comboBoxCurrency.SelectedItem.ToString());
                DatabaseHelper.ExecuteNonQuery(insertSql, new[] { p3, v3 });

                var currencyService = new CurrencyService();
                await currencyService.UpdateExchangeRates();
                CurrencyHelper.LoadSettings();

                MessageBox.Show("Настройки сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
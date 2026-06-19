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

namespace WarehouseManagementSystem.Forms
{
    public partial class HeatmapForm : Form
    {
        public HeatmapForm()
        {
            InitializeComponent();
            LoadCategories();
            LoadData();
            SetupGridView();
        }
        private void SetupGridView()
        {
            string[] months = { "июн 2025", "июл 2025", "авг 2025", "сен 2025", "окт 2025", "ноя 2025",
                                "дек 2025", "янв 2026", "фев 2026", "мар 2026", "апр 2026", "май 2026",
                                "июн 2026", "июл 2026", "авг 2026" };

            dgvHeatmap.Columns.Clear();
            dgvHeatmap.Columns.Add("ProductName", "Товар");
            dgvHeatmap.Columns["ProductName"].Width = 150;
            dgvHeatmap.Columns["ProductName"].Frozen = true;
            dgvHeatmap.Columns["ProductName"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            foreach (string month in months)
            {
                dgvHeatmap.Columns.Add(month, month);
                dgvHeatmap.Columns[month].Width = 85;
                dgvHeatmap.Columns[month].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvHeatmap.Columns[month].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvHeatmap.CellPainting += DgvHeatmap_CellPainting;
        }
        private void LoadCategories()
        {
            try
            {
                string sql = "SELECT Id, Name FROM Categories ORDER BY Name";
                var data = DatabaseHelper.ExecuteQuery(sql);

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("Все категории");

                foreach (DataRow row in data.Rows)
                {
                    cmbCategory.Items.Add(new CategoryItem
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString()
                    });
                }

                cmbCategory.SelectedIndex = 0;
                cmbCategory.SelectedIndexChanged += (s, e) => LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

      
        private void LoadData()
        {
            try
            {
                int? categoryId = null;
                if (cmbCategory.SelectedIndex > 0 && cmbCategory.SelectedItem is CategoryItem cat)
                {
                    categoryId = cat.Id;
                }

                string sql = @"
            SELECT 
                p.Name AS ProductName,
                COALESCE(p.ShelfLife, 0) AS ShelfLife
            FROM Products p
            WHERE p.IsActive = true";

                if (categoryId.HasValue)
                {
                    sql += $" AND p.CategoryId = {categoryId.Value}";
                }

                sql += " ORDER BY p.Name";

                var products = DatabaseHelper.ExecuteQuery(sql);

                // Очищаем строки, но НЕ очищаем колонки (они уже настроены в SetupGridView)
                dgvHeatmap.Rows.Clear();

                if (products.Rows.Count == 0)
                {
                    dgvHeatmap.Rows.Add("Нет товаров");
                    return;
                }

                foreach (DataRow product in products.Rows)
                {
                    string productName = product["ProductName"].ToString();
                    int shelfLife = Convert.ToInt32(product["ShelfLife"]);

                    // Добавляем новую строку
                    int rowIndex = dgvHeatmap.Rows.Add();

                    // Заполняем ячейки
                    dgvHeatmap.Rows[rowIndex].Cells[0].Value = productName;

                    int year = 2025;
                    int month = 6;

                    for (int col = 0; col < 15; col++)
                    {
                        DateTime currentDate = new DateTime(year, month, 1);
                        DateTime expiryDate = currentDate.AddDays(shelfLife);
                        int status = GetStatus(expiryDate, currentDate, shelfLife);
                        dgvHeatmap.Rows[rowIndex].Cells[col + 1].Value = status;

                        month++;
                        if (month > 12)
                        {
                            month = 1;
                            year++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private int GetStatus(DateTime expiryDate, DateTime currentMonth, int shelfLife)
        {
            if (shelfLife == 0) return 4;
            if (expiryDate < currentMonth) return 3;
            double monthsLeft = (expiryDate - currentMonth).TotalDays / 30;
            if (monthsLeft < 1) return 2;
            if (monthsLeft <= 6) return 1;
            return 0;
        }
        private void DgvHeatmap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex > 0 && e.Value != null)
            {
                int status = (int)e.Value;
                Color backColor;

                switch (status)
                {
                    case 0: backColor = Color.FromArgb(146, 208, 80); break;
                    case 1: backColor = Color.FromArgb(255, 255, 153); break;
                    case 2: backColor = Color.FromArgb(255, 153, 51); break;
                    case 3: backColor = Color.FromArgb(255, 102, 102); break;
                    default: backColor = Color.LightGray; break;
                }

                using (Brush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
                e.PaintContent(e.CellBounds);
                e.Handled = true;
            }
        }
        private class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

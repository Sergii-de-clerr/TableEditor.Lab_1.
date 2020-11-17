using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TableEditor.Lab_1
{
    public partial class TableEditor : Form
    {
        int columns = 10;
        int rows = 10;
        Table Main_table;
        bool Is_Formula_Saved = true;
        string path;
        //string[] Alpeabet = new string[]( "A", "B", "C", "D", "E",
        //    "F", "G", "H", "I", "J", "K", "L", "M", "N",
        //    "O", "P", "Q", "R", "S", "T", "U", "V", "W",
        //    "X", "Y", "Z" );
        public TableEditor()
        {
            InitializeComponent();
            Main_table = new Table(rows, columns);
            FillDataGrid();
        }

        public TableEditor(string Path, int Rows, int Columns)
        {
            InitializeComponent();
            this.path = Path;
            this.rows = Rows;
            this.columns = Columns;
            Main_table = new Table(Path, rows, columns);
            Main_table.EvaluateCells();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            TabledataGridView.RowHeadersWidth = 60;
            TabledataGridView.ColumnCount = columns;
            TabledataGridView.RowCount = rows;
            for (int i = 0; i < columns; i++)
            {
                if (i < 26)
                    TabledataGridView.Columns[i].Name = Convert.ToChar(65 + i).ToString();
                else
                {
                    TabledataGridView.Columns[i].Name = "";
                    int n = 26;
                    for (; ((i / n) >= 26); n *= 26) ;
                    for (; n != 1; n /= 26)
                        TabledataGridView.Columns[i].Name += Convert.ToChar(64 + ((i / n) % 26)).ToString();
                    TabledataGridView.Columns[i].Name += Convert.ToChar(65 + (i % 26)).ToString();
                }
            }
            for (int i = 0; i < rows; i++)
                TabledataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (Main_table.Cells[i, j].Is_Formula_Count)
                        TabledataGridView.Rows[i].Cells[j].Value = Main_table.Cells[i, j].value;
                    else TabledataGridView.Rows[i].Cells[j].Value = "";
        }

        private void AddColumn(object sender, EventArgs e)
        {
            TabledataGridView.Columns.Add(new DataGridViewColumn(TabledataGridView.Rows[0].Cells[0]));
            columns++;
            Main_table = new Table(Main_table, rows, columns);
            FillDataGrid();
        }

        private void AddRow(object sender, EventArgs e)
        {
            rows++;
            Main_table = new Table(Main_table, rows, columns);
            TabledataGridView.Rows.Add(new DataGridViewRow());
            FillDataGrid();
        }

        private void DeleteRow(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Видалити рядок?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                if (rows == 1)
                {
                    MessageBox.Show("У таблиці один рядок", "Неможливо виконати операцію", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                rows--;
                TabledataGridView.Rows.RemoveAt(rows - 1);
                Main_table.rows--;
                Main_table = new Table(Main_table, rows, columns);
                Main_table.EvaluateCellsWMess(true);
                FillDataGrid();
            }
        }

        private void DeleteColumn(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Видалити стовпчик?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                if (columns == 1)
                {
                    MessageBox.Show("У таблиці один стовпчик", "Неможливо виконати операцію", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                columns--;
                TabledataGridView.Columns.RemoveAt(columns - 1);
                Main_table.columns--;
                Main_table = new Table(Main_table, rows, columns);
                Main_table.EvaluateCellsWMess(true);
                FillDataGrid();
            }
        }

        private void TabledataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            CurentElLabel.Text = TabledataGridView.Columns[e.ColumnIndex].Name + 
                TabledataGridView.Rows[e.RowIndex].HeaderCell.Value;
            ParsertextBox.Text = Main_table.Cells[e.RowIndex, e.ColumnIndex].expression;
        }

        private void ParsertextBox_TextChanged(object sender, EventArgs e)
        {
            Is_Formula_Saved = false;
            if (TabledataGridView.SelectedCells.Count != 0)
            {
                if ((TabledataGridView.RowCount > 0) || (TabledataGridView.ColumnCount > 0))
                {
                    Main_table.Cells[TabledataGridView.SelectedCells[0].RowIndex,
                        TabledataGridView.SelectedCells[0].ColumnIndex].expression = ParsertextBox.Text;
                }
            }
        }

        private void ParsertextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveExp_from_Textbox();
            }
        }

        void SaveExp_from_Textbox ()
        {
            Main_table.IsError = false;
            int save_c = TabledataGridView.SelectedCells[0].ColumnIndex;
            int save_r = TabledataGridView.SelectedCells[0].RowIndex;
            Main_table.Cells[save_r, save_c].expression = ParsertextBox.Text;
            Main_table.EvaluateCell(save_r, save_c);
            if (Main_table.IsError)
            {
                MessageBox.Show(Main_table.ParsMess);
                Is_Formula_Saved = false;
                Main_table.EvaluateCellsWMess(false);
                FillDataGrid();
            }
            else
            {
                Main_table.Cells[save_r, save_c].Is_Formula_Count = true;
                Is_Formula_Saved = true;
                Main_table.EvaluateCellsWMess(false);
                FillDataGrid();
            }
        }

        private void SaveTable(object sender, EventArgs e)
        {
            Main_table.SaveTable(path);
        }

        private void проПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Лабораторна робота №1:
Розробка програми для роботи з електронними таблицями
Виконав студент групи К-26 Назарчук Сергій");
        }

        private void умоваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Варіант 19: 
Операції:
1) +, -, (унарні операції);
2) ^ (піднесення у ступінь);
3) max(x,y), min(x,y);
4) mmax(x1,x2,...,xN), mmin(x1,x2,...,xN) (N>=1);
Для того щоб записати формулу в клітинку натисніть на неї
та у відповідному textbox уведіть формулу та натисніть Enter.");
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Path = new SaveFileDialog();
            Path.InitialDirectory = "f:\\";
            Path.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            Path.ShowDialog();
            Path.DefaultExt = ".xml";
            this.path = Path.FileName;
            Main_table.SaveTable(Path.FileName);
        }

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            openFileDialog1.InitialDirectory = "f:\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                TableEditor table = new TableEditor(filePath, Main_table.rows, Main_table.columns);
                table.Show();
            }
        }

        private void зберегтиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Main_table.SaveTable(path);
        }

        private void TableEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Не бажаєте зберегти таблицю?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                var Path = new SaveFileDialog();
                Path.InitialDirectory = "f:\\";
                Path.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                Path.ShowDialog();
                Path.DefaultExt = ".xml";
                this.path = Path.FileName;
                Main_table.SaveTable(Path.FileName);
            }
        }
    }
    public class Table
    {
        public bool IsError = false;
        public string ParsMess;
        public Cell[,] Cells = { };
        private Parser parser;
        public int rows;
        public int columns;
        public Table(string Path, int Rows, int Columns)
        {
            Cells = new Cell[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Cells[i, j] = new Cell();
            this.rows = Rows;
            this.columns = Columns;
            ReadTable(Path);
            parser = new Parser(Cells, Rows, Columns);
            EvaluateCells();
        }
        public Table (int Rows, int Columns)
        {
            Cells = new Cell[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Cells[i, j] = new Cell();
            this.rows = Rows;
            this.columns = Columns;
            parser = new Parser(Cells, Rows, Columns);
        }
        public Table(Table Copy, int Rows, int Columns)
        {
            Cells = new Cell[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Cells[i, j] = new Cell();
            this.rows = Rows;
            this.columns = Columns;
            for (int i = 0; (i < Copy.rows) && (i < Rows); i++)
                for (int j = 0; (j < Copy.columns) && (j < Columns); j++)
                {
                    Cells[i, j].expression = Copy.Cells[i, j].expression;
                    Cells[i, j].value = Copy.Cells[i, j].value;
                    Cells[i, j].Is_Formula_Count = Copy.Cells[i, j].Is_Formula_Count;
                }
            parser = new Parser(Cells, Rows, Columns);
        }
        public void EvaluateCells()
        {
            IsError = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (Cells[i, j].Is_Formula_Count)
                    {
                        Cells[i, j].value = parser.Eval(Cells[i, j].expression, i, j);
                        Parser.varsInFormula.Clear();
                        if (parser.Error)
                        {
                            IsError = true;
                            Cells[i, j].Is_Formula_Count = false;
                            ParsMess = parser.message;
                            return;
                        }
                    }
        }
        public void EvaluateCellsWMess(bool r)
        {
            IsError = false;
            bool What = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (!Cells[i, j].IsNULL())
                    {
                        Cells[i, j].value = parser.Eval(Cells[i, j].expression, i, j);
                        Parser.varsInFormula.Clear();
                        if (parser.Error)
                        {
                            if (Cells[i, j].Is_Formula_Count == true)
                            {
                                What = true;
                            }
                            Cells[i, j].Is_Formula_Count = false;
                        }
                        else
                        {
                            Cells[i, j].Is_Formula_Count = true;
                        }
                    }
            if ((What == true) && (r == true))
            {
                MessageBox.Show(@"Через видалення елементів деякі 
                    клітинки втратили значення");
            }
        }
        public void EvaluateCell(int row, int column)
        {
            if (Cells[row, column].expression != "")
            {
                Cells[row, column].value = parser.Eval(Cells[row, column].expression, row, column);
                Parser.varsInFormula.Clear();
                if (parser.Error)
                {
                    IsError = true;
                    ParsMess = parser.message;
                    return;
                }
            }
            IsError = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (Cells[i, j].Is_Formula_Count)
                    {
                        Cells[i, j].value = parser.Eval(Cells[i, j].expression, i, j);
                        Parser.varsInFormula.Clear();
                        if (parser.Error)
                        {
                            IsError = true;
                            Cells[i, j].Is_Formula_Count = false;
                            ParsMess = parser.message;
                            return;
                        }
                    }
        }
        public void SaveTable(string path)
        {
            if (path == string.Empty)
            {
                MessageBox.Show("Не вказаний шлях");
                return;
            }
            XDocument xdoc = new XDocument();
            XElement table = new XElement("table");
            XAttribute colAttr = new XAttribute("columns", columns.ToString());
            XAttribute rowAttr = new XAttribute("rows", rows.ToString());
            table.Add(colAttr);
            table.Add(rowAttr);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (Cells[i, j].Is_Formula_Count)
                    {
                        XElement cell = new XElement("cell", Cells[i, j].expression);
                        XAttribute XIndex = new XAttribute("X", (i + 1).ToString());
                        XAttribute YIndex = new XAttribute("Y", (j + 1).ToString());
                        cell.Add(XIndex);
                        cell.Add(YIndex);
                        table.Add(cell);
                    }
            xdoc.Add(table);
            xdoc.Save(path);
        }
        public void ReadTable(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            int X;
            int Y;
            string f = xmlDoc.InnerXml;
            string chislo = string.Empty;
            int beg = f.IndexOf("columns");
            for (int i = 0; f[beg + 9 + i] != 34; i++)
                chislo += f[beg + 9 + i];
            columns = Convert.ToInt32(chislo);
            beg = f.IndexOf("rows");
            chislo = string.Empty;
            for (int i = 0; f[beg + 6 + i] != 34; i++)
                chislo += f[beg + 6 + i];
            rows = Convert.ToInt32(chislo);
            Cells = new Cell[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    Cells[i, j] = new Cell();
            foreach (XmlElement table in xmlDoc.DocumentElement.ChildNodes)
            {
                foreach (XmlElement cellinfo in xmlDoc.DocumentElement.ChildNodes)
                {
                    Int32.TryParse(cellinfo.GetAttribute("X"), out X);
                    Int32.TryParse(cellinfo.GetAttribute("Y"), out Y);
                    Cells[X - 1, Y - 1] = new Cell(cellinfo.InnerText);
                }
            }
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (!Cells[i, j].IsNULL())
                        Cells[i, j].Is_Formula_Count = true;
        }
    }
    public class Cell
    {
        public string name;
        public string expression;
        public double value;
        public bool Is_Formula_Count = false;
        public Cell (string expr)
        {
            expression = expr;
            value = 0;
        }
        public Cell()
        {
            expression = "";
            value = 0;
        }
        public bool IsNULL()
        {
            if ((expression == string.Empty))
                return true;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeiroNet1
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Экземпляр класса NeiroWeb
        /// </summary>
        private NeiroWeb nw;

        /// <summary>
        /// Точка для начала вычисления
        /// </summary>
        private Point startP;

        /// <summary>
        /// Двумерный массив данных
        /// </summary>
        private int[,] arr;

        /// <summary>
        /// Имя файла памяти с весами
        /// </summary>
        private string memoryName;


        /// <summary>
        /// Флаг для начала обучения
        /// </summary>
        private bool enableTraining;

        /// <summary>
        /// Конструктор класса MainForm
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            enableTraining = false;
        }

        /// <summary>
        /// Метод загрузки формы
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
                     
        }

        /// <summary>
        /// Метод, отвечающий за закрытие формы
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            nw.SaveState();
        }

        /// <summary>
        /// Перемещение с зажатой левой кнопки мыши, инициирующее рисование линии
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point endP = new Point(e.X,e.Y);
                Bitmap image = (Bitmap)pictureBox1.Image;
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.DrawLine(new Pen(Color.BlueViolet), startP, endP);
                }
                pictureBox1.Image = image;
                startP = endP;                
            }
        }

        /// <summary>
        /// Нажатие кнопки мыши на picturebox1
        /// </summary>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {    
            startP = new Point(e.X, e.Y);
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Метод инициации загрузки символа из файла
        /// </summary>
        private void loadSymvolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addZipFile();
        }

        /// <summary>
        /// Метод загрузки символа из файла
        /// </summary>
        private void addZipFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "All files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            string[] ls = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
            if (ls.Length == 0) return;
            var newItems = new List<string>();
            foreach (var i in comboBox.Items) if (!newItems.Contains((string)i)) newItems.Add((string)i);
            foreach (var i in             ls) if (!newItems.Contains((string)i)) newItems.Add((string)i);
            newItems.Sort();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(newItems.ToArray());
            comboBox.SelectedIndex = 0;            
        }

        /// <summary>
        /// Метод перемещения литеры в память
        /// </summary>
        private void toMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string litera = comboBox.SelectedIndex >= 0 ? (string)comboBox.Items[comboBox.SelectedIndex] : comboBox.Text;
            if (litera.Length == 0)
            {
                MessageBox.Show("Name = NULL");
                return;
            }
            nw.SetTraining(litera, arr);
            NeiroGraphUtils.ClearImage(pictureBox1);
            NeiroGraphUtils.ClearImage(pictureBox2);
            NeiroGraphUtils.ClearImage(pictureBox3);
        }

        /// <summary>
        /// Метод инициализирующий обучение
        /// </summary>
        private void learnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Learn();
        }

        /// <summary>
        /// Метод отрисовки символа из системы
        /// </summary>
        private void drawFromComboBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NeiroGraphUtils.ClearImage(pictureBox1);
            NeiroGraphUtils.ClearImage(pictureBox2);
            NeiroGraphUtils.ClearImage(pictureBox3);
            pictureBox1.Image = NeiroGraphUtils.DrawLitera(pictureBox1.Image, (string)comboBox.SelectedItem);
        }


        /// <summary>
        /// Обучение
        /// </summary>
        private void Learn()
        {
            int[,] clipArr = NeiroGraphUtils.CutImageToArray((Bitmap)pictureBox1.Image, new Point(pictureBox1.Width, pictureBox1.Height));
            if (clipArr == null) return;
            arr = NeiroGraphUtils.LeadArray(clipArr, new int[NeiroWeb.neironInArrayWidth, NeiroWeb.neironInArrayHeight]);
            pictureBox2.Image = NeiroGraphUtils.GetBitmapFromArr(clipArr);
            pictureBox3.Image = NeiroGraphUtils.GetBitmapFromArr(arr);
            string s = nw.CheckLitera(arr);
            if (s == null) s = "null";
            DialogResult askResult = MessageBox.Show("result = " + s + " ?", "", MessageBoxButtons.YesNo);
            if ( askResult != DialogResult.Yes || !enableTraining || MessageBox.Show("save ?", "", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            nw.SetTraining(s, arr);
            NeiroGraphUtils.ClearImage(pictureBox1);
            NeiroGraphUtils.ClearImage(pictureBox2);
            NeiroGraphUtils.ClearImage(pictureBox3);       
        }


        /// <summary>
        /// Метод добавления литеры из текстбокса в список cobmobox`а
        /// </summary>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string t = ((TextBox)sender).Text;
                if (t == null || t.Length == 0) return;
                comboBox.Items.Add(t[0].ToString());
            }
        }

        /// <summary>
        /// Метод инициалиализации обучения по нажаю правой кнопки мыши
        /// </summary>
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) Learn();
        }

        /// <summary>
        /// Метод включения обучения
        /// </summary>
        private void enableTrainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableTraining = true;
            MessageBox.Show("Training mode is enable.");
        }

        /// <summary>
        /// Вспомогательный метод, обновляющий память нейронной сети и список символов для обучения
        /// </summary>
        /// <param name="res">Имя файла памяти</param>
        private void RefreshMemory(string memoryName)
        {
            NeiroGraphUtils.ClearImage(pictureBox1);
            comboBox.Items.Clear();
            if (memoryName == null)
            {
                memoryName = "memory.txt";
            }
            nw = new NeiroWeb(memoryName);
            nw.SetMemoryName(memoryName);
            string[] items = nw.GetLiteras();
            if (items.Length > 0)
            {
                comboBox.Items.AddRange(items);
                comboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Метод, позволяющий загрузить файл с памятью нейронной сети
        /// </summary>
        private void loadMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "All files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            memoryName = openFileDialog.FileName;

            RefreshMemory(memoryName);
        }

        /// <summary>
        ///  Метод, позволяющий сохранить новый файл с памятью нейронной сети и продолжить с ним работу.
        /// </summary>
        private void newMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            string fileName = saveFileDialog.FileName;
            File.WriteAllText(fileName, "[{\"name\":\"1\",\"veight\":[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0.16666666666666669,0.16666666666666669,0,0.33333333333333331,0.83333333333333326,0.16666666666666669,0,0,0,0.83333333333333326,0.83333333333333326,0.83333333333333326,0.83333333333333326,0.83333333333333326,0.16666666666666669,0.16666666666666669,0,0.83333333333333326,1,1,1,1,1,1,1,1,1,1,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.16666666666666669,0.83333333333333326,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],\"countTrainig\":3},{\"name\":\"2\",\"veight\":[0,0,0,0,0,0,0,0,0,0,0,0.5,1,0,0,0,0,0,0.5,1,1,1,1,1,0,0,0.5,0.5,1,1,1,0.5,0.5,0,0,0,0.5,0.5,1,1,1,0.5,0,0,0,0.5,0.5,1,1,1,1,0.5,0,0,0.5,0.5,1,0.5,0.5,1,1,1,1,0.5,0.5,1,0.5,0,0.5,1,0.5,0.5,1,1,1,0.5,0,0,0.5,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],\"countTrainig\":2}]", Encoding.GetEncoding(1251));
            memoryName = fileName;

            RefreshMemory(memoryName);
        }
    }
}

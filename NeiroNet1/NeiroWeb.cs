using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace NeiroNet1
{
    class NeiroWeb
    {
        /// <summary>
        /// Значение по умолчанию для количества нейронов
        /// </summary>
        public const int defaultNeironCount = 32;

        /// <summary>
        /// Ширина массива нейронов
        /// </summary>
        public const int neironInArrayWidth = 10;

        /// <summary>
        /// Высота массива нейронов
        /// </summary>
        public const int neironInArrayHeight = 10;

        /// <summary>
        /// Имя нейрона
        /// </summary>
        private static string neironName;

        /// <summary>
        /// Массив нейронов
        /// </summary>
        private List<Neiron> neironArray = null;

        /// <summary>
        /// Значение по умолчанию для веса нейрона
        /// </summary>
        private const double[,] defaultNeironVeight = null;

        /// <summary>
        /// Имя файла, в котором сохраняются весы нейрона.
        /// </summary>
        private string memoryName;

        /// <summary>
        /// Метод для возврата имени файла с весами.
        /// </summary>
        /// <returns>Имя файла с памятью</returns>
        public string GetMemoryName() { return memoryName; }

        /// <summary>
        /// Метод для получения имени файла с весами.
        /// </summary>
        public void SetMemoryName(string memoryName) { this.memoryName = memoryName; }

        /// <summary>
        /// Конструктор класса NeiroWeb.
        /// </summary>
        /// <param name="memory">Передаваемое имя файла с весами.</param>
        public NeiroWeb(string memoryName)
        {
            string[] lines = File.ReadAllLines(memoryName);
            if (lines.Length == 0)
            {
                neironArray = new List<Neiron>();
                return;
            }
            string jStr = lines[0];
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<Object> objects = json.Deserialize<List<Object>>(jStr);
            neironArray = new List<Neiron>();
            foreach (var o in objects) neironArray.Add(NeironCreate((Dictionary<string, Object>)o));
        }

        /// <summary>
        /// Метод для проверки литерала или символа в передаваемом массиве данных.
        /// </summary>
        /// <param name="arr">Массив данных с основного поля для считывания</param>
        public string CheckLitera(int[,] arr)
        {
            string res = null;
            double max = 0;
            foreach (var n in neironArray)
            {
                double d = n.GetResourses(arr);
                if (d > max)
                {
                    max = d;
                    res = n.GetName();
                }
            }
            return res;
        }

        /// <summary>
        /// Метод для сохранения весов нейросети в файл
        /// </summary>
        public void SaveState()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            string jStr = json.Serialize(neironArray);
            System.IO.StreamWriter file = new System.IO.StreamWriter(memoryName);
            file.WriteLine(jStr);
            file.Close();
        }

        /// <summary>
        /// Метод для возврата списка отсортированных имен нейрона.
        /// </summary>
        /// <returns>Массив строк</returns>
        public string[] GetLiteras()
        {
            var res = new List<string>();
            for (int i = 0; i < neironArray.Count; i++) res.Add(neironArray[i].GetName());
            res.Sort();
            return res.ToArray();
        }

        /// <summary>
        /// Обучает нейрон с заданным именем на переданных данных
        /// </summary>
        /// <param name="trainingName">Имя нейрона</param>
        /// <param name="data">Данные для обучения</param>
        public void SetTraining(string trainingName, int[,] data)
        {
            Neiron neiron = neironArray.Find(v => v.GetName().Equals(trainingName));
            if (neiron == null)
            {
                neironName = trainingName;
                neiron = new Neiron(defaultNeironVeight, neironInArrayWidth, neironInArrayHeight, neironName);
                neironArray.Add(neiron);
            }
            int countTraining = neiron.Training(data);
            MessageBox.Show("litera - " + neiron.GetName() + " count of training = " + countTraining.ToString());
        }

        /// <summary>
        /// Создает нейрон на основе переданного словаря
        /// </summary>
        /// <param name="o">Словарь с данными нейрона</param>
        /// <returns>Нейрон</returns>
        private static Neiron NeironCreate(Dictionary<string, object> o)
        {
            Neiron res = new Neiron(defaultNeironVeight, neironInArrayWidth, neironInArrayHeight, neironName);
            if (o.ContainsKey("name"))
            {
                res.SetName((string)o["name"]);
            }
            if (o.ContainsKey("countTraining"))
            {
                res.SetCountTraining((int)o["countTraining"]);
            }
            if (o.ContainsKey("veight"))
            {
                Object[] veightData = (Object[])o["veight"];
                int arrSize = (int)Math.Sqrt(veightData.Length);
                res.SetVeight(new double[arrSize, arrSize]);
                int index = 0;
                for (int n = 0; n < res.GetVeight().GetLength(0); n++)
                    for (int m = 0; m < res.GetVeight().GetLength(1); m++)
                    {
                        res.SetVeight(Double.Parse(veightData[index].ToString()), n, m);
                        index++;
                    }
            }
            return res;
        }
    }
}

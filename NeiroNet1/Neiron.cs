using System;

namespace NeiroNet1
{
    /// <summary>
    /// Базовая структурная единица нейронной сети.
    /// </summary>
    class Neiron
    {
        /// <summary>
        /// Имя нейрона.
        /// Используется для обозначения символа или паттерна, который он представляет.
        /// </summary>
        private string neironName { get; set; }

        /// <summary>
        /// Метод, возвращающий имя нейрона.
        /// </summary>
        /// <returns>Возвращает имя нейрона</returns>
        public string GetName() { return neironName; }

        /// <summary>
        /// Метод, принимающий имя нейрона.
        /// </summary>
        public void SetName(string neironName) { this.neironName = neironName; }

        /// <summary>
        /// Двумерный массив весов.
        /// </summary>
        private double[,] veight { get; set; }

        /// <summary>
        /// Метод, возвращающий массив весов.
        /// </summary>
        public double[,] GetVeight() { return veight; }

        /// <summary>
        /// Метод, принимающий значения массива весов.
        /// </summary>
        public void SetVeight(double veight,int n, int m) { this.veight[n, m] = veight; }

        /// <summary>
        /// Метод, принимющий массив весов.
        /// </summary>
        public void SetVeight(double[,] veight) { this.veight = veight; }

        /// <summary>
        /// Счётчик обучения.
        /// Записывает, сколько раз был обучен данный нейрон.
        /// </summary>
        private int countTraining { get; set; }

        /// <summary>
        /// Метод, принимающий числовое количество тренировок.
        /// </summary>
        public void SetCountTraining(int countTraining) { this.countTraining = countTraining; }

        /// <summary>
        /// Метод, возвращающий числовое количество тренировок
        /// </summary>
        public int GetCountTraining() { return countTraining; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="Veight">Массив весов данных</param>
        /// <param name="Width">Ширина матрицы входных данных. Ширина матрицы определяет количество столбцов пикселей</param>
        /// <param name="Height">Высота матрицы входных данных. Высота матрицы определяет количество строк пикселей</param>
        /// <param name="NeironName">Имя нейрона. Используется для обозначения символа или паттерна, который он представляет. </param>
        public Neiron(double[,] Veight, int Width, int Height, string NeironName)
        {
            this.neironName = NeironName;
            veight = Veight;
            veight = new double[Width, Height];
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++)
                    veight[n, m] = 0;
            countTraining = 0;
        }

        /// <summary>
        /// Вычисляет результат нейрона на основе переданных данных.Сравнивает веса с данными, возвращая среднее значение в диапазоне от 0 до 1 (где 1 — полное соответствие).
        /// </summary>
        /// <param name="data">Массив, передающийся из NeiroNet.</param>
        /// <returns>Усредненное значение.</returns>
        public double GetResourses(int[,] data)
        {
            if (veight.GetLength(0) != data.GetLength(0) || veight.GetLength(1) != data.GetLength(1)) return -1;
            double res = 0;
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++)
                    res += 1 - Math.Abs(veight[n, m] - data[n, m]);
            return res / (veight.GetLength(0) * veight.GetLength(1));
        }

        /// <summary>
        /// Функция обучения нейрона. 
        /// Модифицирует веса на основе предоставленных данных и увеличивает счетчик обучения.Возвращает обновленное значение счетчика обучения.
        /// </summary>
        /// <param name="data">Массив, передающийся из NeiroNet.</param>
        /// <returns>Возвращает колличество тренировок.</returns>
        public int Training(int[,] data)
        {
            if (data == null || veight.GetLength(0) != data.GetLength(0) || veight.GetLength(1) != data.GetLength(1)) return countTraining;
            countTraining++;
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++)
                {
                    double v = data[n, m] == 0 ? 0 : 1;
                    veight[n, m] += 2 * (v - 0.5f) / countTraining;
                    if (veight[n, m] > 1) veight[n, m] = 1;
                    if (veight[n, m] < 0) veight[n, m] = 0;
                }
            return countTraining;
        }
    }
}

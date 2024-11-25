using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace ex
{
    public class exceptions3
    {
        public static void Main()
        {
            string directoryPath = "file";
            string noFilePath = Path.Combine(directoryPath, "no_file.txt"); // oб'єднує шлях до папки та ім'я файлу в один повний шлях
            string badDataPath = Path.Combine(directoryPath, "bad_data.txt");
            string overflowPath = Path.Combine(directoryPath, "overflow.txt");
            string[] fileNames = GenerateFileNames(10, 29);

            try
            {
                Directory.CreateDirectory(directoryPath); // cтворює папку за заданим шляхом (directoryPath). Якщо папка вже існує, нічого не робить

                // перевіряємо чи всі файли від 10.txt до 29.txt відсутні
                bool allFilesMissing = fileNames.All(fileName => !File.Exists(Path.Combine(directoryPath, fileName))); // Exists перевіряє чи існує файл 

                if (allFilesMissing) // якщо всі файли відсутні, створюємо їх
                { 
                    foreach (string fileName in fileNames) 
                    {
                        string filePath = Path.Combine(directoryPath, fileName); 
                        try
                        {
                            using (FileStream fs = File.Open(filePath, FileMode.CreateNew)) { }// створюємо порожній файл, якщо його немає 
                        }
                        catch (IOException) { } // файл уже існує, нічого не робимо
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка створення файлу {fileName}: {ex.Message}");
                        }
                    }
                }
                File.WriteAllText(noFilePath, string.Empty);
                File.WriteAllText(badDataPath, string.Empty);
                File.WriteAllText(overflowPath, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка створення каталогу або файлів: {ex.Message}");
                return;
            }

            int sumOfProducts = 0, countOfProducts = 0;

            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName); 
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        int[] numbers = new int[2];
                        numbers[0] = int.Parse(reader.ReadLine());
                        numbers[1] = int.Parse(reader.ReadLine());

                        checked // включає перевірку на переповнення для цілочисельних операцій
                        {
                            int product = numbers[0] * numbers[1];
                            sumOfProducts += product;
                            countOfProducts++;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    File.AppendAllText(noFilePath, fileName + Environment.NewLine);
                }
                catch (FormatException)
                {
                    File.AppendAllText(badDataPath, fileName + Environment.NewLine);
                }
                catch (OverflowException)
                {
                    File.AppendAllText(overflowPath, fileName + Environment.NewLine);
                }
                catch (Exception)
                {
                    File.AppendAllText(badDataPath, fileName + Environment.NewLine);
                }
            }

            try
            {
                Console.WriteLine(countOfProducts > 0
                    ? $"Середнє арифметичне: {(double)sumOfProducts / countOfProducts}"
                    : "Не вдалося обчислити жодного добутку.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка обчислення середнього арифметичного: {ex.Message}");
            }
        }

        public static string[] GenerateFileNames(int start, int end)
        {
            string[] fileNames = new string[end - start + 1];
            for (int i = 0; i < fileNames.Length; i++)
                fileNames[i] = $"{start + i}.txt";
            return fileNames;
        }
    }
}


//long firstNumber = long.Parse(firstLine);
//long secondNumber = long.Parse(secondLine);

//if (firstNumber < int.MinValue || firstNumber > int.MaxValue || secondNumber < int.MinValue || secondNumber > int.MaxValue)
//{
//    File.AppendAllText(badDataPath, fileName + Environment.NewLine);
//}
//else
//{
//    checked
//    {
//        int product = (int)firstNumber * (int)secondNumber;
//        sumOfProducts += product;
//        countOfProducts++;
//    }
//}
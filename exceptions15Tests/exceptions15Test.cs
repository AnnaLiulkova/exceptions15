using ex;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;

namespace exceptions15Tests
{
    [TestClass]
    public class ProgramTest
    {
        private string directoryPath = "file";
        private string noFilePath;
        private string badDataPath;
        private string overflowPath;
        private string[] fileNames;

        [TestInitialize]
        public void Setup()
        {
            // Ініціалізація шляху до файлів
            noFilePath = Path.Combine(directoryPath, "no_file.txt");
            badDataPath = Path.Combine(directoryPath, "bad_data.txt");
            overflowPath = Path.Combine(directoryPath, "overflow.txt");
            fileNames = ex.exceptions3.GenerateFileNames(10, 29);

            // Створення папки і очищення файлів перед тестами
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(noFilePath, string.Empty);
            File.WriteAllText(badDataPath, string.Empty);
            File.WriteAllText(overflowPath, string.Empty);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Очистка після тестів
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [TestMethod]
        public void TestCorrectCalculationOfAverage()
        {
            // Перевірка правильного обчислення середнього значення
            string filePath = Path.Combine(directoryPath, "10.txt");
            string[] validNumbers = { "2", "3" };
            File.WriteAllLines(filePath, validNumbers);

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ex.exceptions3.Main();

                string expectedOutput = "Середнє арифметичне: 6";
                string actualOutput = sw.ToString().Trim(); // Останній вивід програми

                Assert.AreEqual(expectedOutput, actualOutput, "Середнє арифметичне обчислено неправильно.");
            }
        }

        [TestMethod]
        public void TestNoFileHandling()
        {
            // Ініціалізація
            string missingFileName = "10.txt";
            string filePath = Path.Combine(directoryPath, missingFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Гарантуємо, що файл дійсно відсутній
            }

            // Виклик програми
            ex.exceptions3.Main();

            // Перевірка результату
            Assert.IsTrue(File.Exists(noFilePath), "Файл no_file.txt не було створено.");
            string[] noFileContent = File.ReadAllLines(noFilePath);

            Assert.IsTrue(
                !noFileContent.Contains(missingFileName),
                $"Файл {missingFileName} не був доданий до no_file.txt."
            );
        }

        [TestMethod]
        public void TestBadDataHandling()
        {
            // Перевірка, чи правильно обробляються файли з некоректними даними
            string badDataFileName = "11.txt";
            string filePath = Path.Combine(directoryPath, badDataFileName);
            string[] invalidData = { "abc", "123" };
            File.WriteAllLines(filePath, invalidData);

            ex.exceptions3.Main();

            string[] badDataContent = File.ReadAllLines(badDataPath);
            Assert.IsTrue(badDataContent.Contains(badDataFileName), "Файл з некоректними даними не було додано до bad_data.txt.");
        }

        [TestMethod]
        public void TestOverflowHandling()
        {
            // Перевірка, чи правильно обробляються файли, що викликають OverflowException
            string overflowFileName = "12.txt";
            string filePath = Path.Combine(directoryPath, overflowFileName);
            string[] overflowData = { int.MaxValue.ToString(), "2" };
            File.WriteAllLines(filePath, overflowData);

            ex.exceptions3.Main();

            string[] overflowContent = File.ReadAllLines(overflowPath);
            Assert.IsTrue(overflowContent.Contains(overflowFileName), "Файл, що викликав OverflowException, не було додано до overflow.txt.");
        }

        [TestMethod]
        public void TestEmptyFilesAreSkipped()
        {
            // Перевірка, чи порожні файли пропускаються
            string emptyFileName = "13.txt";
            string filePath = Path.Combine(directoryPath, emptyFileName);
            File.WriteAllText(filePath, string.Empty); // Створюємо порожній файл

            ex.exceptions3.Main();

            string[] badDataContent = File.ReadAllLines(badDataPath);
            Assert.IsTrue(badDataContent.Contains(emptyFileName), "Порожній файл не було додано до bad_data.txt.");
        }

        [TestMethod]
        public void TestAllFilesAreProcessed()
        {
            // Перевірка, чи всі файли обробляються
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                File.WriteAllLines(filePath, new string[] { "1", "2" }); // Заповнюємо всі файли валідними даними
            }

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ex.exceptions3.Main();

                string output = sw.ToString();
                Assert.IsTrue(output.Contains("Середнє арифметичне"), "Програма не обробила всі файли коректно.");
            }
        }
        [TestMethod]
        public void TestMultipleMissingFiles()
        {
            // Ініціалізація: видаляємо кілька файлів
            string[] missingFileNames = { "10.txt", "11.txt", "12.txt" };
            foreach (var fileName in missingFileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Видаляємо файл, якщо він існує
                }
            }

            // Виклик програми
            ex.exceptions3.Main();

            // Перевірка результату
            Assert.IsTrue(File.Exists(noFilePath), "Файл no_file.txt не було створено.");
            string[] noFileContent = File.ReadAllLines(noFilePath);

            foreach (var missingFileName in missingFileNames)
            {
                Assert.IsTrue(
                    !noFileContent.Contains(missingFileName),
                    $"Файл {missingFileName} не був доданий до no_file.txt."
                );
            }
        }

        // Тест на відсутність помилок при наявності всіх даних
        [TestMethod]
        public void TestNoErrorsWithValidData()
        {
            // Створення всіх файлів з валідними даними
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                File.WriteAllLines(filePath, new[] { "1", "1" }); // Усі файли з добутком 1
            }

            // Виклик програми
            ex.exceptions3.Main();

            // Перевірка, чи файли з помилками залишаються порожніми
            Assert.IsTrue(File.Exists(noFilePath), "Файл no_file.txt не було створено.");
            Assert.IsTrue(File.Exists(badDataPath), "Файл bad_data.txt не було створено.");
            Assert.IsTrue(File.Exists(overflowPath), "Файл overflow.txt не було створено.");

            Assert.AreEqual(0, File.ReadAllLines(noFilePath).Length, "Файл no_file.txt не є порожнім.");
            Assert.AreEqual(0, File.ReadAllLines(badDataPath).Length, "Файл bad_data.txt не є порожнім.");
            Assert.AreEqual(0, File.ReadAllLines(overflowPath).Length, "Файл overflow.txt не є порожнім.");
        }

        // Тест на обчислення середнього арифметичного з декількома файлами
        [TestMethod]
        public void TestAverageCalculationWithMultipleFiles()
        {
            // Створення файлів з валідними даними
            string file1 = "10.txt";
            string file2 = "11.txt";
            File.WriteAllLines(Path.Combine(directoryPath, file1), new[] { "2", "3" }); // Добуток 6
            File.WriteAllLines(Path.Combine(directoryPath, file2), new[] { "4", "5" }); // Добуток 20

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Виклик програми
                ex.exceptions3.Main();

                // Очікуваний результат
                string expectedOutput = "Середнє арифметичне: 13"; // (6 + 20) / 2
                string actualOutput = sw.ToString().Trim();

                Assert.AreEqual(expectedOutput, actualOutput, "Середнє арифметичне обчислено неправильно.");
            }
        }

        //Тест на обробку порожнього файлу
        [TestMethod]
        public void TestEmptyFileHandling()
        {
            // Створення порожнього файлу
            string emptyFileName = "10.txt";
            string filePath = Path.Combine(directoryPath, emptyFileName);
            File.WriteAllText(filePath, string.Empty);

            // Виклик програми
            ex.exceptions3.Main();

            // Перевірка, чи файл потрапив до bad_data.txt
            Assert.IsTrue(File.Exists(badDataPath), "Файл bad_data.txt не було створено.");
            string[] badDataContent = File.ReadAllLines(badDataPath);

            Assert.IsTrue(
                badDataContent.Contains(emptyFileName),
                $"Порожній файл {emptyFileName} не був доданий до bad_data.txt."
            );
        }

        //Тест на перевірку створення всіх відсутніх файлів
        [TestMethod]
        public void TestMissingFilesAreCreated()
        {
            // Видаляємо всі файли
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Виклик програми
            ex.exceptions3.Main();

            // Перевірка, чи всі файли створено
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                Assert.IsTrue(File.Exists(filePath), $"Файл {fileName} не було створено.");
            }
        }
    }
}
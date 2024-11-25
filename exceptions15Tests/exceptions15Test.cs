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
            // ����������� ����� �� �����
            noFilePath = Path.Combine(directoryPath, "no_file.txt");
            badDataPath = Path.Combine(directoryPath, "bad_data.txt");
            overflowPath = Path.Combine(directoryPath, "overflow.txt");
            fileNames = ex.exceptions3.GenerateFileNames(10, 29);

            // ��������� ����� � �������� ����� ����� �������
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(noFilePath, string.Empty);
            File.WriteAllText(badDataPath, string.Empty);
            File.WriteAllText(overflowPath, string.Empty);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // ������� ���� �����
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [TestMethod]
        public void TestCorrectCalculationOfAverage()
        {
            // �������� ����������� ���������� ���������� ��������
            string filePath = Path.Combine(directoryPath, "10.txt");
            string[] validNumbers = { "2", "3" };
            File.WriteAllLines(filePath, validNumbers);

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ex.exceptions3.Main();

                string expectedOutput = "������ �����������: 6";
                string actualOutput = sw.ToString().Trim(); // ������� ���� ��������

                Assert.AreEqual(expectedOutput, actualOutput, "������ ����������� ��������� �����������.");
            }
        }

        [TestMethod]
        public void TestNoFileHandling()
        {
            // �����������
            string missingFileName = "10.txt";
            string filePath = Path.Combine(directoryPath, missingFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // ���������, �� ���� ����� �������
            }

            // ������ ��������
            ex.exceptions3.Main();

            // �������� ����������
            Assert.IsTrue(File.Exists(noFilePath), "���� no_file.txt �� ���� ��������.");
            string[] noFileContent = File.ReadAllLines(noFilePath);

            Assert.IsTrue(
                !noFileContent.Contains(missingFileName),
                $"���� {missingFileName} �� ��� ������� �� no_file.txt."
            );
        }

        [TestMethod]
        public void TestBadDataHandling()
        {
            // ��������, �� ��������� ������������ ����� � ������������ ������
            string badDataFileName = "11.txt";
            string filePath = Path.Combine(directoryPath, badDataFileName);
            string[] invalidData = { "abc", "123" };
            File.WriteAllLines(filePath, invalidData);

            ex.exceptions3.Main();

            string[] badDataContent = File.ReadAllLines(badDataPath);
            Assert.IsTrue(badDataContent.Contains(badDataFileName), "���� � ������������ ������ �� ���� ������ �� bad_data.txt.");
        }

        [TestMethod]
        public void TestOverflowHandling()
        {
            // ��������, �� ��������� ������������ �����, �� ���������� OverflowException
            string overflowFileName = "12.txt";
            string filePath = Path.Combine(directoryPath, overflowFileName);
            string[] overflowData = { int.MaxValue.ToString(), "2" };
            File.WriteAllLines(filePath, overflowData);

            ex.exceptions3.Main();

            string[] overflowContent = File.ReadAllLines(overflowPath);
            Assert.IsTrue(overflowContent.Contains(overflowFileName), "����, �� �������� OverflowException, �� ���� ������ �� overflow.txt.");
        }

        [TestMethod]
        public void TestEmptyFilesAreSkipped()
        {
            // ��������, �� ������ ����� �������������
            string emptyFileName = "13.txt";
            string filePath = Path.Combine(directoryPath, emptyFileName);
            File.WriteAllText(filePath, string.Empty); // ��������� ������� ����

            ex.exceptions3.Main();

            string[] badDataContent = File.ReadAllLines(badDataPath);
            Assert.IsTrue(badDataContent.Contains(emptyFileName), "������� ���� �� ���� ������ �� bad_data.txt.");
        }

        [TestMethod]
        public void TestAllFilesAreProcessed()
        {
            // ��������, �� �� ����� ������������
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                File.WriteAllLines(filePath, new string[] { "1", "2" }); // ���������� �� ����� �������� ������
            }

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ex.exceptions3.Main();

                string output = sw.ToString();
                Assert.IsTrue(output.Contains("������ �����������"), "�������� �� �������� �� ����� ��������.");
            }
        }
        [TestMethod]
        public void TestMultipleMissingFiles()
        {
            // �����������: ��������� ����� �����
            string[] missingFileNames = { "10.txt", "11.txt", "12.txt" };
            foreach (var fileName in missingFileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // ��������� ����, ���� �� ����
                }
            }

            // ������ ��������
            ex.exceptions3.Main();

            // �������� ����������
            Assert.IsTrue(File.Exists(noFilePath), "���� no_file.txt �� ���� ��������.");
            string[] noFileContent = File.ReadAllLines(noFilePath);

            foreach (var missingFileName in missingFileNames)
            {
                Assert.IsTrue(
                    !noFileContent.Contains(missingFileName),
                    $"���� {missingFileName} �� ��� ������� �� no_file.txt."
                );
            }
        }

        // ���� �� ��������� ������� ��� �������� ��� �����
        [TestMethod]
        public void TestNoErrorsWithValidData()
        {
            // ��������� ��� ����� � �������� ������
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                File.WriteAllLines(filePath, new[] { "1", "1" }); // �� ����� � �������� 1
            }

            // ������ ��������
            ex.exceptions3.Main();

            // ��������, �� ����� � ��������� ����������� ��������
            Assert.IsTrue(File.Exists(noFilePath), "���� no_file.txt �� ���� ��������.");
            Assert.IsTrue(File.Exists(badDataPath), "���� bad_data.txt �� ���� ��������.");
            Assert.IsTrue(File.Exists(overflowPath), "���� overflow.txt �� ���� ��������.");

            Assert.AreEqual(0, File.ReadAllLines(noFilePath).Length, "���� no_file.txt �� � �������.");
            Assert.AreEqual(0, File.ReadAllLines(badDataPath).Length, "���� bad_data.txt �� � �������.");
            Assert.AreEqual(0, File.ReadAllLines(overflowPath).Length, "���� overflow.txt �� � �������.");
        }

        // ���� �� ���������� ���������� ������������� � ��������� �������
        [TestMethod]
        public void TestAverageCalculationWithMultipleFiles()
        {
            // ��������� ����� � �������� ������
            string file1 = "10.txt";
            string file2 = "11.txt";
            File.WriteAllLines(Path.Combine(directoryPath, file1), new[] { "2", "3" }); // ������� 6
            File.WriteAllLines(Path.Combine(directoryPath, file2), new[] { "4", "5" }); // ������� 20

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // ������ ��������
                ex.exceptions3.Main();

                // ���������� ���������
                string expectedOutput = "������ �����������: 13"; // (6 + 20) / 2
                string actualOutput = sw.ToString().Trim();

                Assert.AreEqual(expectedOutput, actualOutput, "������ ����������� ��������� �����������.");
            }
        }

        //���� �� ������� ���������� �����
        [TestMethod]
        public void TestEmptyFileHandling()
        {
            // ��������� ���������� �����
            string emptyFileName = "10.txt";
            string filePath = Path.Combine(directoryPath, emptyFileName);
            File.WriteAllText(filePath, string.Empty);

            // ������ ��������
            ex.exceptions3.Main();

            // ��������, �� ���� �������� �� bad_data.txt
            Assert.IsTrue(File.Exists(badDataPath), "���� bad_data.txt �� ���� ��������.");
            string[] badDataContent = File.ReadAllLines(badDataPath);

            Assert.IsTrue(
                badDataContent.Contains(emptyFileName),
                $"������� ���� {emptyFileName} �� ��� ������� �� bad_data.txt."
            );
        }

        //���� �� �������� ��������� ��� ������� �����
        [TestMethod]
        public void TestMissingFilesAreCreated()
        {
            // ��������� �� �����
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // ������ ��������
            ex.exceptions3.Main();

            // ��������, �� �� ����� ��������
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(directoryPath, fileName);
                Assert.IsTrue(File.Exists(filePath), $"���� {fileName} �� ���� ��������.");
            }
        }
    }
}
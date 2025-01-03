using ExcelDataReader;
using Spire.Xls;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string folderPath = "../../../DataGenerator/XLSXS";
            string tempFolderPath = "../../../DataGenerator/TempXLSXS";
            string[] excelFiles = Directory.GetFiles(folderPath, "*.xlsx");

            // ----- 1. 임시 폴더 생성 -----
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            // ----- 2. 파일 복사 -----
            foreach (string file in excelFiles)
            {
                // 파일 이름이 ~$로 시작하는 경우 건너뜀
                if (Path.GetFileName(file).StartsWith("~$"))
                {
                    continue;
                }

                string tempFile = Path.Combine(tempFolderPath, Path.GetFileName(file));

                File.Copy(file, tempFile, true);
                Console.WriteLine("Copied file: " + Path.GetFileName(file));
            }

            // ----- 3. 복사한 파일로 csv 생성 -----
            foreach (string tempFile in Directory.GetFiles(tempFolderPath, "*.xlsx"))
            {

                // 파일 이름이 ~$로 시작하는 경우 건너뜀
                if (Path.GetFileName(tempFile).StartsWith("~$"))
                {
                    continue;
                }

                // 파싱 및 CSV 파일 생성
                Console.WriteLine("csv 생성: " + Path.GetFileName(tempFile));
                ParseExcel(tempFile);

                // Workbook 클래스의 인스턴스 생성
                Workbook workbook = new Workbook();

                // Excel 파일 로드
                workbook.LoadFromFile(tempFile);

                // 첫 번째 워크시트 가져오기
                Worksheet sheet = workbook.Worksheets[0];

                // 워크시트를 CSV로 저장
                sheet.SaveToFile($"../../../Client/Assets/Resources/CSV/{Path.GetFileNameWithoutExtension(Path.GetFileName(tempFile))}.csv", ",", Encoding.UTF8);
            }


            // 임시 폴더 및 폴더 내부의 파일 삭제
            Directory.Delete(tempFolderPath, true);
            Console.WriteLine("TempXLSXS 삭제 완료");

            Console.WriteLine("CSV파일 생성 & 코드 생성 완료");
            Console.ReadKey();
            Environment.Exit(0);
        }
         
        public static void ParseExcel(string file)
        {
            string excelName = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
            string dataRegister = string.Empty;
            string dataParse = string.Empty;


            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    DataTable table = result.Tables[0];


                    for (int columnIndex = 0; columnIndex <= table.Columns.Count - 1; columnIndex++)
                    {
                        var dataDescRow = table.Rows[0][columnIndex];
                        var dataNameRow = table.Rows[1][columnIndex];
                        var dataTypeRow = table.Rows[2][columnIndex];

                        string dataDesc;
                        string dataName;
                        string dataType;

                        DataColumn column = table.Columns[columnIndex];

                        if (dataNameRow == DBNull.Value)
                        {
                            continue;
                        }

                        if (dataTypeRow == DBNull.Value)
                        {
                            continue;
                        }

                        dataDesc = dataDescRow.ToString();
                        dataName = dataNameRow.ToString();
                        dataType = dataTypeRow.ToString();

                        if (dataDesc[0] == '#' || dataName[0] == '#' || dataType[0] == '#')
                        {
                            continue;
                        }

                        var toMemberType = ToMemberType(dataType.Replace("[]", ""));
                        if (toMemberType != string.Empty)
                        {
                            // 리스트 타입 처리
                            if (dataType.EndsWith("[]"))
                            {
                                string elementType = dataType.Replace("[]", "");
                                dataRegister += string.Format(DataFormat.dataRegisterListFormat, elementType, dataName, dataDesc) + Environment.NewLine;
                                dataParse += string.Format(DataFormat.dataParseListFormat, columnIndex.ToString(), dataName, ToMemberType(elementType)) + Environment.NewLine;
                            }
                            else
                            {
                                // 기본 자료형
                                dataRegister += string.Format(DataFormat.dataRegisterFormat, dataType, dataName, dataDesc) + Environment.NewLine;
                                dataParse += string.Format(DataFormat.dataParseFomat, columnIndex.ToString(), dataName, toMemberType) + Environment.NewLine;
                            }
                        }
                        else
                        {
                            // Enum 전용
                            dataRegister += string.Format(DataFormat.dataEnumRegisterFormat, dataType, dataName, dataDesc) + Environment.NewLine;
                            dataParse += string.Format(DataFormat.dataEnumParseFomat, columnIndex.ToString(), dataName, dataType) + Environment.NewLine;
                        }
                    }
                }
            }

            dataRegister = dataRegister.Replace("\n", "\n\t\t");
            dataParse = dataParse.Replace("\n", "\n\t\t\t\t\t");
            var dataManagerText = string.Format(DataFormat.dataFormat, excelName, dataRegister, dataParse);

            File.WriteAllText($"../../../Client/Assets/Scripts/DataSheets/{excelName}.cs", dataManagerText);
        }


        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                case "Bool":
                    return "ToBoolean";
                // case "byte": // 따로 정의되어있지는 않음
                case "short":
                case "Short":
                    return "ToInt16";
                case "ushort":
                case "Ushort":
                    return "ToUInt16";
                case "int":
                case "Int":
                    return "ToInt32";
                case "uint":
                case "Uint":
                    return "ToUInt32";
                case "long":
                case "Long":
                    return "ToInt64";
                case "ulong":
                case "Ulong":
                    return "ToUInt64";
                case "float":
                case "Float":
                    return "ToSingle";
                case "double":
                case "Double":
                    return "ToDouble";
                case "string":
                case "String":
                    return "ToString";
                default:
                    return string.Empty;
            }
        }
    }
}
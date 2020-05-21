/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XZOA.Code.Excel
{ 
    public class NPOIExcel
    {
        private string _title;
        private string _sheetName;
        private string _filePath;

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool ToExcel(DataTable table)
        {
            FileStream fs = new FileStream(this._filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workBook = new HSSFWorkbook();
            this._sheetName = this._sheetName.IsEmpty() ? "sheet1" : this._sheetName;
            ISheet sheet = workBook.CreateSheet(this._sheetName);

            //处理表格标题
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue(this._title);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
            row.Height = 500;

            ICellStyle cellStyle = workBook.CreateCellStyle();
            IFont font = workBook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 17;
            cellStyle.SetFont(font);
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            row.Cells[0].CellStyle = cellStyle;

            //处理表格列头
            row = sheet.CreateRow(1);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(table.Columns[i].ColumnName);
                row.Height = 350;
                sheet.AutoSizeColumn(i);
            }

            //处理数据内容
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = sheet.CreateRow(2 + i);
                row.Height = 250;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(table.Rows[i][j].ToString());
                    sheet.SetColumnWidth(j, 256 * 15);
                }
            }

            //写入数据流
            workBook.Write(fs);
            fs.Flush();
            fs.Close();

            return true;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        /// <param name="savePath"></param>
        public void Export(DataTable dt,string fileName,string savePath)
        {
            
            try
            {
                IWorkbook workbook;

                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                string fileExt = Path.GetExtension(fileName).ToLower();

                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = null;
                }

                if (workbook == null)
                {
                    return;
                }

                #region 填充数据

                ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

                StringBuilder strbu = new StringBuilder();

                ICellStyle dateStyle = workbook.CreateCellStyle();
                dateStyle.Alignment = HorizontalAlignment.Center;//字体居中
                ICellStyle style12 = workbook.CreateCellStyle();
                style12.Alignment = HorizontalAlignment.Center;//字体居中
                style12.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");//数字格式
                int[] arrColWidth = new int[dt.Columns.Count];

                foreach (DataColumn item in dt.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dt.Rows[i][j].ToString()).Length;
                        if(intTemp>20)
                        {
                            intTemp = 20;
                        }
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                //表头  
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                    sheet.SetColumnWidth(i, arrColWidth[i] * 300);
                    cell.CellStyle = dateStyle;
                }

                //数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        if (dt.Columns[j].DataType.FullName == "System.Decimal" || dt.Columns[j].DataType.FullName == "System.Int32")
                        {
                            cell.SetCellType(CellType.Numeric);
                            if (dt.Rows[i][j].ToString() == "")
                            {
                                cell.SetCellValue(0);
                            }
                            else {
                                cell.SetCellValue(double.Parse(dt.Rows[i][j].ToString()));
                            }
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                            cell.CellStyle = dateStyle;
                        }
                    }
                }

                #endregion

                //转为字节数组  
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                var buf = stream.ToArray();
                //保存为Excel文件
                //创建文件
                using (FileStream fs = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 汇总
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        /// <param name="savePath"></param>
        public void GatherExport(DataTable dt, string fileName, string savePath)
        {
            IWorkbook workbook;
            
            //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
            string fileExt = Path.GetExtension(fileName).ToLower();

            if (fileExt == ".xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileExt == ".xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = null;
            }

            if (workbook == null)
            {
                return;
            }
            #region 填充数据
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            StringBuilder strbu = new StringBuilder();

            ICellStyle dateStyle = workbook.CreateCellStyle();
            dateStyle.Alignment = HorizontalAlignment.Center;//字体居中
            int[] arrColWidth = new int[dt.Columns.Count];
            foreach (DataColumn item in dt.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dt.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            //表头  
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.CellStyle = dateStyle;
                cell.SetCellValue(dt.Columns[i].ColumnName);
                sheet.SetColumnWidth(i, arrColWidth[i]*300);
            }

            string Account = dt.Rows[0]["工号"].ToString();
            Dictionary<int, string> keyValues = new Dictionary<int, string>();

            string UserName= dt.Rows[0]["请假人姓名"].ToString();
            int k = 1;
            decimal TotalDay=0, TotalHour=0;
            //数据  
            for (int i = 0; i <= dt.Rows.Count; i++)
            {

                if (i == dt.Rows.Count)//最后一行
                {
                    GatherRow(sheet, dt, UserName, TotalDay, TotalHour, (i+ k ), dateStyle);
                }
                else
                {
                    if (Account==dt.Rows[i]["工号"].ToString())
                    {
                        TotalDay += Convert.ToDecimal(dt.Rows[i]["天数"].ToString());
                        TotalHour += Convert.ToDecimal(dt.Rows[i]["请假合计（H）"].ToString());
                        CreateRow(sheet, dt, (i + k), i, dateStyle);
                    }
                    else
                    {
                        GatherRow(sheet, dt,UserName,TotalDay,TotalHour, ( i + k ), dateStyle);
                        Account = dt.Rows[i]["工号"].ToString();
                        UserName = dt.Rows[i]["请假人姓名"].ToString();
                        TotalDay = 0; TotalHour = 0;
                        k++;
                        TotalDay += Convert.ToDecimal(dt.Rows[i]["天数"].ToString());
                        TotalHour += Convert.ToDecimal(dt.Rows[i]["请假合计（H）"].ToString());
                        CreateRow(sheet, dt, (i + k),i, dateStyle);
                    }
                }
                
             }

            #endregion

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();
            //保存为Excel文件
            //创建文件
            using (FileStream fs = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }

        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <param name="i"></param>
        /// <param name="dateStyle"></param>
        public void CreateRow(ISheet sheet,DataTable dt,int row,int i, ICellStyle dateStyle)
        {
            IRow row1 = sheet.CreateRow(row);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                cell.SetCellValue(dt.Rows[i][j].ToString());
                cell.CellStyle = dateStyle;
            }
        }

        /// <summary>
        /// 汇总行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="dt"></param>
        /// <param name="i"></param>
        /// <param name="dateStyle"></param>
        public void GatherRow(ISheet sheet, DataTable dt,string UserName, decimal TotalDay,decimal TotalHour, int i, ICellStyle dateStyle)
        {
            IRow row1 = sheet.CreateRow(i);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                cell.CellStyle = dateStyle;
                cell.SetCellValue("");
                if (j == 5)
                {
                    cell.SetCellValue(UserName + "汇总");
                }
                if (j == 7)
                {
                    cell.SetCellValue(TotalDay.ToString());
                }
                if (j == 8)
                {
                    cell.SetCellValue(TotalHour.ToString());
                }
            }
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="title"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public bool ToExcel(DataTable table, string title, string sheetName, string filePath)
        {
            this._title = title;
            this._sheetName = sheetName;
            this._filePath = filePath;
            return ToExcel(table);
        }
    }
}

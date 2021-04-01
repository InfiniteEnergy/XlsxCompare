using System;
using System.IO;
using OfficeOpenXml;

namespace XlsxCompare
{

    /// <summary>
    /// wraps <see cref="ExcelPackage"/>
    /// </summary>
    sealed class XlsxFacade : IDisposable
    {
        readonly ExcelPackage _excel;

        public XlsxFacade(string path) : this(new FileInfo(path)) { }

        public XlsxFacade(FileInfo file)
        {
            _excel = new ExcelPackage(file);
        }

        public void Dispose() => _excel.Dispose();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Datatables.Mvc;
using System.Web.Mvc;
using System.Text;

namespace Mvc4Application.AppSpike.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult GetDataTables(DataTable dataTable) {
            List<List<string>> table = new List<List<string>>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++) {
                column1.Add(i);
            }

            foreach (var sortDir in dataTable.sSortDirs) {
                if (sortDir == DataTableSortDirection.Ascending) {
                    column1 = column1.OrderBy(x => x).ToList();
                } else {
                    column1 = column1.OrderByDescending(x => x).ToList();                    
                }
            }

            for (int i = 0; i < column1.Count; i++) {
                table.Add(new List<string> { column1[i].ToString(), "Nummer" + i });
            }

            var result = new DataTableResult(dataTable, table.Count, table.Count, table);
            result.ContentEncoding = Encoding.UTF8;
            return result;
        }

        public ActionResult Index2() {
            return View();
        }

        [HttpPost]
        public ActionResult GetDataTables2(DataTable dataTable) {
            List<List<string>> table = new List<List<string>>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++) {
                column1.Add(i);
            }

            foreach (var sortDir in dataTable.sSortDirs) {
                if (sortDir == DataTableSortDirection.Ascending) {
                    column1 = column1.OrderBy(x => x).ToList();
                } else {
                    column1 = column1.OrderByDescending(x => x).ToList();
                }
            }

            for (int i = 0; i < column1.Count; i++) {
                table.Add(new List<string> { column1[i].ToString(), "ÄÖÜäöü" + i });
            }

            var result = new DataTableResult(dataTable, table.Count, table.Count, table);
            result.ContentEncoding = Encoding.UTF8;
            return result;
        }

        public ActionResult Index3() {
            return View();
        }

        [HttpPost]
        public ActionResult GetDataTables3(DataTable dataTable) {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++) {
                column1.Add(i);
            }

            foreach (var sortDir in dataTable.sSortDirs) {
                if (sortDir == DataTableSortDirection.Ascending) {
                    column1 = column1.OrderBy(x => x).ToList();
                } else {
                    column1 = column1.OrderByDescending(x => x).ToList();
                }
            }

            for (int i = 0; i < column1.Count; i++) {
                table.Add(new DataTableRow("rowId" + i.ToString(), "dtrowclass") { column1[i].ToString(), "ÄÖÜäöü" + i });
            }
            
            return new DataTableResultExt(dataTable, table.Count, table.Count, table);
        }
    }
}

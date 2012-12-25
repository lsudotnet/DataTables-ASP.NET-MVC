#region License
//Copyright (c) 2012 Johannes Brunner


//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:


//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.


//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Datatables.Mvc {

    /// <summary>
    /// This class represents an MVC Action result for
    /// a jquery.datatables response.
    /// </summary>
    public class DataTableResultExt : ActionResult, IDataTableResult<List<DataTableRow>> {

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public int iTotalRecords { get; set; }

        public string sColumns { get; set; }

        public string sEcho { get; protected set; }

        public List<DataTableRow> aaData { get; set; }


        public DataTableResultExt(DataTable dataTable, int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<DataTableRow> aaData = null)
            : this(dataTable.sEcho, iTotalRecords, iTotalDisplayRecords, aaData) {
        }

        public DataTableResultExt(string sEcho = "", int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<DataTableRow> aaData = null) {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            this.sEcho = sEcho;
            this.iTotalRecords = iTotalRecords;
            this.iTotalDisplayRecords = iTotalDisplayRecords;
            this.aaData = aaData;
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) {
                throw new InvalidOperationException("Get not allowed");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType)) {
                response.ContentType = this.ContentType;
            } else {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null) {
                response.ContentEncoding = this.ContentEncoding;
            }
                                    
            using (JsonWriter writer = new JsonTextWriter(response.Output)) {                
                writer.WriteStartObject();

                writer.WritePropertyName("sEcho");
                writer.WriteValue(this.sEcho);

                writer.WritePropertyName("iTotalRecords");
                writer.WriteValue(this.iTotalRecords);

                writer.WritePropertyName("iTotalDisplayRecords");
                writer.WriteValue(this.iTotalDisplayRecords);

                writer.WritePropertyName("aaData");
                writer.WriteStartArray();
                for (int i = 0; i < aaData.Count; i++) {
                    writer.WriteStartObject();
                    DataTableRow row = aaData[i];
                    if (row.DT_RowId != null) {
                        writer.WritePropertyName("DT_RowId");
                        writer.WriteValue(row.DT_RowId);
                    }

                    if (row.DT_RowClass != null) {
                        writer.WritePropertyName("DT_RowClass");
                        writer.WriteValue(row.DT_RowClass);
                    }

                    for (int j = 0; j < row.Count; j++) {
                        writer.WritePropertyName(j.ToString());
                        writer.WriteValue(row[j]);                        
                    }
                    writer.WriteEndObject();
                }                
                writer.WriteEndArray();    
                writer.WriteEndObject();
            }            
        }
    }
}
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Datatables.Mvc {

    /// <summary>
    /// This class represents an MVC Action result for
    /// a jquery.datatables response.
    /// </summary>    
    public class DataTableResult : ActionResult, IDataTableResult<List<List<string>>> {

        /// <summary>
        /// An unaltered copy of sEcho sent from the client side. 
        /// This parameter will change with each draw (it is basically a draw count) - 
        /// so it is important that this is implemented. Note that it strongly recommended 
        /// for security reasons that you 'cast' this parameter to an integer 
        /// in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>                        
        public string sEcho { get { return SerializationData.sEcho; } }

        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>                
        public int iTotalRecords { get { return SerializationData.iTotalRecords; } set { SerializationData.iTotalRecords = value; } }

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - 
        /// not just the number of records being returned in this result set)
        /// </summary>        
        public int iTotalDisplayRecords { get { return SerializationData.iTotalDisplayRecords; } set { SerializationData.iTotalDisplayRecords = value; } }

        /// <summary>
        /// Optional - this is a string of column names, comma separated (used in combination with sName) 
        /// which will allow DataTables to reorder data on the client-side if required for display
        /// </summary>                
        public string sColumns { get { return SerializationData.sColumns; } set { SerializationData.sColumns = value; } }

        /// <summary>
        /// The data in a 2D array
        /// Fill this structure with the plain table data
        /// represented as string.
        /// </summary>                
        public List<List<string>> aaData { get { return SerializationData.aaData; } set { SerializationData.aaData = value; } }
         
        /// <summary>
        /// This property sets, gets the encoding used to serialize the .Net object to json.
        /// If this property is not set, the encoding from the Response is used.
        /// </summary>               
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// This property sets, gets the content Type.
        /// The default value is application/json
        /// </summary>        
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the json request behavior.
        /// The default behaviour is DenyGet requests.
        /// </summary>
        /// <value>
        /// The json request behavior.
        /// </value>        
        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        internal DataTableSerializationData SerializationData { get; set; }
        
        public DataTableResult(DataTable dataTable, int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<List<string>> aaData = null)
            : this(dataTable.sEcho, iTotalRecords, iTotalDisplayRecords, aaData) {
        }       

        public DataTableResult(string sEcho = "", int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<List<string>> aaData = null) {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            this.SerializationData = new DataTableSerializationData {
                sEcho = sEcho,
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalDisplayRecords,
                aaData = aaData
            };
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

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DataTableSerializationData));            
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, this.SerializationData);
            string json = response.ContentEncoding.GetString(ms.ToArray());            

            response.Write(json);
        }
    }   
}
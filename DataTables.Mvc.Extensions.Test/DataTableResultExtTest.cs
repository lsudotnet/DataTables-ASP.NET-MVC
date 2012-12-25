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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Net;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.IO;
using Datatables.Mvc;

namespace jquery.dataTables.Test {
    
    [TestClass]
    public class DataTableResultExtTest {


        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void MyClassCleanup() { }

        [TestInitialize]
        public void MyTestInitialize() { }

        [TestCleanup]
        public void MyTestCleanup() { }

        #endregion

        [TestMethod]
        public void DataTableResult_full_parameter_list() {
            var data = new List<DataTableRow>() {
                new DataTableRow { "hallo" }
            };
            DataTableResultExt dataTableResult = new DataTableResultExt("1", 10, 10, data);
            Assert.AreEqual("1", dataTableResult.sEcho);
            Assert.AreEqual(10, dataTableResult.iTotalRecords);
            Assert.AreEqual(10, dataTableResult.iTotalDisplayRecords);
            Assert.AreSame(data, dataTableResult.aaData);
            Assert.AreEqual(JsonRequestBehavior.DenyGet, dataTableResult.JsonRequestBehavior);
        }

        [TestMethod]
        public void DataTableResult_full_parameter_list_from_DataTable() {
            var data = new List<DataTableRow>() {
                new DataTableRow { "hallo" }
            };
            DataTableResultExt dataTableResult = new DataTableResultExt(new DataTable { sEcho = "1" }, 10, 10, data);
            Assert.AreEqual("1", dataTableResult.sEcho);
            Assert.AreEqual(10, dataTableResult.iTotalRecords);
            Assert.AreEqual(10, dataTableResult.iTotalDisplayRecords);
            Assert.AreSame(data, dataTableResult.aaData);
            Assert.AreEqual(JsonRequestBehavior.DenyGet, dataTableResult.JsonRequestBehavior);
        }

        [TestMethod]
        public void DataTableResult_from_DataTable() {
            DataTableResult dataTableResult = new DataTableResult(new DataTable { sEcho = "1" });
            Assert.AreEqual("1", dataTableResult.sEcho);
            Assert.AreEqual(0, dataTableResult.iTotalRecords);
            Assert.AreEqual(0, dataTableResult.iTotalDisplayRecords);
            Assert.AreSame(null, dataTableResult.aaData);
            Assert.AreEqual(JsonRequestBehavior.DenyGet, dataTableResult.JsonRequestBehavior);
        }

        [TestMethod]
        public void ExecuteResult_Ok() {
            //Arrange
            var data = new List<DataTableRow>() {
                new DataTableRow { "hallo", "österreich"}
            };

            var httpRequest = new Mock<HttpRequestBase>();
            httpRequest.Setup(h => h.HttpMethod)
                       .Returns("POST");
            var httpResponse = new Mock<HttpResponseBase>();
            httpResponse.Setup(x => x.ContentEncoding)
                        .Returns(Encoding.UTF8);            
            StringBuilder result = new StringBuilder();
            httpResponse.Setup(x => x.Output)
                        .Returns(new StringWriter(result));
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(h => h.Request)
                       .Returns(httpRequest.Object);
            httpContext.Setup(h => h.Response)
                       .Returns(httpResponse.Object);
            DataTableResultExt dataTableResult = new DataTableResultExt("1", 10, 10, data);
            //Act            
            dataTableResult.ExecuteResult(new ControllerContext {
                HttpContext = httpContext.Object
            });
            //Assert
            Assert.AreEqual(@"{""sEcho"":""1"",""iTotalRecords"":10,""iTotalDisplayRecords"":10,""aaData"":[{""0"":""hallo"",""1"":""österreich""}]}", result.ToString());
        }

        [TestMethod]
        public void ExecuteResult_DT_RowId_Ok() {
            //Arrange
            var data = new List<DataTableRow>() {
                new DataTableRow("1", "row1")  { "hallo","österreich"}
            };

            var httpRequest = new Mock<HttpRequestBase>();
            httpRequest.Setup(h => h.HttpMethod)
                       .Returns("POST");
            var httpResponse = new Mock<HttpResponseBase>();
            httpResponse.Setup(x => x.ContentEncoding)
                        .Returns(Encoding.UTF8);            
            StringBuilder result = new StringBuilder();
            httpResponse.Setup(x => x.Output)
                        .Returns(new StringWriter(result));
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(h => h.Request)
                       .Returns(httpRequest.Object);
            httpContext.Setup(h => h.Response)
                       .Returns(httpResponse.Object);
            DataTableResultExt dataTableResult = new DataTableResultExt("1", 10, 10, data);
            //Act            
            dataTableResult.ExecuteResult(new ControllerContext {
                HttpContext = httpContext.Object
            });
            //Assert
            Assert.AreEqual(@"{""sEcho"":""1"",""iTotalRecords"":10,""iTotalDisplayRecords"":10,""aaData"":[{""DT_RowId"":""1"",""DT_RowClass"":""row1"",""0"":""hallo"",""1"":""österreich""}]}", result.ToString());
        }
    }
}

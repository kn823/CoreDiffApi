using CoreDiffApi.Models;
using CoreDiffApi.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using Xunit;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace xUnitDiffApi.System.Services
{
    [DataContract]
    public class DiffResponse
    {
        [DataMember]
        public string DiffResultType { get; set; }
    }

    public class Diff
    {
        public string Position { get; set; }
        public string Difference { get; set; }
    }

    public class DiffDetailsResponse
    {
        public List<Diff> Diffs { get; set; }

        public string DiffResultType { get; set; }
    }

    public class TestDiff
    {
        public static int intInputId = 0;
        
        /// <summary>
        /// Test Left Base64 encode = Right Base64 encode.  diffResultType is Equals
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShoudReturnEquals()
        {
            string strInputId = Convert.ToString(++intInputId);
            var options = new DbContextOptionsBuilder<DiffInputDBContext>()
            .UseInMemoryDatabase("DiffInputDB")
            .Options;
            var _context = new DiffInputDBContext(options);
            _context.Database.EnsureCreated();
            DiffOp _diffOp = new DiffOp(_context);

            DiffInput leftInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "AAAAAA==",
                DataRight = ""
            };
            DiffInput rightInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "",
                DataRight = "AAAAAA=="
            };
            bool ret;
            ret = await _diffOp.Save(leftInput);
            ret = await _diffOp.Save(rightInput);
            string jsonData = await _diffOp.Diff(strInputId);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DiffResponse));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            stream.Position = 0;
            DiffResponse diffResponse = (DiffResponse)jsonSerializer.ReadObject(stream);

            Assert.Equal("Equals", diffResponse.DiffResultType);
        }

        /// <summary>
        /// Test Left Base64 encode <> Right Base64 encode.  diffResultType is ContentDoNotMatch
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShoudReturnContentDoNotMatch()
        {
            string strInputId = Convert.ToString(++intInputId);
            var options = new DbContextOptionsBuilder<DiffInputDBContext>()
            .UseInMemoryDatabase("DiffInputDB")
            .Options;
            var _context = new DiffInputDBContext(options);
            _context.Database.EnsureCreated();
            DiffOp _diffOp = new DiffOp(_context);

            DiffInput leftInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "AAAAAA==",
                DataRight = ""
            };
            DiffInput rightInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "",
                DataRight = "AQABAQ=="
            };
            bool ret;
            ret = await _diffOp.Save(leftInput);
            ret = await _diffOp.Save(rightInput);
            string jsonData = await _diffOp.Diff(strInputId);
            JToken token = JObject.Parse(jsonData);
            string diffResultType = (string)token.SelectToken("diffResultType");
            Assert.Equal("ContentDoNotMatch", diffResultType);
        }

        /// <summary>
        /// Test Left Base64 encode < Right Base64 encode.  diffResultType is SizeDoNotMatch
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShoudReturnSizeDoNotMatch()
        {
            string strInputId = Convert.ToString(++intInputId);
            var options = new DbContextOptionsBuilder<DiffInputDBContext>()
            .UseInMemoryDatabase("DiffInputDB")
            .Options;
            var _context = new DiffInputDBContext(options);
            _context.Database.EnsureCreated();
            DiffOp _diffOp = new DiffOp(_context);

            string id = "456";
            DiffInput leftInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "AAAAAA==",
                DataRight = ""
            };
            DiffInput rightInput = new DiffInput
            {
                Id = strInputId,
                DataLeft = "",
                DataRight = "AAAAAAAA"
            };
            bool ret;
            ret = await _diffOp.Save(leftInput);
            ret = await _diffOp.Save(rightInput);
            string jsonData = await _diffOp.Diff(strInputId);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DiffResponse));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            stream.Position = 0;
            DiffResponse diffResponse = (DiffResponse)jsonSerializer.ReadObject(stream);

            Assert.Equal("SizeDoNotMatch", diffResponse.DiffResultType);
        }
        
    }
}

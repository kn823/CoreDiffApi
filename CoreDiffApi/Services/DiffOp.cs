using CoreDiffApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Web.Http;

namespace CoreDiffApi.Services
{
    public class DiffOp : IDiffOp
    {
        private readonly DiffInputDBContext _context;

        /// <summary>
        /// Inject DBContext service at the start of this DiffOp service.
        /// </summary>
        /// <param name="context">Injected DBContext</param>
        public DiffOp(DiffInputDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initializing Id & base64 encode in DiffInput instance and calling dbcontext to save away in In-Memory database 
        /// If input Id already exists, update record.  Otherwise add a new record. 
        /// </summary>
        /// <param name="diffInput">DiffInput instance has the input Id & Based64 encode</param>
        /// <returns>true if saved successfully</returns>
        public async Task<bool> Save(DiffInput diffInput)
        {
            DiffInput _diffInput = _context.DiffInputs.FirstOrDefault(x => x.Id == diffInput.Id);
            if (_diffInput == null)
            {
                _context.DiffInputs.AddAsync(diffInput);
            }
            else
            {
                if (String.IsNullOrEmpty(diffInput.DataLeft))
                {
                    _diffInput.DataRight = diffInput.DataRight;
                }
                else
                {
                    _diffInput.DataLeft = diffInput.DataLeft;

                }
                _context.DiffInputs.Update(_diffInput);
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Given the input Id, do a diff between left base64 encode and right base64 encode that were saved.
        /// Diff only occurs if both encodes exist and diff response is "Equals", "ContentDoNotMatch" or "SizeDoNotMatch".
        /// If both encodes are equal in length, but different in content, position(s) and value(s) of mismatch(es) will also be returned.
        /// </summary>
        /// <param name="id">Input Id for the two Base64 encodes to be diff-ed</param>
        /// <returns>Diff response in a JSON-formatted string.</returns>
        public async Task<string> Diff(string id)
        {
            string jsonString;

            DiffInput diffInput = _context.DiffInputs.FirstOrDefault(x => x.Id == id);
            if (diffInput == null || String.IsNullOrEmpty(diffInput.DataLeft) || String.IsNullOrEmpty(diffInput.DataRight))
                return "";

            Byte[] left = Convert.FromBase64String(diffInput.DataLeft);
            Byte[] right = Convert.FromBase64String(diffInput.DataRight);

            JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            bool bEqualLength = (left.Length == right.Length);
            if (bEqualLength == false || left.SequenceEqual(right))
            {
                DiffOutput diffOutput = new DiffOutput();
                diffOutput.DiffResultType = bEqualLength ? DiffResponseConstants.DR_EQUALS : DiffResponseConstants.DR_SIZE_NOT_MATCH;
                jsonString = System.Text.Json.JsonSerializer.Serialize<Object>(diffOutput, jsonOptions);
            }
            else
            {
                DiffOutputDiffs diffOutput = new DiffOutputDiffs();
                diffOutput.DiffResultType = DiffResponseConstants.DR_CONTENT_NOT_MATCH;

                int numDiffBits = 0;
                for (int i = 0; i < left.Length; i++)
                {
                    byte diffBits = (byte)(left[i] ^ right[i]);
                    if (diffBits != 0)
                    {
                        string strBinary = Convert.ToString(diffBits, 2).PadLeft(8, '0');
                        string strLeft = Convert.ToString(left[i], 2).PadLeft(8, '0');

                        for (int j = 0; j < strBinary.Length; j++)
                        {
                            if (strBinary[j].Equals('1'))
                            {
                                string detDiff = "L:" + strLeft[j] + " R:" + (strLeft[j] == '0' ? "1" : "0");
                                diffOutput.Diffs.Add(numDiffBits, detDiff);
                            }
                            numDiffBits++;
                        }
                    }
                    else
                        numDiffBits += 8;
                }

                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false
                    }
                };
                jsonString = JsonConvert.SerializeObject(diffOutput, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            }
            return jsonString;               
        }

        /// <summary>
        /// Validate POST input id and base64 encode data.
        /// </summary>
        /// <param name="id">Input Id for Base64 encode data to be saved.</param>
        /// <param name="input">Input Base64 encode data to be saved.</param>
        /// <returns></returns>
        public bool IsDataValid(string id, string input)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) ||
                input == null ||
                string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            try
            {
                var base64String = Convert.FromBase64String(input);
            }
            catch (FormatException ex)
            {
                return false;
            }

            return true;
        }
    }
}

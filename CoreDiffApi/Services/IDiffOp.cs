using CoreDiffApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreDiffApi.Services
{
    /// <summary>
    /// IDiffOp interface 
    /// </summary>
    public interface IDiffOp
    {
        public Task<bool> Save(DiffInput diffInput);
        public Task<string> Diff(string id);
        public bool IsDataValid(string id, string input);
    }
}

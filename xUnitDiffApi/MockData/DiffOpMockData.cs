using CoreDiffApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitDiffApi.MockData
{
    public class DiffOpMockData
    {
        public static DiffInput NewDiffInput()
        {
            return new DiffInput
            {
                Id = "123",
                DataLeft = "AAAAAA==",
                DataRight = ""
            };
        }
    };
}

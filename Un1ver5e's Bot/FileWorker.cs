using System;
using System.Collections.Generic;
using System.Text;

namespace Un1ver5e.FileWorker
{
    public static class FileWorker
    {
        public interface IСsvStorable
        {
            public string CsvStore();
            public object[] CsvGet();
        }

    }
}

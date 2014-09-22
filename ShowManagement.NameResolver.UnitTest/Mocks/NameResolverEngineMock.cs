using ShowManagement.NameResolver.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.UnitTest.Mocks
{
    public class NameResolverEngineMock : INameResolverEngine
    {
        public async Task Start()
        {
        }

        public async Task Stop()
        {
        }

        public async Task Add(string fileName)
        {
            await this.Add(new List<string> { fileName }, 3);
        }
        public async Task Add(string fileName, int retryAttempts)
        {
            await this.Add(new List<string> { fileName }, retryAttempts);
        }
        public async Task Add(IEnumerable<string> fileNames)
        {
            await this.Add(fileNames, 3);
        }
        public async Task Add(IEnumerable<string> fileNames, int retryAttempts)
        {
            this.FileNames.Clear();

            if (fileNames != null)
            {
                this.FileNames.AddRange(fileNames);
            }

            this.RetryAttempts = retryAttempts;
        }

        public async Task Remove(string fileName)
        {
            await this.Remove(new List<string> { fileName });
        }
        public async Task Remove(IEnumerable<string> fileNames)
        {
            this.FileNames.RemoveAll(fn => FileNames.Contains(fn));
        }

        public List<string> FileNames = new List<string>();
        public int RetryAttempts = 0;
    }
}

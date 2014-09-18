using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services
{
    public interface INameResolverService
    {
        Task Start();
        Task Stop();

        Task Add(string fileName);
        Task Add(string fileName, int retryAttempts);
        Task Add(IEnumerable<string> fileNames);
        Task Add(IEnumerable<string> fileNames, int retryAttempts);

        Task Remove(string filePath);
        Task Remove(IEnumerable<string> filePaths);
    }
}

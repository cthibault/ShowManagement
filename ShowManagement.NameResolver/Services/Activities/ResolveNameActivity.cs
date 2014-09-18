using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services.Activities
{
    class ResolveNameActivity : Activity
    {
        public ResolveNameActivity(string filePath, int maxRetryAttempts)
            : base(maxRetryAttempts)
        {
            this.FilePath = filePath;
        }

        public override async Task<IActivity> Perform()
        {
            Trace.WriteLine("Performing Activity: " + this.ToString());

            FileInfo validFileInfo = this.FindAndValidateFileInfo();



            IActivity nextActivity = this.BuildNextActivity(validFileInfo != null);

            return nextActivity;
        }
        public override async Task Cancel()
        {
            this.IsCancelled = true;

            Trace.WriteLine("Canceling Activity: " + this.ToString());
        }

        public string FilePath { get; private set; }

        public override bool Equals(IActivity other)
        {
            bool isEquals = false;

            var activity = other as ResolveNameActivity;

            if (activity != null)
            {
                isEquals = this.FilePath.Equals(activity.FilePath);
            }

            return isEquals;
        }
        protected override int GetHashCodeImplementation()
        {
            return this.FilePath.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.FilePath, this.MaxRetryAttempts.ToString());
        }


        private IActivity BuildNextActivity(bool validFileInfoFound)
        {
            IActivity nextActivity = null;

            if (validFileInfoFound && this.MaxRetryAttempts > 0)
            {
                nextActivity = new ResolveNameActivity(this.FilePath, this.MaxRetryAttempts - 1);
            }

            return nextActivity;
        }
        

        /// <summary>
        /// Find File on FS and Validate Directory structure
        /// </summary>
        /// <returns>FileInfo if valid to process</returns>
        private FileInfo FindAndValidateFileInfo()
        {
            var fileInfo = new FileInfo(this.FilePath);

            if (!fileInfo.Exists)
            {
                Trace.WriteLine(string.Format("File Does Not Exist: {0}", fileInfo.FullName));
                fileInfo = null;
            }
            else if (fileInfo.Directory == null || !fileInfo.Directory.Exists)
            {
                Trace.WriteLine(string.Format("Season Directory Does Not Exist: {0}", fileInfo.Directory != null ? fileInfo.Directory.FullName : string.Empty));
                fileInfo = null;
            }
            else if (fileInfo.Directory.Parent == null || !fileInfo.Directory.Parent.Exists)
            {
                Trace.WriteLine(string.Format("Show Directory Does Not Exist: {0}", fileInfo.Directory.Parent != null ? fileInfo.Directory.Parent.FullName : string.Empty));
                fileInfo = null;
            }

            return fileInfo;
        }
        
        /// <summary>
        /// Parse Series Information
        /// </summary>
        /// <param name="fileInfo"></param>
        private void Parse(FileInfo fileInfo)
        {
            if (fileInfo != null)
            {
                var showDirectory = fileInfo.Directory.Parent;
            }
        }


        //Retrieve Episode Information
        
        //Rename File
    }
}

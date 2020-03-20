using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface IProcessor
    {
        void Dispose();
        IEnumerable<string> Process(string response);
        Task<IEnumerable<string>> ProcessAsync(string response);
    }

    public class Processor : IProcessor, IDisposable
    {
        private bool disposed = false;
        private readonly IDownloader _downloader;
        private readonly IFeeder _feeder;

        public Processor(IDownloader downloader, IFeeder feeder)
        {
            _downloader = downloader;
            _feeder = feeder;
        }

        /// <summary>
        /// Extracts data from an internet response.
        /// </summary>
        /// <param name="responseBody">internet response</param>
        public IEnumerable<string> Process(string response)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> ProcessAsync(string response)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                _downloader.Dispose();

            disposed = true;
        }

        ~Processor()
        {
            Dispose(false);
        }
    }
}

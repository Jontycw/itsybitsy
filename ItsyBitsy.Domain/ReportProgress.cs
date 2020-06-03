namespace ItsyBitsy.Domain
{
    internal class ReportProgress : CrawlWorkerBase
    {
        private readonly ICrawlProgress _progress;

        public ReportProgress(ICrawlProgress progress, bool separateThread = true)
            : base(separateThread)
        {
            _progress = progress;
        }

        protected override void DoWorkInternal()
        {

        }

        protected override bool TerminateCondition()
        {
            throw new System.NotImplementedException();
        }
    }
}
namespace ItsyBitsy.Domain
{
    internal class ReportProgress : CrawlWorkerBase
    {
        private ICrawlProgress _progress;

        public ReportProgress(ICrawlProgress progress)
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
using System;

namespace MovieCatalog.Models
{
    public class RepositoryOperationResult
    {
        public RepositoryOperationResult( DateTime startDateTime, DateTime endDateTime, RepositoryOperationResultState state )
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            State = state;
        }

        public RepositoryOperationResult(string operationName)
        {
            OperationName = operationName;
        }

        public string OperationName { get; private set; }

        public RepositoryOperationResultState State { get; internal set; }

        public long MaxIteration { get; internal set; }

        public long IterationDone { get; internal set; }

        public int ProgressInPercent
        {
            get { return MaxIteration > 0 ? (int)(IterationDone * 100 / MaxIteration) : 0; }
        }

        public DateTime StartDateTime { get; internal set; }

        public DateTime EndDateTime { get; internal set; }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan returnValue = TimeSpan.Zero;
                switch (State)
                {
                    case RepositoryOperationResultState.InProcess:
                        returnValue = DateTime.Now - StartDateTime;
                        break;

                    case RepositoryOperationResultState.Failed:
                    case RepositoryOperationResultState.Success:
                        returnValue = EndDateTime - StartDateTime;
                        break;
                }
                
                return returnValue;
            }
        }
    }
}
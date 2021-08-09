using PITL.Extract.Job.Abstractions.Input;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PITL.Extract.Job
{
    public class ExtractJob
    {
        private readonly IPositionReader _positionReader;
        private readonly ICsvFileCreator _csvFileCreator;

        public ExtractJob(IPositionReader positionReader, ICsvFileCreator csvFileCreator)
        {
            _positionReader = positionReader ?? throw new ArgumentNullException(nameof(positionReader));
            _csvFileCreator = csvFileCreator ?? throw new ArgumentNullException(nameof(csvFileCreator));
        }
        
        public void FireAndForget(DateTime extractTime, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(a => DoWork(extractTime, cancellationToken), cancellationToken);
        }

        private void DoWork(DateTime extractTime, CancellationToken cancellationToken)
        {
            var positions = _positionReader.GetPositionsExtract(extractTime, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;
            _csvFileCreator.Create(extractTime, positions);
        }
    }
}

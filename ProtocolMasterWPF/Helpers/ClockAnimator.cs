using ProtocolMasterCore.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.Helpers
{
    internal delegate void ClockUpdate(double elapsed, double duration);
    internal class ClockAnimator
    {
        public ClockUpdate OnUpdate;
        public DateTime StartDateTime { get; private set; }
        public double StartTime { get; private set; }
        public double Duration { get; private set; }
        public Task AnimTask { get; private set; }
        Progress<int> animationProgress;
        CancellationTokenSource tokenSource;

        public ClockAnimator()
        {
            PrepAnimator();
        }
        public void FindMaxTime(List<ProtocolEvent> events, string label)
        {
            if (events == null) return;
            double milliseconds = events.Where(i => i.Arguments.ContainsKey("TimeEndMs")).Max(i => Convert.ToDouble( i.Arguments["TimeEndMs"]));
            Duration = TimeSpan.FromMilliseconds(milliseconds).TotalDays;
        }
        public void PrepAnimator()
        {
            animationProgress = new Progress<int>();
            tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            animationProgress.ProgressChanged += AnimatorProgress;
            AnimTask = new Task(() =>
            {
                AnimatorLoop(animationProgress, cancelToken);
            }, cancelToken);
        }
        public void StartAnimator(DateTime startTime)
        {
            StartDateTime = startTime;
            StartTime = StartDateTime.ToOADate();
            AnimTask.Start();
        }
        public void StartAnimatorNow() => StartAnimator(DateTime.Now);
        public void StopAnimator()
        {
            tokenSource?.Cancel();
            PrepAnimator();
        }
        void AnimatorProgress(object sender, int e)
        {
            if (!tokenSource.IsCancellationRequested)
            {
                double elapsed = (DateTime.Now.ToOADate() - StartTime);
                OnUpdate?.Invoke(elapsed, Duration);
            }
        }
        void AnimatorLoop(IProgress<int> progress, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                Thread.Sleep(256);
                progress.Report(1);
            }
            progress.Report(0);
        }
    }
}

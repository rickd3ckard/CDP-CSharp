/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;

namespace CDP.Objects
{
    public class Waiter
    {
        public Waiter(Tab Parent, TimeSpan? TimeOut = null, TimeSpan? RateLimit = null)
        {
            this.Parent = Parent;
            this.TimeOut = TimeOut == null ? TimeSpan.FromSeconds(10) : TimeOut.Value;
            this.RateLimit = RateLimit == null ? TimeSpan.FromSeconds(1) : RateLimit.Value;
        }

        public Tab Parent { get; }
        public TimeSpan TimeOut { get; }
        public TimeSpan RateLimit { get; }

        public async Task<Node> Wait(string CssSelector)
        {
            if (this.Parent.DOM == null) { throw new NullReferenceException(nameof(this.Parent)); }
            Stopwatch watch = Stopwatch.StartNew();

            while (true)
            {
                if (watch.Elapsed > this.TimeOut) { throw new TimeoutException(); }
                Node? target = await this.Parent.SelectNode(CssSelector);
                if (target != null) { return target; }
                await Task.Delay(RateLimit);
            }
        }
    }
}
using System;

namespace Xambon.PreLoader
{
    public class PreLoadResult
    {
        public PreLoadResult(TimeSpan absoluteExpirationRelativeToNow, object data)
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            Data = data;
        }

        public TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
        public object Data { get; set; }
    }
}
